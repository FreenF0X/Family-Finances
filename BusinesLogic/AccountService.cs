using DataAccess;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinesLogic
{
    public class AccountService
    {
        private readonly BudgetDbContext _dbContext;

        public AccountService(BudgetDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Account> CreateAsync(Guid familyId, string name, string currency)
        {
            // Проверяем, существует ли семья
            var familyExists = await _dbContext.Families.AnyAsync(f => f.Id == familyId);
            if (!familyExists) throw new Exception("Семья не найдена");

            var account = new Account
            {
                Id = Guid.NewGuid(),
                FamilyId = familyId,
                Name = name,
                Currency = currency,
                Balance = 0, // Новый счет всегда с нулевым балансом
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Accounts.Add(account);
            await _dbContext.SaveChangesAsync();
            return account;
        }

        public async Task<List<Account>> GetByFamilyIdAsync(Guid familyId)
        {
            return await _dbContext.Accounts
                .AsNoTracking()
                .Where(a => a.FamilyId == familyId)
                .ToListAsync();
        }

        public async Task<Account?> UpdateAsync(Guid id, string name, string currency)
        {
            var account = await _dbContext.Accounts.FindAsync(id);
            if (account == null) return null;

            account.Name = name;
            account.Currency = currency;
            // Баланс не трогаем!

            await _dbContext.SaveChangesAsync();
            return account;
        }

        // Метод закрытия счета
        public async Task<Account?> CloseAsync(Guid id)
        {
            var account = await _dbContext.Accounts.FindAsync(id);
            if (account == null) return null;

            if (account.IsClosed)
                throw new Exception("Счет уже закрыт");

            if (account.Balance != 0)
                throw new Exception($"Нельзя закрыть счет с ненулевым балансом. Текущий баланс: {account.Balance}");

            account.IsClosed = true;
            await _dbContext.SaveChangesAsync();
            return account;
        }

        // Метод удаления счета (теперь с проверкой на транзакции)
        public async Task<bool> DeleteAsync(Guid id)
        {
            var account = await _dbContext.Accounts.FindAsync(id);
            if (account == null) return false;

            // Проверяем, есть ли транзакции по этому счету
            var hasTransactions = await _dbContext.Transactions.AnyAsync(t => t.AccountId == id);
            if (hasTransactions)
            {
                throw new Exception("Нельзя удалить счет, так как по нему есть транзакции. Вы можете только закрыть его.");
            }

            _dbContext.Accounts.Remove(account);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
