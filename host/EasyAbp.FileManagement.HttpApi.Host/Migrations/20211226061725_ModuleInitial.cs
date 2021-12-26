using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EasyAbp.FileManagement.Migrations
{
    public partial class ModuleInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EasyAbpFileManagementFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FileContainerName = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    MimeType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileType = table.Column<int>(type: "int", nullable: false),
                    SubFilesQuantity = table.Column<int>(type: "int", nullable: false),
                    ByteSize = table.Column<long>(type: "bigint", nullable: false),
                    Hash = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    BlobName = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    OwnerUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Flag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EasyAbpFileManagementFiles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EasyAbpFileManagementFiles_BlobName",
                table: "EasyAbpFileManagementFiles",
                column: "BlobName");

            migrationBuilder.CreateIndex(
                name: "IX_EasyAbpFileManagementFiles_FileName_OwnerUserId_FileContainerName",
                table: "EasyAbpFileManagementFiles",
                columns: new[] { "FileName", "OwnerUserId", "FileContainerName" });

            migrationBuilder.CreateIndex(
                name: "IX_EasyAbpFileManagementFiles_Hash",
                table: "EasyAbpFileManagementFiles",
                column: "Hash");

            migrationBuilder.CreateIndex(
                name: "IX_EasyAbpFileManagementFiles_ParentId_OwnerUserId_FileContainerName_FileType",
                table: "EasyAbpFileManagementFiles",
                columns: new[] { "ParentId", "OwnerUserId", "FileContainerName", "FileType" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EasyAbpFileManagementFiles");
        }
    }
}
