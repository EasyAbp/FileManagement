using Microsoft.EntityFrameworkCore.Migrations;

namespace EasyAbp.FileManagement.Migrations
{
    public partial class RemovedFilePath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EasyAbpFileManagementFiles_FilePath_OwnerUserId_FileContainerName",
                table: "EasyAbpFileManagementFiles");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "EasyAbpFileManagementFiles");

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "EasyAbpFileManagementFiles",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EasyAbpFileManagementFiles_FileName_OwnerUserId_FileContainerName",
                table: "EasyAbpFileManagementFiles",
                columns: new[] { "FileName", "OwnerUserId", "FileContainerName" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EasyAbpFileManagementFiles_FileName_OwnerUserId_FileContainerName",
                table: "EasyAbpFileManagementFiles");

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "EasyAbpFileManagementFiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "EasyAbpFileManagementFiles",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EasyAbpFileManagementFiles_FilePath_OwnerUserId_FileContainerName",
                table: "EasyAbpFileManagementFiles",
                columns: new[] { "FilePath", "OwnerUserId", "FileContainerName" });
        }
    }
}
