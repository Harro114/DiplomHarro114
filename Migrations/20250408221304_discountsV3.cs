using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diplom.Migrations
{
    /// <inheritdoc />
    public partial class discountsV3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExchangeDiscounts",
                columns: table => new
                {
                    DiscountId = table.Column<int>(type: "integer", nullable: false),
                    DiscountExchangeOneId = table.Column<int>(type: "integer", nullable: false),
                    DiscountExchangeTwoId = table.Column<int>(type: "integer", nullable: false),
                    DiscountExchangeOne = table.Column<int>(type: "integer", nullable: false),
                    DiscountExchangeTwo = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_ExchangeDiscounts_Discounts_DiscountExchangeOneId",
                        column: x => x.DiscountExchangeOneId,
                        principalTable: "Discounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExchangeDiscounts_Discounts_DiscountExchangeTwoId",
                        column: x => x.DiscountExchangeTwoId,
                        principalTable: "Discounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExchangeDiscounts_Discounts_DiscountId",
                        column: x => x.DiscountId,
                        principalTable: "Discounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeDiscounts_DiscountExchangeOneId",
                table: "ExchangeDiscounts",
                column: "DiscountExchangeOneId");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeDiscounts_DiscountExchangeTwoId",
                table: "ExchangeDiscounts",
                column: "DiscountExchangeTwoId");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeDiscounts_DiscountId",
                table: "ExchangeDiscounts",
                column: "DiscountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExchangeDiscounts");
        }
    }
}
