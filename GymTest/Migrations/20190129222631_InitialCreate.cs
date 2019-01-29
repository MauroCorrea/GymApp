using System;
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
                name: "MedicalEmergency",
                columns: table => new
                {
                    MedicalEmergencyId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MedicalEmergencyDescription = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalEmergency", x => x.MedicalEmergencyId);
                });

            migrationBuilder.CreateTable(
                name: "MovementType",
                columns: table => new
                {
                    MovementTypeId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovementType", x => x.MovementTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Supplier",
                columns: table => new
                {
                    SupplierId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SupplierDescription = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supplier", x => x.SupplierId);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Token = table.Column<string>(nullable: false),
                    FullName = table.Column<string>(maxLength: 100, nullable: false),
                    BirthDate = table.Column<DateTime>(nullable: false),
                    DocumentNumber = table.Column<string>(maxLength: 20, nullable: false),
                    Email = table.Column<string>(maxLength: 50, nullable: true),
                    Address = table.Column<string>(maxLength: 200, nullable: true),
                    Phones = table.Column<string>(maxLength: 200, nullable: true),
                    ContactFullName = table.Column<string>(maxLength: 100, nullable: true),
                    ContactPhones = table.Column<string>(maxLength: 200, nullable: true),
                    SignInDate = table.Column<DateTime>(nullable: false),
                    MedicalEmergencyId = table.Column<int>(nullable: false),
                    HealthPhysicalProblems = table.Column<string>(maxLength: 1000, nullable: true),
                    HealthHeartProblems = table.Column<string>(maxLength: 1000, nullable: true),
                    HealthCronicalProblems = table.Column<string>(maxLength: 1000, nullable: true),
                    HealthRegularPills = table.Column<string>(maxLength: 1000, nullable: true),
                    Target = table.Column<string>(maxLength: 200, nullable: true),
                    Commentaries = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_User_MedicalEmergency_MedicalEmergencyId",
                        column: x => x.MedicalEmergencyId,
                        principalTable: "MedicalEmergency",
                        principalColumn: "MedicalEmergencyId",
                        onDelete: ReferentialAction.Cascade);
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
                    CashMovementDate = table.Column<DateTime>(nullable: false),
                    CashCategoryId = table.Column<int>(nullable: false),
                    SupplierId = table.Column<int>(nullable: false)
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
                    table.ForeignKey(
                        name: "FK_CashMovement_Supplier_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Supplier",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "Payment",
                columns: table => new
                {
                    PaymentId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PaymentDate = table.Column<DateTime>(nullable: false),
                    LimitUsableDate = table.Column<DateTime>(nullable: false),
                    MovementTypeId = table.Column<int>(nullable: false),
                    QuantityMovmentType = table.Column<int>(nullable: false),
                    Amount = table.Column<float>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_Payment_MovementType_MovementTypeId",
                        column: x => x.MovementTypeId,
                        principalTable: "MovementType",
                        principalColumn: "MovementTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payment_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
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

            migrationBuilder.CreateIndex(
                name: "IX_CashMovement_SupplierId",
                table: "CashMovement",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_MovementTypeId",
                table: "Payment",
                column: "MovementTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_UserId",
                table: "Payment",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_User_MedicalEmergencyId",
                table: "User",
                column: "MedicalEmergencyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assistance");

            migrationBuilder.DropTable(
                name: "CashMovement");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "CashCategory");

            migrationBuilder.DropTable(
                name: "CashMovementType");

            migrationBuilder.DropTable(
                name: "Supplier");

            migrationBuilder.DropTable(
                name: "MovementType");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "MedicalEmergency");
        }
    }
}
