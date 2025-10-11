using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dotnet_auth.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            // Seed data với password đã hash
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Name", "Email", "Password", "CreatedAt" },
                values: new object[,]
                {
                    {
                        "1",
                        "admin",
                        "admin@example.com",
                        BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                        DateTime.UtcNow
                    },
                    {
                        "2",
                        "john_doe",
                        "john@example.com",
                        BCrypt.Net.BCrypt.HashPassword("John@123"),
                        DateTime.UtcNow
                    },
                    {
                        "3",
                        "jane_smith",
                        "jane@example.com",
                        BCrypt.Net.BCrypt.HashPassword("Jane@123"),
                        DateTime.UtcNow
                    },
                    {
                        "4",
                        "testuser",
                        "test@example.com",
                        BCrypt.Net.BCrypt.HashPassword("Test@123"),
                        DateTime.UtcNow
                    }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
