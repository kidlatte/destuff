using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Destuff.Server.Data.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class AddPurchaseItemData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "PurchaseItems");

            migrationBuilder.AddColumn<string>(
                name: "Data",
                table: "PurchaseItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "fe73948a-1173-43ad-9473-2f014b39f7c3",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEPSr3i5viUNO1vf2I8Zb13k/7mZeV3UmTknB8bPqtJEsgXdni5EtEmKq7TEaIcXIYw==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Data",
                table: "PurchaseItems");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "PurchaseItems",
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
    }
}
