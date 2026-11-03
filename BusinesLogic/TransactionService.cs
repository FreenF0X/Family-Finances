using DataAccess;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinesLogic
{
    public class TransactionService
    {
        private readonly BudgetDbContext _dbContext;

        public TransactionService(BudgetDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Transaction> CreateAsync(
            Guid accountId,
            Guid categoryId,
            decimal amount,
            DateTime date,
            string? comment,
            Guid userId)
        {
            if (amount <= 0)
                throw new Exception("Сумма транзакции должна быть больше нуля");

            // 1. Проверяем счет
            var account = await _dbContext.Accounts.FindAsync(accountId);
            if (account == null)
                throw new Exception("Счет не найден");

            if (account.IsClosed)
                throw new Exception("Нельзя совершать операции по закрытому счету");

            // 2. Проверяем категорию
            var category = await _dbContext.Categories.FindAsync(categoryId);
            if (category == null)
                throw new Exception("Категория не найдена");

            // 3. Создаем транзакцию
            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                CategoryId = categoryId,
                UserId = userId,
                Amount = amount,
                Type = category.Type,
                Date = date,
                Comment = comment,
                CreatedAt = DateTime.UtcNow
            };

            // 4. МЕНЯЕМ БАЛАНС (Бизнес-логика)
            if (category.Type == TransactionType.Expense)
            {
                account.Balance -= amount;
            }
            else
            {
                account.Balance += amount;
            }

            // 5. Сохраняем всё вместе (транзакция и изменение баланса)
            _dbContext.Transactions.Add(transaction);
            await _dbContext.SaveChangesAsync();

            return transaction;
        }

        public async Task<List<Transaction>> GetByAccountIdAsync(Guid accountId)
        {
            return await _dbContext.Transactions
                .AsNoTracking()
                .Where(t => t.AccountId == accountId)
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }
    }
}
