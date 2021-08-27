using Microsoft.EntityFrameworkCore.Migrations;

namespace NZMSA_HYD.Migrations
{
    public partial class addFieldToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Github",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageURI",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Github",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ImageURI",
                table: "Users");
        }
    }
}
