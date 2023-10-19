using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Daleya.API.Migrations
{
    /// <inheritdoc />
    public partial class updateUserAndRoleTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "53a5ac05-330b-4c51-b4a7-ea07ddc316a7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f8b1411f-7f2f-4359-a5a9-f62b9657fc45");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "e490ff26-67ad-4cc6-9f25-7e60ec590722", null, "ADMIN", "ADMIN" },
                    { "f0a9b9b9-8473-42e3-b08b-f859069f0910", null, "ADMIN", null },
                    { "f93e330b-b24a-4b0e-b735-10c5f5004be2", null, "CUSTOMER", null },
                    { "fe348fe6-0aab-4356-a784-57062821f697", null, "CUSTOMER", "CUSTOMER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e490ff26-67ad-4cc6-9f25-7e60ec590722");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f0a9b9b9-8473-42e3-b08b-f859069f0910");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f93e330b-b24a-4b0e-b735-10c5f5004be2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fe348fe6-0aab-4356-a784-57062821f697");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "53a5ac05-330b-4c51-b4a7-ea07ddc316a7", null, "ADMIN", null },
                    { "f8b1411f-7f2f-4359-a5a9-f62b9657fc45", null, "CUSTOMER", null }
                });
        }
    }
}
