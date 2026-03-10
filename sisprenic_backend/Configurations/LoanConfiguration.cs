using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using sisprenic_backend.Entities;

namespace sisprenic_backend.Configurations;

public class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id)
            .ValueGeneratedOnAdd();

        builder.Property(l => l.Principal)
            .IsRequired()
            .HasPrecision(12, 2);

        builder.Property(l => l.InterestRate)
            .IsRequired()
            .HasPrecision(5, 4);

        builder.Property(l => l.TermMonths)
            .IsRequired();

        builder.Property(l => l.StartDate)
            .IsRequired();

        builder.Property(l => l.ClientId)
            .IsRequired();

        builder
            .HasOne(l => l.Client)
            .WithMany(c => c.Loans)
            .HasForeignKey(l => l.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}