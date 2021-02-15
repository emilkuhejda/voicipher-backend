using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Voicipher.DataAccess.Migrations
{
    public partial class CreateRecognizedAudioSamplesEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecognizedAudioSample",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateCreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecognizedAudioSample", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SpeechResult",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecognizedAudioSampleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalTime = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpeechResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpeechResult_RecognizedAudioSample_RecognizedAudioSampleId",
                        column: x => x.RecognizedAudioSampleId,
                        principalTable: "RecognizedAudioSample",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpeechResult_RecognizedAudioSampleId",
                table: "SpeechResult",
                column: "RecognizedAudioSampleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpeechResult");

            migrationBuilder.DropTable(
                name: "RecognizedAudioSample");
        }
    }
}
