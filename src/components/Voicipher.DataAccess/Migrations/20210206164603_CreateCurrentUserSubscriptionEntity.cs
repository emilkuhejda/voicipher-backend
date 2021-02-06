using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Voicipher.DataAccess.Migrations
{
    public partial class CreateCurrentUserSubscriptionEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CurrentUserSubscription",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ticks = table.Column<long>(type: "bigint", nullable: false),
                    DateUpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrentUserSubscription", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CurrentUserSubscription_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscription_UserId",
                table: "UserSubscription",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrentUserSubscription_UserId",
                table: "CurrentUserSubscription",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSubscription_User_UserId",
                table: "UserSubscription",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSubscription_User_UserId",
                table: "UserSubscription");

            migrationBuilder.DropTable(
                name: "CurrentUserSubscription");

            migrationBuilder.DropIndex(
                name: "IX_UserSubscription_UserId",
                table: "UserSubscription");
        }
    }
}
