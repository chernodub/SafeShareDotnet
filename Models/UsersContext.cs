using Microsoft.EntityFrameworkCore;

using SafeShare.Models;

public class UsersContext : DbContext
{
    public UsersContext(DbContextOptions<UsersContext> options) : base(options) { }

    public DbSet<User> Users { get; set; } = null!;
}