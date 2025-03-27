using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WebApplication2.Services;

public interface IWalletService
{
    Task<(IEnumerable<WalletDto> Wallets, PaginationMetadata Pagination)> GetWalletsAsync(decimal? minBalance, decimal? maxBalance, DateTime? createDate, int? status, long? customerId, string sortBy, bool isAscending, int page, int pageSize);
    Task<WalletDto> GetWalletByIdAsync(long id);
    Task<WalletDto> GetWalletByCustomerIdAsync(long? customerId);
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

    public async Task<(IEnumerable<WalletDto> Wallets, PaginationMetadata Pagination)> GetWalletsAsync(decimal? minBalance, decimal? maxBalance, DateTime? createDate, int? status, long? customerId, string sortBy, bool isAscending, int page, int pageSize)
    {
        var query = _walletRepository.Query();

        if (minBalance.HasValue)
            query = query.Where(w => w.Balance >= minBalance);
        if (maxBalance.HasValue)
            query = query.Where(w => w.Balance <= maxBalance);
        if (createDate.HasValue)
            query = query.Where(w => w.CreateDate == createDate);
        if (status.HasValue)
            query = query.Where(w => w.Status == status);
        if (customerId.HasValue)
            query = query.Where(w => w.CustomerId == customerId);

        if (!string.IsNullOrEmpty(sortBy) && typeof(Wallet).GetProperty(sortBy) != null)
        {
            query = isAscending
                ? query.OrderBy(w => EF.Property<object>(w, sortBy))
                : query.OrderByDescending(w => EF.Property<object>(w, sortBy));
        }

        var totalCount = await query.CountAsync();
        var wallets = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        var walletDtos = wallets.Select(w => new WalletDto
        {
            WalletId = w.WalletId,
            Balance = w.Balance,
            CreateDate = w.CreateDate,
            Status = w.Status,
            CustomerId = w.CustomerId
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

    public async Task<WalletDto> GetWalletByIdAsync(long id)
    {
        var wallet = await _walletRepository.GetByIdAsync(id);
        if (wallet == null)
            return null;

        return new WalletDto
        {
            WalletId = wallet.WalletId,
            Balance = wallet.Balance,
            CreateDate = wallet.CreateDate,
            Status = wallet.Status,
            CustomerId = wallet.CustomerId
        };
    }

    public async Task<WalletDto> GetWalletByCustomerIdAsync(long? customerId)
    {
        var wallet = await _walletRepository.Query()
            .Where(w => w.CustomerId == customerId)
            .FirstOrDefaultAsync();

        if (wallet == null)
            return null;

        return new WalletDto
        {
            WalletId = wallet.WalletId,
            Balance = wallet.Balance,
            CustomerId = wallet.CustomerId,
            Status = wallet.Status
        };
    }

    public async Task<WalletDto> CreateWalletAsync(WalletDto walletDto)
    {
        var wallet = new Wallet
        {
            Balance = walletDto.Balance,
            CreateDate = walletDto.CreateDate,
            Status = walletDto.Status,
            CustomerId = walletDto.CustomerId
        };

        await _walletRepository.AddAsync(wallet);
        await _walletRepository.SaveChangesAsync();
        walletDto.WalletId = wallet.WalletId;
        return walletDto;
    }

    public async Task<WalletDto> UpdateWalletAsync(long id, WalletDto walletDto)
    {
        var wallet = await _walletRepository.GetByIdAsync(id);
        if (wallet == null)
            return null;

        wallet.Balance = walletDto.Balance;
        wallet.CreateDate = walletDto.CreateDate;
        wallet.Status = walletDto.Status;
        wallet.CustomerId = walletDto.CustomerId;

        await _walletRepository.UpdateAsync(wallet);
        await _walletRepository.SaveChangesAsync();

        return walletDto;
    }

    public async Task<bool> DeleteWalletAsync(long id)
    {
        var wallet = await _walletRepository.GetByIdAsync(id);
        if (wallet == null)
            return false;

        await _walletRepository.DeleteAsync(id);
        await _walletRepository.SaveChangesAsync();
        return true;
    }
}