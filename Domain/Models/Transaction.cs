namespace Domain.Models
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public Guid CategoryId { get; set; }
        public Guid UserId { get; set; } // Кто создал транзакцию

        // Тип дублируется из категории для денормализации (осознанное решение)
        public TransactionType Type { get; set; }

        // Деньги только в decimal!
        public decimal Amount { get; set; }

        public DateTime Date { get; set; } // Когда произошла транзакция
        public DateTime CreatedAt { get; set; } // Когда запись создана в БД
        public string? Comment { get; set; }

        // Навигационные свойства
        public Account Account { get; set; } = null!;
        public Category Category { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
