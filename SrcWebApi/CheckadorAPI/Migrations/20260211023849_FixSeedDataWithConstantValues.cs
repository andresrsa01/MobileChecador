using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CheckadorAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixSeedDataWithConstantValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "GeofenceConfigs",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "$2a$11$2oHZX7cKZmJ5qF5kW3kfD.8YqhJ8wHZF6N8kGv9fMp0YxZQdB8d6K" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "$2a$11$3pHZX8dLZnK6rG6lX4lgE.9ZriI9xIaG7O9lHw0gNq1ZyaRdC9e7L" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "GeofenceConfigs",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 11, 2, 36, 14, 439, DateTimeKind.Utc).AddTicks(9798));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 11, 2, 36, 14, 439, DateTimeKind.Utc).AddTicks(1942), "$2a$11$b5Se71LKv3E79PhL1Ll5EemKIZJueVZ.UOJaewZMz0zDQNym/uJLK" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 11, 2, 36, 14, 439, DateTimeKind.Utc).AddTicks(2369), "$2a$11$fnGIB.c58RihSeIhV6GsYu26iJk2XbwyRoGCqKf67I...QMsZC51S" });
        }
    }
}
