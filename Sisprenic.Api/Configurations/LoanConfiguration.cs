using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sisprenic.Domain.Entities;

namespace Sisprenic.Api.Configurations;

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

        builder.Property(l => l.Status)
            .IsRequired()
            .HasDefaultValue(LoanStatus.Active)
            .HasComment("Estado del préstamo: 0 = Activo, 1 = Pagado");

        builder.ToTable(t =>
            t.HasCheckConstraint(
                "CK_Loan_ValidStatus",
                "status IN (0, 1)"));

        builder.Property(l => l.IsDeleted).HasDefaultValue(false).IsRequired();
        builder.Property(l => l.DeletedOn).IsRequired(false);

        builder.HasQueryFilter(l => !l.IsDeleted);

        builder.HasIndex(l => l.IsDeleted)
            .HasFilter("is_deleted = false")
            .HasDatabaseName("ix_loan_is_deleted");
    }
}