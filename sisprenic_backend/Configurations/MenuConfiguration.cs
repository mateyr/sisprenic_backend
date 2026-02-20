using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using sisprenic.Entities;

namespace sisprenic.Configurations;

public class MenuConfiguration : IEntityTypeConfiguration<Menu>
{
    public void Configure(EntityTypeBuilder<Menu> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).ValueGeneratedOnAdd();

        builder.Property(m => m.Name).IsRequired().HasMaxLength(200);
        builder.Property(m => m.Route).HasMaxLength(200);
        builder.Property(m => m.Icon).HasMaxLength(100);
        builder.Property(m => m.Section).HasMaxLength(200);
        builder.Property(m => m.RequiredClaim);
        builder.Property(m => m.Order).HasDefaultValue(0);

        builder.HasOne(m => m.ParentMenu)
            .WithMany(p => p.SubMenus)
            .HasForeignKey(m => m.ParentMenuId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}