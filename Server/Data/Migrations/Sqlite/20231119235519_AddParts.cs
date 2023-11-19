using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Destuff.Server.Data.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class AddParts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "Stuffs",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "fe73948a-1173-43ad-9473-2f014b39f7c3",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEERPH9btZ6P0nMBVmme6sGkJYLSCWmpScnPLHgsETGj7NmYgDnQNyNwNE0N0C+5MGw==");

            migrationBuilder.CreateIndex(
                name: "IX_Stuffs_ParentId",
                table: "Stuffs",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stuffs_Stuffs_ParentId",
                table: "Stuffs",
                column: "ParentId",
                principalTable: "Stuffs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stuffs_Stuffs_ParentId",
                table: "Stuffs");

            migrationBuilder.DropIndex(
                name: "IX_Stuffs_ParentId",
                table: "Stuffs");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Stuffs");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "fe73948a-1173-43ad-9473-2f014b39f7c3",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEBcW/+OL+mrwoJGWOSDNFUPUBra7TkNkLYwG9eghzQFRObqj9gv6eIJyQ7FBCcIJ3g==");
        }
    }
}
