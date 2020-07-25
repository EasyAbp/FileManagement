using Microsoft.EntityFrameworkCore.Migrations;

namespace EasyAbp.FileManagement.Migrations
{
    public partial class RemovedTrees : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EasyAbpFileManagementFiles_EasyAbpFileManagementFiles_ParentId",
                table: "EasyAbpFileManagementFiles");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "EasyAbpFileManagementFiles");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "EasyAbpFileManagementFiles");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "EasyAbpFileManagementFiles");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "EasyAbpFileManagementFiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

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
