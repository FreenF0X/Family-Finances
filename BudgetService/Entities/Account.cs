using System.Transactions;

namespace BudgetService.Entities
{
    public class Account
    {
        public Guid Id { get; set; }
        public Guid FamilyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Currency { get; set; } = "RUB"; // По умолчанию рубли

        // Хранимый баланс. Тип decimal!
        public decimal Balance { get; set; }

        public DateTime CreatedAt { get; set; }

        // Навигационные свойства
        public Family Family { get; set; } = null!;
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
