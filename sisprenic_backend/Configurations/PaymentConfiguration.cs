using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using sisprenic_backend.Entities;

namespace sisprenic_backend.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).ValueGeneratedOnAdd();

        builder.Property(p => p.Interest)
            .HasColumnType("decimal(12,2)")
            .IsRequired();

        builder.Property(p => p.Principal)
            .HasColumnType("decimal(12,2)")
            .IsRequired();

        builder.Property(p => p.PaymentDay).IsRequired();

        builder.Property(p => p.Note).HasMaxLength(500);

        builder.HasOne(p => p.Loan)
            .WithMany(l => l.Payments)
            .HasForeignKey(p => p.LoanId);

        // Evita pagos inválidos:
        // - No permite valores negativos
        // - No permite que Principal e Interest sean ambos 0
        builder.ToTable(t => 
            t.HasCheckConstraint(
                "CK_Payment_NonNegativeAndNotBothZero", 
                "Principal >= 0 AND Interest >= 0 AND (Principal > 0 OR Interest > 0)"
            )
        );
    }
}
