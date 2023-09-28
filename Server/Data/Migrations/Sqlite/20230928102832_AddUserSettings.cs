using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Destuff.Server.Data.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class AddUserSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.AddColumn<string>(
                name: "Settings",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "fe73948a-1173-43ad-9473-2f014b39f7c3",
                columns: new[] { "PasswordHash", "Settings" },
                values: new object[] { "AQAAAAIAAYagAAAAEBcW/+OL+mrwoJGWOSDNFUPUBra7TkNkLYwG9eghzQFRObqj9gv6eIJyQ7FBCcIJ3g==", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Settings",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Key);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "fe73948a-1173-43ad-9473-2f014b39f7c3",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEPSr3i5viUNO1vf2I8Zb13k/7mZeV3UmTknB8bPqtJEsgXdni5EtEmKq7TEaIcXIYw==");
        }
    }
}
