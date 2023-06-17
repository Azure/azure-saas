using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saas.Admin.Service.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedTenant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserInfo",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2023, 6, 13, 11, 52, 9, 139, DateTimeKind.Utc).AddTicks(607),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 6, 12, 12, 48, 39, 491, DateTimeKind.Utc).AddTicks(3983));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Organization",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 6, 13, 11, 52, 9, 138, DateTimeKind.Utc).AddTicks(4126),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 6, 12, 12, 48, 39, 490, DateTimeKind.Utc).AddTicks(6714));

            migrationBuilder.AddColumn<string>(
                name: "DatabaseName",
                table: "Organization",
                type: "nvarchar(max)",
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
                oldDefaultValue: new DateTime(2023, 9, 10, 12, 48, 39, 490, DateTimeKind.Utc).AddTicks(8842));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Employee",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 6, 13, 11, 52, 9, 138, DateTimeKind.Utc).AddTicks(7719),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 6, 12, 12, 48, 39, 491, DateTimeKind.Utc).AddTicks(458));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DatabaseName",
                table: "Organization");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserInfo",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2023, 6, 12, 12, 48, 39, 491, DateTimeKind.Utc).AddTicks(3983),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 6, 13, 11, 52, 9, 139, DateTimeKind.Utc).AddTicks(607));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Organization",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 6, 12, 12, 48, 39, 490, DateTimeKind.Utc).AddTicks(6714),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 6, 13, 11, 52, 9, 138, DateTimeKind.Utc).AddTicks(4126));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiryDate",
                table: "Employee",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 9, 10, 12, 48, 39, 490, DateTimeKind.Utc).AddTicks(8842),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 9, 11, 11, 52, 9, 138, DateTimeKind.Utc).AddTicks(6357));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Employee",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 6, 12, 12, 48, 39, 491, DateTimeKind.Utc).AddTicks(458),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 6, 13, 11, 52, 9, 138, DateTimeKind.Utc).AddTicks(7719));
        }
    }
}
