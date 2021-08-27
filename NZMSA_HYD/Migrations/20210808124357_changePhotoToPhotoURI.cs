using Microsoft.EntityFrameworkCore.Migrations;

namespace NZMSA_HYD.Migrations
{
    public partial class changePhotoToPhotoURI : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Photo",
                table: "Events",
                newName: "PhotoURI");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhotoURI",
                table: "Events",
                newName: "Photo");
        }
    }
}
