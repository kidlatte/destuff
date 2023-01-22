using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Destuff.Server.Data.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class AddSupplierSLug : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Tags",
                type: "TEXT",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Suppliers",
                type: "TEXT",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Suppliers",
                type: "TEXT",
                maxLength: 1023,
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_Slug",
                table: "Suppliers",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Suppliers_Slug",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "Suppliers");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Tags",
                type: "TEXT",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 255);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "fe73948a-1173-43ad-9473-2f014b39f7c3",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "6b8af8b0-0154-4702-9f8c-c120d1210f78", "AQAAAAEAACcQAAAAEFFRaQu/TSynC199ay8D8JaJtv24bErrHvWa8etrmriicx6FhxmsIn4LLUN6SScvaw==" });

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Updated" },
                values: new object[] { new DateTime(2022, 10, 1, 6, 35, 44, 312, DateTimeKind.Utc).AddTicks(1833), new DateTime(2022, 10, 1, 6, 35, 44, 312, DateTimeKind.Utc).AddTicks(1838) });
        }
    }
}
