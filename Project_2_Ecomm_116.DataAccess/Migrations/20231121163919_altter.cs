using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project_2_Ecomm_116.DataAccess.Migrations
{
    public partial class altter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StreetAddress",
                table: "AspNetUsers",
                newName: "StreetAdress");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StreetAdress",
                table: "AspNetUsers",
                newName: "StreetAddress");
        }
    }
}
