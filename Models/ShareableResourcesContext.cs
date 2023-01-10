using Microsoft.EntityFrameworkCore;

namespace SafeShare.Models;

public class ShareableResourcesContext : DbContext
{
    public ShareableResourcesContext(DbContextOptions<ShareableResourcesContext> options) : base(options) { }

    public DbSet<ShareableMessage> Messages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli#excluding-parts-of-your-model
        modelBuilder.Entity<User>().ToTable("Users", t => t.ExcludeFromMigrations());
    }
}