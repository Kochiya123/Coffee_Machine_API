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
        Task<(IEnumerable<Transaction> Transactions, PaginationMetadata Pagination)> GetTransactionsAsync(
            long? TransactionId, decimal? TransactionAmount, DateTime? TransactionDate, int? TransactionType, int? Status, long? WalletId, int? OrderId, int? PaymentId,
            string sortBy, bool isAscending, int page, int pageSize);
        Task<Transaction> GetTransactionByIdAsync(long id);
        Task<Transaction> CreateTransactionAsync(Transaction transaction);
        Task<Transaction> UpdateTransactionAsync(long id, Transaction transaction);
        Task<bool> DeleteTransactionAsync(long id);
    }

    public class TransactionService : ITransactionService
    {
        private readonly IRepository<Transaction> _transactionRepository;

        public TransactionService(IRepository<Transaction> transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<(IEnumerable<Transaction> Transactions, PaginationMetadata Pagination)> GetTransactionsAsync(
            long? TransactionId, decimal? TransactionAmount, DateTime? TransactionDate, int? TransactionType, int? Status, long? WalletId, int? OrderId, int? PaymentId,
            string sortBy, bool isAscending, int page, int pageSize)
        {
            var query = _transactionRepository.Query(); // Start with IQueryable
            bool hasFilters = false;

            // Apply filtering
            if (TransactionId.HasValue)
            {
                query = query.Where(t => t.TransactionId == TransactionId);
                hasFilters = true;
            }
            if (TransactionAmount.HasValue)
            {
                query = query.Where(t => t.TransactionAmount == TransactionAmount);
                hasFilters = true;
            }
            if (TransactionDate.HasValue)
            {
                query = query.Where(t => t.TransactionDate == TransactionDate);
                hasFilters = true;
            }
            if (TransactionType.HasValue)
            {
                query = query.Where(t => t.TransactionType == TransactionType);
                hasFilters = true;
            }
            if (Status.HasValue)
            {
                query = query.Where(t => t.Status == Status);
                hasFilters = true;
            }
            if (WalletId.HasValue)
            {
                query = query.Where(t => t.WalletId == WalletId);
                hasFilters = true;
            }
            if (OrderId.HasValue)
            {
                query = query.Where(t => t.OrderId == OrderId);
                hasFilters = true;
            }
            if (PaymentId.HasValue)
            {
                query = query.Where(t => t.PaymentId == PaymentId);
                hasFilters = true;
            }

            if (!hasFilters)
            {
                query = _transactionRepository.Query(); // Reset query to fetch all records
            }

            // Validate and apply sorting
            if (!string.IsNullOrEmpty(sortBy) && typeof(Transaction).GetProperty(sortBy) != null)
            {
                query = isAscending
                    ? query.OrderBy(t => EF.Property<object>(t, sortBy))
                    : query.OrderByDescending(t => EF.Property<object>(t, sortBy));
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var transactions = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            // Create pagination metadata
            var paginationMetadata = new PaginationMetadata
            {
                TotalItems = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return (transactions, paginationMetadata);
        }

        public async Task<Transaction> GetTransactionByIdAsync(long id)
        {
            return await _transactionRepository.Query()
                .Where(t => t.TransactionId == id)
                .FirstOrDefaultAsync();
        }

        public async Task<Transaction> CreateTransactionAsync(Transaction transaction)
        {
            await _transactionRepository.AddAsync(transaction);
            await _transactionRepository.SaveChangesAsync();
            return transaction;
        }

        public async Task<Transaction> UpdateTransactionAsync(long id, Transaction transaction)
        {
            var existingTransaction = await _transactionRepository.GetByIdAsync(id);
            if (existingTransaction == null)
            {
                return null;
            }

            existingTransaction.TransactionAmount = transaction.TransactionAmount;
            existingTransaction.TransactionDate = transaction.TransactionDate;
            existingTransaction.TransactionType = transaction.TransactionType;
            existingTransaction.Status = transaction.Status;
            existingTransaction.WalletId = transaction.WalletId;
            existingTransaction.OrderId = transaction.OrderId;
            existingTransaction.PaymentId = transaction.PaymentId;

            await _transactionRepository.UpdateAsync(existingTransaction);
            await _transactionRepository.SaveChangesAsync();

            return existingTransaction;
        }

        public async Task<bool> DeleteTransactionAsync(long id)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id);
            if (transaction == null)
            {
                return false;
            }

            await _transactionRepository.DeleteAsync(id);
            await _transactionRepository.SaveChangesAsync();

            return true;
        }
    }
}