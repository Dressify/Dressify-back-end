using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dressify.Migrations
{
    public partial class VendorIdNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductsQuestions_AspNetUsers_VendorId",
                table: "ProductsQuestions");

            migrationBuilder.AlterColumn<string>(
                name: "VendorId",
                table: "ProductsQuestions",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Answear",
                table: "ProductsQuestions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductsQuestions_AspNetUsers_VendorId",
                table: "ProductsQuestions",
                column: "VendorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductsQuestions_AspNetUsers_VendorId",
                table: "ProductsQuestions");

            migrationBuilder.AlterColumn<string>(
                name: "VendorId",
                table: "ProductsQuestions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Answear",
                table: "ProductsQuestions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductsQuestions_AspNetUsers_VendorId",
                table: "ProductsQuestions",
                column: "VendorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
