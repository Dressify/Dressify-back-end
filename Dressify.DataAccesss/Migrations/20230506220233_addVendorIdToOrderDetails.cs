using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dressify.Migrations
{
    public partial class addVendorIdToOrderDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VendorId",
                table: "OrdersDetails",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_OrdersDetails_VendorId",
                table: "OrdersDetails",
                column: "VendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdersDetails_AspNetUsers_VendorId",
                table: "OrdersDetails",
                column: "VendorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdersDetails_AspNetUsers_VendorId",
                table: "OrdersDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrdersDetails_VendorId",
                table: "OrdersDetails");

            migrationBuilder.DropColumn(
                name: "VendorId",
                table: "OrdersDetails");
        }
    }
}
