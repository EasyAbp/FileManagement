using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyAbp.FileManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddedSoftDeletionTokenProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EasyAbpFileManagementFiles_FileName_ParentId_OwnerUserId_FileContainerName",
                table: "EasyAbpFileManagementFiles");

            migrationBuilder.DropIndex(
                name: "IX_EasyAbpFileManagementFiles_ParentId_OwnerUserId_FileContainerName",
                table: "EasyAbpFileManagementFiles");

            migrationBuilder.AddColumn<string>(
                name: "SoftDeletionToken",
                table: "EasyAbpFileManagementFiles",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EasyAbpFileManagementFiles_FileName_ParentId_OwnerUserId_FileContainerName_TenantId_SoftDeletionToken",
                table: "EasyAbpFileManagementFiles",
                columns: new[] { "FileName", "ParentId", "OwnerUserId", "FileContainerName", "TenantId", "SoftDeletionToken" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EasyAbpFileManagementFiles_ParentId_OwnerUserId_FileContainerName_FileName",
                table: "EasyAbpFileManagementFiles",
                columns: new[] { "ParentId", "OwnerUserId", "FileContainerName", "FileName" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EasyAbpFileManagementFiles_FileName_ParentId_OwnerUserId_FileContainerName_TenantId_SoftDeletionToken",
                table: "EasyAbpFileManagementFiles");

            migrationBuilder.DropIndex(
                name: "IX_EasyAbpFileManagementFiles_ParentId_OwnerUserId_FileContainerName_FileName",
                table: "EasyAbpFileManagementFiles");

            migrationBuilder.DropColumn(
                name: "SoftDeletionToken",
                table: "EasyAbpFileManagementFiles");

            migrationBuilder.CreateIndex(
                name: "IX_EasyAbpFileManagementFiles_FileName_ParentId_OwnerUserId_FileContainerName",
                table: "EasyAbpFileManagementFiles",
                columns: new[] { "FileName", "ParentId", "OwnerUserId", "FileContainerName" },
                unique: true,
                filter: "[FileName] IS NOT NULL AND [ParentId] IS NOT NULL AND [OwnerUserId] IS NOT NULL AND [FileContainerName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EasyAbpFileManagementFiles_ParentId_OwnerUserId_FileContainerName",
                table: "EasyAbpFileManagementFiles",
                columns: new[] { "ParentId", "OwnerUserId", "FileContainerName" });
        }
    }
}
