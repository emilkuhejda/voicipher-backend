using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Voicipher.DataAccess.Migrations
{
    public partial class CreateBillingPurchaseEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BillingPurchase",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PurchaseId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    AutoRenewing = table.Column<bool>(type: "bit", nullable: false),
                    PurchaseToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PurchaseState = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ConsumptionState = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Platform = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    TransactionDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillingPurchase", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BillingPurchase");
        }
    }
}
