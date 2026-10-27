using Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Currency)
                .IsRequired()
                .HasMaxLength(3); // RUB, USD, EUR

            // Для денег обязательно указываем точность!
            // decimal(18,2) - 18 цифр всего, 2 после запятой
            builder.Property(a => a.Balance)
                .HasPrecision(18, 2);

            builder.HasOne(a => a.Family)
                .WithMany(f => f.Accounts)
                .HasForeignKey(a => a.FamilyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
