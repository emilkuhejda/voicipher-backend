using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Voicipher.DataAccess.Migrations
{
    public partial class CreateAudioFileEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AudioFile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Language = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RecognitionState = table.Column<int>(type: "int", nullable: false),
                    OriginalSourceFileName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SourceFileName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Storage = table.Column<int>(type: "int", nullable: false),
                    UploadStatus = table.Column<int>(type: "int", nullable: false),
                    TotalTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    TranscribedTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateProcessedUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateUpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsPermanentlyDeleted = table.Column<bool>(type: "bit", nullable: false),
                    WasCleaned = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AudioFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AudioFile_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AudioFile_UserId",
                table: "AudioFile",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AudioFile");
        }
    }
}
