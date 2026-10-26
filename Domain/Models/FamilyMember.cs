namespace Domain.Models
{
    public enum FamilyRole
    {
        Owner = 1,
        Member = 2
    }

    public class FamilyMember
    {
        public Guid Id { get; set; }
        public Guid FamilyId { get; set; }
        public Guid UserId { get; set; }
        public FamilyRole Role { get; set; }

        // Навигационные свойства
        public Family Family { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
