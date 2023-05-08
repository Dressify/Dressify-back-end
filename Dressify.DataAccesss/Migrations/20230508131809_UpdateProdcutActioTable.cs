using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dressify.Migrations
{
    public partial class UpdateProdcutActioTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Action",
                table: "ProdcutsActions",
                newName: "Reasson");

            migrationBuilder.AlterColumn<string>(
                name: "VendorId",
                table: "ProductsReports",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_ProductsReports_VendorId",
                table: "ProductsReports",
                column: "VendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductsReports_AspNetUsers_VendorId",
                table: "ProductsReports",
                column: "VendorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductsReports_AspNetUsers_VendorId",
                table: "ProductsReports");

            migrationBuilder.DropIndex(
                name: "IX_ProductsReports_VendorId",
                table: "ProductsReports");

            migrationBuilder.RenameColumn(
                name: "Reasson",
                table: "ProdcutsActions",
                newName: "Action");

            migrationBuilder.AlterColumn<string>(
                name: "VendorId",
                table: "ProductsReports",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
