using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saas.Admin.Service.Migrations
{
    /// <inheritdoc />
    public partial class Modified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Organization",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Route = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Company = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Industry = table.Column<int>(type: "int", nullable: false),
                    ProductTierId = table.Column<int>(type: "int", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Employees = table.Column<int>(type: "int", nullable: false),
                    InitReady = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ExternalDB = table.Column<bool>(type: "bit", nullable: false),
                    TimeZone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(2023, 6, 9, 14, 0, 37, 566, DateTimeKind.Utc).AddTicks(8480)),
                    UpdatedUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConcurrencyToken = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organization", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullNames = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConfirmPassword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Telephone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LockAfter = table.Column<int>(type: "int", nullable: false),
                    BioUserID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DOB = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IDType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Profession = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AcceptTerms = table.Column<bool>(type: "bit", nullable: false),
                    Notifications = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpNo = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "001"),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(2023, 9, 7, 14, 0, 37, 566, DateTimeKind.Utc).AddTicks(9471)),
                    ExpiresAfter = table.Column<int>(type: "int", nullable: false, defaultValue: 90),
                    SuperUser = table.Column<bool>(type: "bit", nullable: false),
                    CCCode = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "OO1"),
                    RegSource = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Profession = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrincipalUser = table.Column<bool>(type: "bit", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(2023, 6, 9, 14, 0, 37, 567, DateTimeKind.Utc).AddTicks(300)),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employee_Organization_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Organization",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employee_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employee_TenantId",
                table: "Employee",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_UserId",
                table: "Employee",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_Route",
                table: "Organization",
                column: "Route",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "Organization");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
