using Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Configurations
{
    public class FamilyMemberConfiguration : IEntityTypeConfiguration<FamilyMember>
    {
        public void Configure(EntityTypeBuilder<FamilyMember> builder)
        {
            builder.HasKey(fm => fm.Id);

            // Один пользователь может быть во многих семьях, но в одной семье - только один раз
            builder.HasIndex(fm => new { fm.FamilyId, fm.UserId }).IsUnique();

            builder.HasOne(fm => fm.Family)
                .WithMany(f => f.Members)
                .HasForeignKey(fm => fm.FamilyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(fm => fm.User)
                .WithMany(u => u.FamilyMemberships)
                .HasForeignKey(fm => fm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Enum будет храниться как int (по умолчанию), это нормально для PostgreSQL
            builder.Property(fm => fm.Role)
                .IsRequired();
        }
    }
}
