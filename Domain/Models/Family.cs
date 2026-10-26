using System.Security.Principal;

namespace Domain.Models
{
    public class Family
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Навигационные свойства
        public ICollection<FamilyMember> Members { get; set; } = new List<FamilyMember>();
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
        public ICollection<Category> Categories { get; set; } = new List<Category>();
    }
}
