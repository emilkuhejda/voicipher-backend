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
                name: "TranscribedEndTime",
                table: "AudioFile",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "TranscribedStartTime",
                table: "AudioFile",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TranscribedEndTime",
                table: "AudioFile");

            migrationBuilder.DropColumn(
                name: "TranscribedStartTime",
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
