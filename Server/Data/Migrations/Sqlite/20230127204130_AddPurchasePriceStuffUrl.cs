using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Destuff.Server.Data.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class AddPurchasePriceStuffUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Cost",
                table: "PurchaseItems",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "PathData",
                table: "Locations",
                newName: "Data");

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Stuffs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Purchases",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "fe73948a-1173-43ad-9473-2f014b39f7c3",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "e0537b1a-88ff-4ec0-8de3-fc21cc8d32f5", "AQAAAAIAAYagAAAAEEcVUl9NJ4iCyzzY0xZePBtqY1M5GNk/52ulSGgEcn1yyE8TqJP7hTCZMPAFq6+7XQ==" });

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Updated" },
                values: new object[] { new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Url",
                table: "Stuffs");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Purchases");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "PurchaseItems",
                newName: "Cost");

            migrationBuilder.RenameColumn(
                name: "Data",
                table: "Locations",
                newName: "PathData");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "fe73948a-1173-43ad-9473-2f014b39f7c3",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "6a5fbdf3-62c3-49ec-93a8-a28299143644", "AQAAAAIAAYagAAAAEG7lCEmSniYxOc0X1Z9Wfui7se3g/UhbACOlrXGD6tnPj92NVgg8uk0DZU17XpZ32Q==" });

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Updated" },
                values: new object[] { new DateTime(2023, 1, 22, 20, 42, 59, 318, DateTimeKind.Utc).AddTicks(9819), new DateTime(2023, 1, 22, 20, 42, 59, 318, DateTimeKind.Utc).AddTicks(9821) });
        }
    }
}
