using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace dotnet_auth.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialUsersFixedV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "id", "CreatedAt", "email", "name", "password", "username" },
                values: new object[,]
                {
                    { 10, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@example.com", "Administrator", "hashed_admin_password", "admin" },
                    { 11, new DateTime(2024, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), "user1@example.com", "Standard User", "hashed_user1_password", "user1" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "id",
                keyValue: 11);
        }
    }
}
