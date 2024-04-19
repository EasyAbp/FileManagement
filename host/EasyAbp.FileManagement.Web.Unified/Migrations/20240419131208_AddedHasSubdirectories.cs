using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyAbp.FileManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddedHasSubdirectories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasSubdirectories",
                table: "EasyAbpFileManagementFiles",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasSubdirectories",
                table: "EasyAbpFileManagementFiles");
        }
    }
}
