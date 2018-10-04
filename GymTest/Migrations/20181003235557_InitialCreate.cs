using Microsoft.EntityFrameworkCore.Migrations;

namespace GymTest.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "CashMovement");

            migrationBuilder.DropTable(
                name: "CashCategory");

            migrationBuilder.DropTable(
                name: "CashMovementType");
        }
    }
}
