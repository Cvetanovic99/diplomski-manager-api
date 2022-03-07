using Microsoft.EntityFrameworkCore.Migrations;

namespace Manager.Infrastructure.Persistence.Migrations
{
    public partial class NameAddedToPdfFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "PdfFiles",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "PdfFiles");
        }
    }
}
