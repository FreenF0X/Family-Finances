using System.Transactions;

namespace Domain.Models
{
    public class AppUser
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Навигационные свойства
        public ICollection<FamilyMember> FamilyMemberships { get; set; } = new List<FamilyMember>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
