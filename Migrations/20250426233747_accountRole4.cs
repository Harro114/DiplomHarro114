using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diplom.Migrations
{
    /// <inheritdoc />
    public partial class accountRole4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AccountRole_RoleId",
                table: "AccountRole");

            migrationBuilder.AddColumn<bool>(
                name: "IsBlocked",
                table: "Accounts",
                type: "boolean",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserDiscountsHistory_AccountId",
                table: "UserDiscountsHistory",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDiscountsHistory_DiscountId",
                table: "UserDiscountsHistory",
                column: "DiscountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountRole_RoleId",
                table: "AccountRole",
                column: "RoleId",
                unique: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropIndex(
                name: "IX_AccountRole_RoleId",
                table: "AccountRole");

            migrationBuilder.DropColumn(
                name: "IsBlocked",
                table: "Accounts");

            migrationBuilder.CreateIndex(
                name: "IX_AccountRole_RoleId",
                table: "AccountRole",
                column: "RoleId");
        }
    }
}
