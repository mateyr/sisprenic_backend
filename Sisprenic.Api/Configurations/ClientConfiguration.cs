using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sisprenic.Domain.Entities;

namespace Sisprenic.Api.Configurations
{
    public class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id).ValueGeneratedOnAdd();

            builder.Property(c => c.FirstName).IsRequired().HasMaxLength(100);

            builder.Property(c => c.SecondName).HasMaxLength(100);

            builder.Property(c => c.LastName).IsRequired().HasMaxLength(100);

            builder.Property(c => c.SecondLastName).HasMaxLength(100);

            builder.Property(c => c.Identification).IsRequired().HasMaxLength(50);

            builder.Property(c => c.PhoneNumber).IsRequired().HasMaxLength(30);

            // Unique constraint for identification
            builder.HasIndex(c => c.Identification)
                .IsUnique()
                .HasFilter("is_deleted = false");

            builder.HasMany(c => c.Loans)
                .WithOne(c => c.Client)
                .HasForeignKey(c => c.ClientId);

            builder.Property(c => c.IsDeleted).HasDefaultValue(false).IsRequired();
            builder.Property(c => c.DeletedOn).IsRequired(false);

            builder.HasQueryFilter(c => !c.IsDeleted);

            builder.HasIndex(c => c.IsDeleted)
                .HasFilter("is_deleted = false")
                .HasDatabaseName("ix_client_is_deleted");
        }
    }
}
