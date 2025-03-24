using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyAbp.FileManagement.Blazor.Server.Host.Migrations
{
    /// <inheritdoc />
    public partial class Added_File_ParentId_Index : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_EasyAbpFileManagementFiles_ParentId",
                table: "EasyAbpFileManagementFiles",
                column: "ParentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EasyAbpFileManagementFiles_ParentId",
                table: "EasyAbpFileManagementFiles");
        }
    }
}
