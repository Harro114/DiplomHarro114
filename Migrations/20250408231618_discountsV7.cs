using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diplom.Migrations
{
    /// <inheritdoc />
    public partial class discountsV7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Discounts_CategoriesStore_CategoriesStoreId",
                table: "Discounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Discounts_ProductsStore_ProductsStoreId",
                table: "Discounts");

            migrationBuilder.AlterColumn<int>(
                name: "ProductsStoreId",
                table: "Discounts",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "CategoriesStoreId",
                table: "Discounts",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Discounts_CategoriesStore_CategoriesStoreId",
                table: "Discounts",
                column: "CategoriesStoreId",
                principalTable: "CategoriesStore",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Discounts_ProductsStore_ProductsStoreId",
                table: "Discounts",
                column: "ProductsStoreId",
                principalTable: "ProductsStore",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Discounts_CategoriesStore_CategoriesStoreId",
                table: "Discounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Discounts_ProductsStore_ProductsStoreId",
                table: "Discounts");

            migrationBuilder.AlterColumn<int>(
                name: "ProductsStoreId",
                table: "Discounts",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CategoriesStoreId",
                table: "Discounts",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Discounts_CategoriesStore_CategoriesStoreId",
                table: "Discounts",
                column: "CategoriesStoreId",
                principalTable: "CategoriesStore",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Discounts_ProductsStore_ProductsStoreId",
                table: "Discounts",
                column: "ProductsStoreId",
                principalTable: "ProductsStore",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
