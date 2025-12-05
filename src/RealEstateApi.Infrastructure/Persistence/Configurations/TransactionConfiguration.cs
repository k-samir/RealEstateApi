using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApi.Domain.Entities;

namespace RealEstateApi.Infrastructure.Persistence.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("transactions");
        builder.HasKey(t => t.Id);
        
        builder.HasOne(t => t.Client)
            .WithMany()
            .HasForeignKey(t => t.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        // UnitId is a string, so we need to configure it correctly if Unit.Id is string
        // But Transaction.UnitId is string.
        builder.HasOne(t => t.Unit)
            .WithMany()
            .HasForeignKey(t => t.UnitId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(t => t.DeclaredAmount).HasPrecision(18, 2);
        builder.Property(t => t.UndeclaredAmount).HasPrecision(18, 2);
        
        builder.HasMany(t => t.Payments)
            .WithOne()
            .HasForeignKey(p => p.TransactionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
