using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Destuff.Server.Data.Migrations.Sqlite
{
    public partial class AddSlug : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Stuffs",
                type: "TEXT",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Locations",
                type: "TEXT",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Slug",
                table: "Tags",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stuffs_Slug",
                table: "Stuffs",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Locations_Slug",
                table: "Locations",
                column: "Slug",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tags_Slug",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Stuffs_Slug",
                table: "Stuffs");

            migrationBuilder.DropIndex(
                name: "IX_Locations_Slug",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Stuffs");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Locations");
        }
    }
}
