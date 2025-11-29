using Microsoft.EntityFrameworkCore;
using RealEstateApi.Domain.Entities;
using System.Text.Json;

namespace RealEstateApi.Infrastructure.Persistence;

/// <summary>
/// EF Core DbContext for Real Estate API
/// Infrastructure adapter - implements data access
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Property> Properties => Set<Property>();
    public DbSet<Unit> Units => Set<Unit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply entity configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
