using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diplom.Migrations
{
    /// <inheritdoc />
    public partial class accountRole3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ExchangeDiscounts_DiscountId",
                table: "ExchangeDiscounts");

            migrationBuilder.DropIndex(
                name: "IX_AccountPasswords_AccountId",
                table: "AccountPasswords");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeDiscounts_DiscountId",
                table: "ExchangeDiscounts",
                column: "DiscountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountPasswords_AccountId",
                table: "AccountPasswords",
                column: "AccountId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ExchangeDiscounts_DiscountId",
                table: "ExchangeDiscounts");

            migrationBuilder.DropIndex(
                name: "IX_AccountPasswords_AccountId",
                table: "AccountPasswords");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeDiscounts_DiscountId",
                table: "ExchangeDiscounts",
                column: "DiscountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountPasswords_AccountId",
                table: "AccountPasswords",
                column: "AccountId");
        }
    }
}
