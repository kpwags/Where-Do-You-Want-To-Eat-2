using Microsoft.EntityFrameworkCore.Migrations;

namespace WhereDoYouWantToEat2.Data.Migrations
{
    public partial class AddPhoneNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropColumn(
            //     name: "Discriminator",
            //     table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Restaurants",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumber",
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
