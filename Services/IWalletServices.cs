using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WebApplication2.Services;

public interface IWalletService
{
    Task<(IEnumerable<WalletDto> Wallets, PaginationMetadata Pagination)> GetWalletsAsync(
        long? customerId, int? status, string sortBy, bool isAscending, int page, int pageSize);

    Task<WalletDto> GetWalletByIdAsync(long id);
    Task<WalletDto> CreateWalletAsync(WalletDto walletDto);
    Task<WalletDto> UpdateWalletAsync(long id, WalletDto walletDto);
    Task<bool> DeleteWalletAsync(long id);
}


public class WalletService : IWalletService
{
    private readonly IRepository<Wallet> _walletRepository;

    public WalletService(IRepository<Wallet> walletRepository)
    {
        _walletRepository = walletRepository;
    }

    // Get wallets with filtering, sorting, and pagination
    public async Task<(IEnumerable<WalletDto> Wallets, PaginationMetadata Pagination)> GetWalletsAsync(
        long? customerId, int? status, string sortBy, bool isAscending, int page, int pageSize)
    {
        var query = _walletRepository.Query();
        bool hasFilters = false;

        // Filtering
        if (customerId.HasValue)
        {
            query = query.Where(w => w.CustomerId == customerId.Value);
            hasFilters = true;
        }
        if (status.HasValue)
        {
            query = query.Where(w => w.Status == status.Value);
            hasFilters = true;
        }

        if (!hasFilters)
        {
            query = _walletRepository.Query(); // Reset query to fetch all records
        }

        // Sorting
        if (!string.IsNullOrEmpty(sortBy))
        {
            query = isAscending
                ? query.OrderBy(w => EF.Property<object>(w, sortBy))
                : query.OrderByDescending(w => EF.Property<object>(w, sortBy));
        }

        // Pagination
        var totalCount = await _walletRepository.CountAsync(w => query.Contains(w));
        var walletsList = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        // Mapping
        var walletDtos = walletsList.Select(w => new WalletDto
        {
            WalletId = w.WalletId,
            Balance = w.Balance,
            CreateDate = w.CreateDate,
            Status = w.Status
        });

        var paginationMetadata = new PaginationMetadata
        {
            TotalItems = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };

        return (walletDtos, paginationMetadata);
    }

    // Get a single wallet by ID
    public async Task<WalletDto> GetWalletByIdAsync(long id)
    {
        var wallet = await _walletRepository.GetByIdAsync(id);
        if (wallet == null) return null;

        return new WalletDto
        {
            WalletId = wallet.WalletId,
            Balance = wallet.Balance,
            CreateDate = wallet.CreateDate,
            Status = wallet.Status
        };
    }

    // Create a wallet
    public async Task<WalletDto> CreateWalletAsync(WalletDto walletDto)
    {
        var wallet = new Wallet
        {
            Balance = walletDto.Balance,
            CreateDate = walletDto.CreateDate,
            Status = walletDto.Status
        };

        await _walletRepository.AddAsync(wallet);
        await _walletRepository.SaveChangesAsync();

        walletDto.WalletId = wallet.WalletId;
        return walletDto;
    }

    // Update a wallet
    public async Task<WalletDto> UpdateWalletAsync(long id, WalletDto walletDto)
    {
        var wallet = await _walletRepository.GetByIdAsync(id);
        if (wallet == null) return null;

        wallet.Balance = walletDto.Balance;
        wallet.CreateDate = walletDto.CreateDate;
        wallet.Status = walletDto.Status;

        await _walletRepository.UpdateAsync(wallet);
        await _walletRepository.SaveChangesAsync();

        return walletDto;
    }

    // Delete a wallet
    public async Task<bool> DeleteWalletAsync(long id)
    {
        var wallet = await _walletRepository.GetByIdAsync(id);
        if (wallet == null) return false;

        await _walletRepository.DeleteAsync(id);
        await _walletRepository.SaveChangesAsync();

        return true;
    }
}