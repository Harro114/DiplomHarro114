using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diplom.Migrations
{
    /// <inheritdoc />
    public partial class discountsV5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Discounts_Discounts_DiscountOne",
                table: "Discounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Discounts_Discounts_DiscountTwo",
                table: "Discounts");

            migrationBuilder.DropIndex(
                name: "IX_Discounts_DiscountOne",
                table: "Discounts");

            migrationBuilder.DropIndex(
                name: "IX_Discounts_DiscountTwo",
                table: "Discounts");

            migrationBuilder.DropColumn(
                name: "DiscountOne",
                table: "Discounts");

            migrationBuilder.DropColumn(
                name: "DiscountTwo",
                table: "Discounts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiscountOne",
                table: "Discounts",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DiscountTwo",
                table: "Discounts",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_DiscountOne",
                table: "Discounts",
                column: "DiscountOne");

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_DiscountTwo",
                table: "Discounts",
                column: "DiscountTwo");

            migrationBuilder.AddForeignKey(
                name: "FK_Discounts_Discounts_DiscountOne",
                table: "Discounts",
                column: "DiscountOne",
                principalTable: "Discounts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Discounts_Discounts_DiscountTwo",
                table: "Discounts",
                column: "DiscountTwo",
                principalTable: "Discounts",
                principalColumn: "Id");
        }
    }
}
