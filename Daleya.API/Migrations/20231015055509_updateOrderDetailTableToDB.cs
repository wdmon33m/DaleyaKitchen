using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Daleya.API.Migrations
{
    /// <inheritdoc />
    public partial class updateOrderDetailTableToDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrderItemId",
                table: "OrderDetails",
                newName: "OrderDetailId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrderDetailId",
                table: "OrderDetails",
                newName: "OrderItemId");
        }
    }
}
