using System.Transactions;

namespace BudgetService.Entities
{
    public enum TransactionType
    {
        Expense = 1, // Расход
        Income = 2   // Доход
    }

    public class Category
    {
        public Guid Id { get; set; }
        // Если null — это глобальная системная категория
        public Guid? FamilyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public TransactionType Type { get; set; }

        // Для иерархии (например, "Еда" -> "Рестораны"). Пока опционально.
        public Guid? ParentCategoryId { get; set; }

        // Навигационные свойства
        public Family? Family { get; set; }
        public Category? ParentCategory { get; set; }
        public ICollection<Category> SubCategories { get; set; } = new List<Category>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
