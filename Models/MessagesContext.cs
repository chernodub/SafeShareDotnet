using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace SafeShare.Models;

public class MessagesContext : DbContext
{
    public MessagesContext(DbContextOptions<MessagesContext> options) : base(options) { }

    public DbSet<Message> Messages { get; set; } = null!;
    public DbSet<File> Files { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli#excluding-parts-of-your-model
        modelBuilder.Entity<User>().ToTable("Users", t => t.ExcludeFromMigrations());

        // To protect/hide the neighbouring data, we generate the guids instead of integers
        modelBuilder.Entity<Message>()
            .Property(u => u.Id)
            .HasValueGenerator<GuidValueGenerator>();

        modelBuilder.Entity<File>()
            .Property(u => u.Id)
            .HasValueGenerator<GuidValueGenerator>();
    }
}