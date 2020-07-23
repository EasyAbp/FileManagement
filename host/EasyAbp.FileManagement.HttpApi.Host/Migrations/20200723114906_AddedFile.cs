using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EasyAbp.FileManagement.HttpApi.Host.Migrations
{
    public partial class AddedFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EasyAbpFileManagementFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ExtraProperties = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorId = table.Column<Guid>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierId = table.Column<Guid>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    TenantId = table.Column<Guid>(nullable: true),
                    FileName = table.Column<string>(nullable: true),
                    FilePath = table.Column<string>(nullable: true),
                    MimeType = table.Column<string>(nullable: true),
                    FileType = table.Column<int>(nullable: false),
                    SubFilesQuantity = table.Column<int>(nullable: false),
                    ByteSize = table.Column<long>(nullable: false),
                    Hash = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    Level = table.Column<int>(nullable: false),
                    ParentId = table.Column<Guid>(nullable: true),
                    DisplayName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EasyAbpFileManagementFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EasyAbpFileManagementFiles_EasyAbpFileManagementFiles_ParentId",
                        column: x => x.ParentId,
                        principalTable: "EasyAbpFileManagementFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EasyAbpFileManagementFiles_ParentId",
                table: "EasyAbpFileManagementFiles",
                column: "ParentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EasyAbpFileManagementFiles");
        }
    }
}
