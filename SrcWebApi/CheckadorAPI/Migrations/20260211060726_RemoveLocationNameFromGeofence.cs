using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CheckadorAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLocationNameFromGeofence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocationName",
                table: "GeofenceConfigs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LocationName",
                table: "GeofenceConfigs",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "GeofenceConfigs",
                keyColumn: "Id",
                keyValue: 1,
                column: "LocationName",
                value: "Oficina Central");
        }
    }
}
