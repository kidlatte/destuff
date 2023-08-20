using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Destuff.Server.Data.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class AddStuffData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Locations_LocationId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Locations_ToLocationId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_LocationId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_ToLocationId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ToLocationId",
                table: "Events");

            migrationBuilder.AddColumn<int>(
                name: "PurchaseCount",
                table: "Suppliers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Data",
                table: "Stuffs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ItemCount",
                table: "Purchases",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "fe73948a-1173-43ad-9473-2f014b39f7c3",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEIycSbEY0jSEEH3VQYbCAapytpfZa8lyMkD022YXJX87MEtXeLfAVKyMGUKhmhWz+Q==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PurchaseCount",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "Data",
                table: "Stuffs");

            migrationBuilder.DropColumn(
                name: "ItemCount",
                table: "Purchases");

            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "Events",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ToLocationId",
                table: "Events",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "fe73948a-1173-43ad-9473-2f014b39f7c3",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEAQpkkYqy+N223OZhujjMuU95GK2ZtLcRm1Al4IitDNOjryj46MWf/ETkMD/PracZg==");

            migrationBuilder.CreateIndex(
                name: "IX_Events_LocationId",
                table: "Events",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_ToLocationId",
                table: "Events",
                column: "ToLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Locations_LocationId",
                table: "Events",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Locations_ToLocationId",
                table: "Events",
                column: "ToLocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
