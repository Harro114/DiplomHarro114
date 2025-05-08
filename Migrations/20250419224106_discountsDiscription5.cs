using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diplom.Migrations
{
    /// <inheritdoc />
    public partial class discountsDiscription5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DicountSize",
                table: "Discounts",
                newName: "DiscountSize");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserDiscountsHistory",
                table: "UserDiscountsHistory",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserDiscountsHistory",
                table: "UserDiscountsHistory");

            migrationBuilder.RenameColumn(
                name: "DiscountSize",
                table: "Discounts",
                newName: "DicountSize");
        }
    }
}
