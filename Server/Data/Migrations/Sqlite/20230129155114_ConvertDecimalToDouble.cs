using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Destuff.Server.Data.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class ConvertDecimalToDouble : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "Purchases",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "PurchaseItems",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "fe73948a-1173-43ad-9473-2f014b39f7c3",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "4ed21d28-faa4-4948-ae7a-e945cb3b75a9", "AQAAAAIAAYagAAAAEPp7AkyEBI5y/qEKLAyQCQ4S30Yc1ohXI983K72zNgS8Cm7VNUMkKna3wJso27DjTg==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Purchases",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "PurchaseItems",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "fe73948a-1173-43ad-9473-2f014b39f7c3",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "e0537b1a-88ff-4ec0-8de3-fc21cc8d32f5", "AQAAAAIAAYagAAAAEEcVUl9NJ4iCyzzY0xZePBtqY1M5GNk/52ulSGgEcn1yyE8TqJP7hTCZMPAFq6+7XQ==" });
        }
    }
}
