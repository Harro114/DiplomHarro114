using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diplom.Migrations
{
    /// <inheritdoc />
    public partial class discountsV4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountExchangeOne",
                table: "ExchangeDiscounts");

            migrationBuilder.DropColumn(
                name: "DiscountExchangeTwo",
                table: "ExchangeDiscounts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiscountExchangeOne",
                table: "ExchangeDiscounts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DiscountExchangeTwo",
                table: "ExchangeDiscounts",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
