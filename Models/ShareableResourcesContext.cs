using Microsoft.EntityFrameworkCore;

namespace SafeShare.Models;

public class ShareableResourcesContext : DbContext
{
    public ShareableResourcesContext(DbContextOptions<ShareableResourcesContext> options) : base(options) { }

    public DbSet<ShareableMessage> Messages { get; set; } = null!;
}