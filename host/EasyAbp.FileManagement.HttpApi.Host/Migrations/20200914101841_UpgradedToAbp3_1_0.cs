using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EasyAbp.FileManagement.HttpApi.Host.Migrations
{
    public partial class UpgradedToAbp3_1_0 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EasyAbpFileManagementFiles_EasyAbpFileManagementFiles_ParentId",
                table: "EasyAbpFileManagementFiles");

            migrationBuilder.DropIndex(
                name: "IX_EasyAbpFileManagementFiles_ParentId",
                table: "EasyAbpFileManagementFiles");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "EasyAbpFileManagementFiles");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "EasyAbpFileManagementFiles");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "EasyAbpFileManagementFiles");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "EasyAbpFileManagementFiles");

            migrationBuilder.AlterColumn<string>(
                name: "Hash",
                table: "EasyAbpFileManagementFiles",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "EasyAbpFileManagementFiles",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BlobName",
                table: "EasyAbpFileManagementFiles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileContainerName",
                table: "EasyAbpFileManagementFiles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Flag",
                table: "EasyAbpFileManagementFiles",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerUserId",
                table: "EasyAbpFileManagementFiles",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EasyAbpFileManagementFiles_BlobName",
                table: "EasyAbpFileManagementFiles",
                column: "BlobName");

            migrationBuilder.CreateIndex(
                name: "IX_EasyAbpFileManagementFiles_Hash",
                table: "EasyAbpFileManagementFiles",
                column: "Hash");

            migrationBuilder.CreateIndex(
                name: "IX_EasyAbpFileManagementFiles_FileName_OwnerUserId_FileContainerName",
                table: "EasyAbpFileManagementFiles",
                columns: new[] { "FileName", "OwnerUserId", "FileContainerName" });

            migrationBuilder.CreateIndex(
                name: "IX_EasyAbpFileManagementFiles_ParentId_OwnerUserId_FileContainerName_FileType",
                table: "EasyAbpFileManagementFiles",
                columns: new[] { "ParentId", "OwnerUserId", "FileContainerName", "FileType" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EasyAbpFileManagementFiles_BlobName",
                table: "EasyAbpFileManagementFiles");

            migrationBuilder.DropIndex(
                name: "IX_EasyAbpFileManagementFiles_Hash",
                table: "EasyAbpFileManagementFiles");

            migrationBuilder.DropIndex(
                name: "IX_EasyAbpFileManagementFiles_FileName_OwnerUserId_FileContainerName",
                table: "EasyAbpFileManagementFiles");

            migrationBuilder.DropIndex(
                name: "IX_EasyAbpFileManagementFiles_ParentId_OwnerUserId_FileContainerName_FileType",
                table: "EasyAbpFileManagementFiles");

            migrationBuilder.DropColumn(
                name: "BlobName",
                table: "EasyAbpFileManagementFiles");

            migrationBuilder.DropColumn(
                name: "FileContainerName",
                table: "EasyAbpFileManagementFiles");

            migrationBuilder.DropColumn(
                name: "Flag",
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
                name: "FileName",
                table: "EasyAbpFileManagementFiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "EasyAbpFileManagementFiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "EasyAbpFileManagementFiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "EasyAbpFileManagementFiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "EasyAbpFileManagementFiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_EasyAbpFileManagementFiles_ParentId",
                table: "EasyAbpFileManagementFiles",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_EasyAbpFileManagementFiles_EasyAbpFileManagementFiles_ParentId",
                table: "EasyAbpFileManagementFiles",
                column: "ParentId",
                principalTable: "EasyAbpFileManagementFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
