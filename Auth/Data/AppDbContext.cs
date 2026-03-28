using dotnet_auth.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_auth.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>().ToTable("Users");

        modelBuilder.Entity<User>().HasData(
            new User
            {
                id = 10,
                username = "admin",
                name = "Administrator",
                email = "admin@example.com",
                password = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                id = 11,
                username = "user1",
                name = "Standard User",
                email = "user1@example.com",
                password = BCrypt.Net.BCrypt.HashPassword("User@123"),
                CreatedAt = new DateTime(2024, 1, 2, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}