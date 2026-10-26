using System.Transactions;

namespace Domain.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Login { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? Email { get; set; }
        public DateTime CreatedAt { get; set; }

        // Навигационные свойства
        public ICollection<FamilyMember> FamilyMemberships { get; set; } = new List<FamilyMember>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
