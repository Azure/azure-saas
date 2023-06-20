using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saas.Admin.Service.Migrations
{
    /// <inheritdoc />
    public partial class RegionalUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Organization_Route",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "Route",
                table: "Organization");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserInfo",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2023, 6, 19, 13, 14, 12, 34, DateTimeKind.Utc).AddTicks(9740),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 6, 13, 11, 52, 9, 139, DateTimeKind.Utc).AddTicks(607));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Organization",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 6, 19, 13, 14, 12, 33, DateTimeKind.Utc).AddTicks(1186),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 6, 13, 11, 52, 9, 138, DateTimeKind.Utc).AddTicks(4126));

            migrationBuilder.AddColumn<string>(
                name: "SqlServerRegion",
                table: "Organization",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "default");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiryDate",
                table: "Employee",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 9, 17, 13, 14, 12, 33, DateTimeKind.Utc).AddTicks(9194),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 9, 11, 11, 52, 9, 138, DateTimeKind.Utc).AddTicks(6357));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Employee",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 6, 19, 13, 14, 12, 34, DateTimeKind.Utc).AddTicks(3709),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 6, 13, 11, 52, 9, 138, DateTimeKind.Utc).AddTicks(7719));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SqlServerRegion",
                table: "Organization");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserInfo",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2023, 6, 13, 11, 52, 9, 139, DateTimeKind.Utc).AddTicks(607),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 6, 19, 13, 14, 12, 34, DateTimeKind.Utc).AddTicks(9740));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Organization",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 6, 13, 11, 52, 9, 138, DateTimeKind.Utc).AddTicks(4126),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 6, 19, 13, 14, 12, 33, DateTimeKind.Utc).AddTicks(1186));

            migrationBuilder.AddColumn<string>(
                name: "Route",
                table: "Organization",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiryDate",
                table: "Employee",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 9, 11, 11, 52, 9, 138, DateTimeKind.Utc).AddTicks(6357),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 9, 17, 13, 14, 12, 33, DateTimeKind.Utc).AddTicks(9194));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Employee",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 6, 13, 11, 52, 9, 138, DateTimeKind.Utc).AddTicks(7719),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 6, 19, 13, 14, 12, 34, DateTimeKind.Utc).AddTicks(3709));

            migrationBuilder.CreateIndex(
                name: "IX_Organization_Route",
                table: "Organization",
                column: "Route",
                unique: true);
        }
    }
}
