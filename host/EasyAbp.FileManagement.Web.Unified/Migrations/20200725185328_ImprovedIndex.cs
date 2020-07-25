using Microsoft.EntityFrameworkCore.Migrations;

namespace EasyAbp.FileManagement.Migrations
{
    public partial class ImprovedIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EasyAbpFileManagementFiles_ParentId_FileType",
                table: "EasyAbpFileManagementFiles");

            migrationBuilder.AlterColumn<string>(
                name: "FileContainerName",
                table: "EasyAbpFileManagementFiles",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EasyAbpFileManagementFiles_ParentId_OwnerUserId_FileContainerName_FileType",
                table: "EasyAbpFileManagementFiles",
                columns: new[] { "ParentId", "OwnerUserId", "FileContainerName", "FileType" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EasyAbpFileManagementFiles_ParentId_OwnerUserId_FileContainerName_FileType",
                table: "EasyAbpFileManagementFiles");

            migrationBuilder.AlterColumn<string>(
                name: "FileContainerName",
                table: "EasyAbpFileManagementFiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EasyAbpFileManagementFiles_ParentId_FileType",
                table: "EasyAbpFileManagementFiles",
                columns: new[] { "ParentId", "FileType" });
        }
    }
}
