using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Voicipher.DataAccess.Migrations
{
    public partial class Release03 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PurchaseStateTransaction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BillingPurchaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PreviousPurchaseState = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    PurchaseState = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    TransactionDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseStateTransaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseStateTransaction_BillingPurchase_BillingPurchaseId",
                        column: x => x.BillingPurchaseId,
                        principalTable: "BillingPurchase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseStateTransaction_BillingPurchaseId",
                table: "PurchaseStateTransaction",
                column: "BillingPurchaseId");

            migrationBuilder
                .Sql($"INSERT INTO [PurchaseStateTransaction] ([Id], [BillingPurchaseId], [PreviousPurchaseState], [PurchaseState], [TransactionDateUtc]) SELECT NEWID() AS [Id], [Id] AS [BillingPurchaseId], '' AS [PreviousPurchaseState], [PurchaseState], [TransactionDateUtc] FROM [BillingPurchase]");

            migrationBuilder.DropColumn(
                name: "PurchaseState",
                table: "BillingPurchase");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PurchaseState",
                table: "BillingPurchase",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.DropTable(
                name: "PurchaseStateTransaction");
        }
    }
}
