using Microsoft.EntityFrameworkCore.Migrations;

namespace EasyAbp.FileManagement.Migrations
{
    public partial class ImprovedParentIdIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EasyAbpFileManagementFiles_ParentId",
                table: "EasyAbpFileManagementFiles");

            migrationBuilder.CreateIndex(
                name: "IX_EasyAbpFileManagementFiles_ParentId_FileType",
                table: "EasyAbpFileManagementFiles",
                columns: new[] { "ParentId", "FileType" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EasyAbpFileManagementFiles_ParentId_FileType",
                table: "EasyAbpFileManagementFiles");

            migrationBuilder.CreateIndex(
                name: "IX_EasyAbpFileManagementFiles_ParentId",
                table: "EasyAbpFileManagementFiles",
                column: "ParentId");
        }
    }
}
