using Microsoft.EntityFrameworkCore.Migrations;

namespace Voicipher.DataAccess.Migrations
{
    public partial class CreateInternalValueEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_BillingPurchase_UserId",
                table: "BillingPurchase",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BillingPurchase_User_UserId",
                table: "BillingPurchase",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BillingPurchase_User_UserId",
                table: "BillingPurchase");

            migrationBuilder.DropIndex(
                name: "IX_BillingPurchase_UserId",
                table: "BillingPurchase");
        }
    }
}
