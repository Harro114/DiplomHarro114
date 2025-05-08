using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diplom.Migrations
{
    /// <inheritdoc />
    public partial class discountsDiscription4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDiscountsHistory_Accounts_AccountId",
                table: "UserDiscountsHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_UserDiscountsHistory_Discounts_DiscountId",
                table: "UserDiscountsHistory");

            migrationBuilder.DropIndex(
                name: "IX_UserDiscountsHistory_AccountId",
                table: "UserDiscountsHistory");

            migrationBuilder.DropIndex(
                name: "IX_UserDiscountsHistory_DiscountId",
                table: "UserDiscountsHistory");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserDiscountsHistory_AccountId",
                table: "UserDiscountsHistory",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDiscountsHistory_DiscountId",
                table: "UserDiscountsHistory",
                column: "DiscountId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDiscountsHistory_Accounts_AccountId",
                table: "UserDiscountsHistory",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserDiscountsHistory_Discounts_DiscountId",
                table: "UserDiscountsHistory",
                column: "DiscountId",
                principalTable: "Discounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
