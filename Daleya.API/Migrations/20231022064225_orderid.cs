using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Daleya.API.Migrations
{
    /// <inheritdoc />
    public partial class orderid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6b55fec2-a739-4959-b254-c134a52815e2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b2b9d916-0227-459d-bdab-513d8d465d52");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "6b55fec2-a739-4959-b254-c134a52815e2", null, "ADMIN", null },
                    { "b2b9d916-0227-459d-bdab-513d8d465d52", null, "CUSTOMER", null }
                });
        }
    }
}
