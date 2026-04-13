using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClercSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class cratedAtalteredtocreatedOnlogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "DocumentLogs",
                newName: "CreatedOn");

            migrationBuilder.AlterColumn<string>(
                name: "Desription",
                table: "DocumentLogs",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "DocumentLogs",
                newName: "CreatedAt");

            migrationBuilder.AlterColumn<string>(
                name: "Desription",
                table: "DocumentLogs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300);
        }
    }
}
