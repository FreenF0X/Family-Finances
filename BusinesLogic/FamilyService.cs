using Domain.Models;
using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace BusinesLogic
{
    public class FamilyService
    {
        private readonly BudgetDbContext _dbContext;

        public FamilyService(BudgetDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Family> CreateAsync(string name)
        {
            var family = new Family
            {
                Id = Guid.NewGuid(),
                Name = name,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Families.Add(family);
            await _dbContext.SaveChangesAsync();
            return family;
        }

        public async Task<List<Family>> GetAllAsync()
        {
            return await _dbContext.Families.AsNoTracking().ToListAsync();
        }

        public async Task<Family?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Families.FindAsync(id);
        }

        public async Task<Family?> UpdateAsync(Guid id, string newName)
        {
            var family = await _dbContext.Families.FindAsync(id);
            if (family == null) return null;

            family.Name = newName;
            await _dbContext.SaveChangesAsync();
            return family;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var family = await _dbContext.Families.FindAsync(id);
            if (family == null) return false;

            _dbContext.Families.Remove(family);
            await _dbContext.SaveChangesAsync();
            return true;
        }

    }
}
