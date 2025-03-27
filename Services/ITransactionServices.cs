using LinqKit;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication2.Services
{
    public interface ITransactionService
    {
        Task<(IEnumerable<TransactionDto> Transactions, PaginationMetadata Pagination)> GetTransactionsAsync(decimal? transactionAmount, DateTime? transactionDate, int? transactionType, int? status, long? walletId, int? orderId, int? paymentId, string sortBy, bool isAscending, int page, int pageSize);
        Task<TransactionDto> GetTransactionByIdAsync(long id);
        Task<TransactionDto> CreateTransactionAsync(TransactionDto transactionDto);
        Task<TransactionDto> UpdateTransactionAsync(long id, TransactionDto transactionDto);
        Task<bool> DeleteTransactionAsync(long id);
    }

    public class TransactionService : ITransactionService
    {
        private readonly IRepository<Transaction> _transactionRepository;

        public TransactionService(IRepository<Transaction> transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<(IEnumerable<TransactionDto> Transactions, PaginationMetadata Pagination)> GetTransactionsAsync(decimal? transactionAmount, DateTime? transactionDate, int? transactionType, int? status, long? walletId, int? orderId, int? paymentId, string sortBy, bool isAscending, int page, int pageSize)
        {
            var query = _transactionRepository.Query();

            if (transactionAmount.HasValue)
                query = query.Where(t => t.TransactionAmount == transactionAmount);
            if (transactionDate.HasValue)
                query = query.Where(t => t.TransactionDate == transactionDate);
            if (transactionType.HasValue)
                query = query.Where(t => t.TransactionType == transactionType);
            if (status.HasValue)
                query = query.Where(t => t.Status == status);
            if (walletId.HasValue)
                query = query.Where(t => t.WalletId == walletId);
            if (orderId.HasValue)
                query = query.Where(t => t.OrderId == orderId);
            if (paymentId.HasValue)
                query = query.Where(t => t.PaymentId == paymentId);

            if (!string.IsNullOrEmpty(sortBy) && typeof(Transaction).GetProperty(sortBy) != null)
            {
                query = isAscending
                    ? query.OrderBy(t => EF.Property<object>(t, sortBy))
                    : query.OrderByDescending(t => EF.Property<object>(t, sortBy));
            }

            var totalCount = await query.CountAsync();
            var transactions = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var transactionDtos = transactions.Select(t => new TransactionDto
            {
                TransactionId = t.TransactionId,
                TransactionAmount = t.TransactionAmount,
                TransactionDate = t.TransactionDate,
                TransactionType = t.TransactionType,
                Status = t.Status,
                WalletId = t.WalletId,
                OrderId = t.OrderId,
                PaymentId = t.PaymentId
            });

            var paginationMetadata = new PaginationMetadata
            {
                TotalItems = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return (transactionDtos, paginationMetadata);
        }

        public async Task<TransactionDto> GetTransactionByIdAsync(long id)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id);
            if (transaction == null)
                return null;

            return new TransactionDto
            {
                TransactionId = transaction.TransactionId,
                TransactionAmount = transaction.TransactionAmount,
                TransactionDate = transaction.TransactionDate,
                TransactionType = transaction.TransactionType,
                Status = transaction.Status,
                WalletId = transaction.WalletId,
                OrderId = transaction.OrderId,
                PaymentId = transaction.PaymentId
            };
        }

        public async Task<TransactionDto> CreateTransactionAsync(TransactionDto transactionDto)
        {
            var transaction = new Transaction
            {
                TransactionAmount = transactionDto.TransactionAmount,
                TransactionDate = transactionDto.TransactionDate,
                TransactionType = transactionDto.TransactionType,
                Status = transactionDto.Status,
                WalletId = transactionDto.WalletId,
                OrderId = transactionDto.OrderId,
                PaymentId = transactionDto.PaymentId
            };

            await _transactionRepository.AddAsync(transaction);
            await _transactionRepository.SaveChangesAsync();
            transactionDto.TransactionId = transaction.TransactionId;
            return transactionDto;
        }

        public async Task<TransactionDto> UpdateTransactionAsync(long id, TransactionDto transactionDto)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id);
            if (transaction == null)
                return null;

            transaction.TransactionAmount = transactionDto.TransactionAmount;
            transaction.TransactionDate = transactionDto.TransactionDate;
            transaction.TransactionType = transactionDto.TransactionType;
            transaction.Status = transactionDto.Status;
            transaction.WalletId = transactionDto.WalletId;
            transaction.OrderId = transactionDto.OrderId;
            transaction.PaymentId = transactionDto.PaymentId;

            await _transactionRepository.UpdateAsync(transaction);
            await _transactionRepository.SaveChangesAsync();

            return transactionDto;
        }

        public async Task<bool> DeleteTransactionAsync(long id)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id);
            if (transaction == null)
                return false;

            await _transactionRepository.DeleteAsync(id);
            await _transactionRepository.SaveChangesAsync();
            return true;
        }
    }
}