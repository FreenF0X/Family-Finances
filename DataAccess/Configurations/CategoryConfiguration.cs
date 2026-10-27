using Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Связь с семьей (может быть null для глобальных категорий)
            builder.HasOne(c => c.Family)
                .WithMany(f => f.Categories)
                .HasForeignKey(c => c.FamilyId)
                .OnDelete(DeleteBehavior.Cascade); // Если семью удаляют, её категории тоже удаляются

            // Связь с родительской категорией (иерархия)
            builder.HasOne(c => c.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Нельзя удалить категорию, если у неё есть подкатегории
        }
    }
}
