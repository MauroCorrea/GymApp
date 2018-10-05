using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GymTest.Migrations
{
    public partial class WinDB1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MovmentTypeId",
                table: "Payment",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QuantityMovmentType",
                table: "Payment",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Assistance",
                columns: table => new
                {
                    AssistanceId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AssistanceDate = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assistance", x => x.AssistanceId);
                    table.ForeignKey(
                        name: "FK_Assistance_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CashCategory",
                columns: table => new
                {
                    CashCategoryId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CashCategoryDescription = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashCategory", x => x.CashCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "CashMovementType",
                columns: table => new
                {
                    CashMovementTypeId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CashMovementTypeDescription = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashMovementType", x => x.CashMovementTypeId);
                });

            migrationBuilder.CreateTable(
                name: "CashMovement",
                columns: table => new
                {
                    CashMovementId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CashMovementDetails = table.Column<string>(maxLength: 200, nullable: true),
                    Amount = table.Column<float>(nullable: false),
                    CashMovementTypeId = table.Column<int>(nullable: false),
                    CashCategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashMovement", x => x.CashMovementId);
                    table.ForeignKey(
                        name: "FK_CashMovement_CashCategory_CashCategoryId",
                        column: x => x.CashCategoryId,
                        principalTable: "CashCategory",
                        principalColumn: "CashCategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CashMovement_CashMovementType_CashMovementTypeId",
                        column: x => x.CashMovementTypeId,
                        principalTable: "CashMovementType",
                        principalColumn: "CashMovementTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assistance_UserId",
                table: "Assistance",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CashMovement_CashCategoryId",
                table: "CashMovement",
                column: "CashCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CashMovement_CashMovementTypeId",
                table: "CashMovement",
                column: "CashMovementTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assistance");

            migrationBuilder.DropTable(
                name: "CashMovement");

            migrationBuilder.DropTable(
                name: "CashCategory");

            migrationBuilder.DropTable(
                name: "CashMovementType");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Token",
                table: "User");

            migrationBuilder.DropColumn(
                name: "MovmentTypeId",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "QuantityMovmentType",
                table: "Payment");

            migrationBuilder.AlterColumn<string>(
                name: "Phones",
                table: "User",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "User",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "User",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "User",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "User",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
