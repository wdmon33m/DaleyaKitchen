using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Daleya.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCoupon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CouponCode",
                table: "OrderHeaders");

            migrationBuilder.RenameColumn(
                name: "CouponID",
                table: "Coupons",
                newName: "CouponId");

            migrationBuilder.AddColumn<int>(
                name: "CouponId",
                table: "OrderHeaders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OrderHeaders_CouponId",
                table: "OrderHeaders",
                column: "CouponId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderHeaders_Coupons_CouponId",
                table: "OrderHeaders",
                column: "CouponId",
                principalTable: "Coupons",
                principalColumn: "CouponId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderHeaders_Coupons_CouponId",
                table: "OrderHeaders");

            migrationBuilder.DropIndex(
                name: "IX_OrderHeaders_CouponId",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "CouponId",
                table: "OrderHeaders");

            migrationBuilder.RenameColumn(
                name: "CouponId",
                table: "Coupons",
                newName: "CouponID");

            migrationBuilder.AddColumn<string>(
                name: "CouponCode",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
