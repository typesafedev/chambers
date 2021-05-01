using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Publish.Api.Migrations
{
    public partial class AddDocumentLocationSize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Documents",
                newName: "Location");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Documents",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "FileSize",
                table: "Documents",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileSize",
                table: "Documents");

            migrationBuilder.RenameColumn(
                name: "Location",
                table: "Documents",
                newName: "Description");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Documents",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT")
                .Annotation("Sqlite:Autoincrement", true);
        }
    }
}
