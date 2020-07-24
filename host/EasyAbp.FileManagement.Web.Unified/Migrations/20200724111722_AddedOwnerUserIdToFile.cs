using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EasyAbp.FileManagement.Migrations
{
    public partial class AddedOwnerUserIdToFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Hash",
                table: "EasyAbpFileManagementFiles",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                table: "EasyAbpFileManagementFiles",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerUserId",
                table: "EasyAbpFileManagementFiles",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EasyAbpFileManagementFiles_FilePath",
                table: "EasyAbpFileManagementFiles",
                column: "FilePath");

            migrationBuilder.CreateIndex(
                name: "IX_EasyAbpFileManagementFiles_Hash",
                table: "EasyAbpFileManagementFiles",
                column: "Hash");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EasyAbpFileManagementFiles_FilePath",
                table: "EasyAbpFileManagementFiles");

            migrationBuilder.DropIndex(
                name: "IX_EasyAbpFileManagementFiles_Hash",
                table: "EasyAbpFileManagementFiles");

            migrationBuilder.DropColumn(
                name: "OwnerUserId",
                table: "EasyAbpFileManagementFiles");

            migrationBuilder.AlterColumn<string>(
                name: "Hash",
                table: "EasyAbpFileManagementFiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                table: "EasyAbpFileManagementFiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
