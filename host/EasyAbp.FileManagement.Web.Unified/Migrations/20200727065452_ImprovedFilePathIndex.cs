using Microsoft.EntityFrameworkCore.Migrations;

namespace EasyAbp.FileManagement.Migrations
{
    public partial class ImprovedFilePathIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EasyAbpFileManagementFiles_FilePath",
                table: "EasyAbpFileManagementFiles");

            migrationBuilder.CreateIndex(
                name: "IX_EasyAbpFileManagementFiles_FilePath_OwnerUserId_FileContainerName",
                table: "EasyAbpFileManagementFiles",
                columns: new[] { "FilePath", "OwnerUserId", "FileContainerName" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EasyAbpFileManagementFiles_FilePath_OwnerUserId_FileContainerName",
                table: "EasyAbpFileManagementFiles");

            migrationBuilder.CreateIndex(
                name: "IX_EasyAbpFileManagementFiles_FilePath",
                table: "EasyAbpFileManagementFiles",
                column: "FilePath");
        }
    }
}
