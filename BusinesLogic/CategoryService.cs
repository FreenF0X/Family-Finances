using DataAccess;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinesLogic
{
    public class CategoryService
    {
        private readonly BudgetDbContext _dbContext;

        public CategoryService(BudgetDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Category> CreateAsync(Guid? familyId, string name, TransactionType type, Guid? parentCategoryId)
        {
            // Если передан parentCategoryId, проверяем, что родитель существует
            if (parentCategoryId.HasValue)
            {
                var parentExists = await _dbContext.Categories.AnyAsync(c => c.Id == parentCategoryId.Value);
                if (!parentExists) throw new Exception("Родительская категория не найдена");
            }

            var category = new Category
            {
                Id = Guid.NewGuid(),
                FamilyId = familyId,
                Name = name,
                Type = type,
                ParentCategoryId = parentCategoryId
            };

            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();
            return category;
        }

        public async Task<List<Category>> GetAvailableForFamilyAsync(Guid familyId)
        {
            // Возвращаем и глобальные категории (FamilyId == null), и категории этой семьи
            return await _dbContext.Categories
                .AsNoTracking()
                .Where(c => c.FamilyId == null || c.FamilyId == familyId)
                .ToListAsync();
        }

        public async Task<Category?> UpdateAsync(Guid id, string name, TransactionType type)
        {
            var category = await _dbContext.Categories.FindAsync(id);
            if (category == null) return null;

            category.Name = name;
            category.Type = type;

            await _dbContext.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var category = await _dbContext.Categories.FindAsync(id);
            if (category == null) return false;

            // ПРОВЕРКА: Нельзя удалить категорию, если по ней уже есть транзакции
            var hasTransactions = await _dbContext.Transactions.AnyAsync(t => t.CategoryId == id);
            if (hasTransactions)
            {
                throw new Exception("Нельзя удалить категорию, так как к ней привязаны транзакции");
            }

            // Проверка: есть ли подкатегории (у нас в конфигурации Restrict)
            var hasSubCategories = await _dbContext.Categories.AnyAsync(c => c.ParentCategoryId == id);
            if (hasSubCategories)
            {
                throw new Exception("Нельзя удалить категорию, так как у нее есть подкатегории");
            }

            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
