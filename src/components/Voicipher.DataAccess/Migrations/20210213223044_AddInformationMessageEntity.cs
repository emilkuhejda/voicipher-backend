using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Voicipher.DataAccess.Migrations
{
    public partial class AddInformationMessageEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InformationMessage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CampaignName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    WasOpened = table.Column<bool>(type: "bit", nullable: false),
                    DateCreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateUpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DatePublishedUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InformationMessage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LanguageVersion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InformationMessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Language = table.Column<int>(type: "int", nullable: false),
                    SentOnOsx = table.Column<bool>(type: "bit", nullable: false),
                    SentOnAndroid = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguageVersion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LanguageVersion_InformationMessage_InformationMessageId",
                        column: x => x.InformationMessageId,
                        principalTable: "InformationMessage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LanguageVersion_InformationMessageId",
                table: "LanguageVersion",
                column: "InformationMessageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LanguageVersion");

            migrationBuilder.DropTable(
                name: "InformationMessage");
        }
    }
}
