using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClercSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class is_deleted_property_added_todocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Documents",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Documents");
        }
    }
}
