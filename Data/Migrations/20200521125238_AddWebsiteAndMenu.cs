using Microsoft.EntityFrameworkCore.Migrations;

namespace wheredoyouwanttoeat2.Data.Migrations
{
    public partial class AddWebsiteAndMenu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropColumn(
            //     name: "Discriminator",
            //     table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "Menu",
                table: "Restaurants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Restaurants",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Menu",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "Restaurants");

            // migrationBuilder.AddColumn<string>(
            //     name: "Discriminator",
            //     table: "AspNetUsers",
            //     type: "TEXT",
            //     nullable: false,
            //     defaultValue: "");
        }
    }
}
