using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Voicipher.DataAccess.Migrations
{
    public partial class Release02 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Storage",
                table: "TranscribeItem");

            migrationBuilder.DropColumn(
                name: "Storage",
                table: "AudioFile");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "TranscriptionEndTime",
                table: "AudioFile",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "TranscriptionStartTime",
                table: "AudioFile",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TranscriptionEndTime",
                table: "AudioFile");

            migrationBuilder.DropColumn(
                name: "TranscriptionStartTime",
                table: "AudioFile");

            migrationBuilder.AddColumn<int>(
                name: "Storage",
                table: "TranscribeItem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Storage",
                table: "AudioFile",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
