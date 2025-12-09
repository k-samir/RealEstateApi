using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApi.Domain.Entities;

namespace RealEstateApi.Infrastructure.Persistence.Configurations;

public class UserPermissionsConfiguration : IEntityTypeConfiguration<UserPermissions>
{
    public void Configure(EntityTypeBuilder<UserPermissions> builder)
    {
        builder.ToTable("user_permissions");

        builder.HasKey(u => u.UserId);

        builder.Property(u => u.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(u => u.Role)
            .HasColumnName("role")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.AgentTier)
            .HasColumnName("agent_tier")
            .HasMaxLength(50);

        builder.Property(u => u.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(u => u.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        // Configure owned types for permissions (stored as JSON)
        builder.OwnsOne(u => u.Properties, props =>
        {
            props.ToJson("properties_permissions");
        });

        builder.OwnsOne(u => u.Clients, clients =>
        {
            clients.ToJson("clients_permissions");
        });

        builder.OwnsOne(u => u.Transactions, trans =>
        {
            trans.ToJson("transactions_permissions");
        });
    }
}
