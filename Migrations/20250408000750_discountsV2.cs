using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Diplom.Migrations
{
    /// <inheritdoc />
    public partial class discountsV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Amount",
                table: "Discounts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "UserDiscounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AccountId = table.Column<int>(type: "integer", nullable: false),
                    DiscountId = table.Column<int>(type: "integer", nullable: false),
                    DateAccruals = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDiscounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDiscounts_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserDiscounts_Discounts_DiscountId",
                        column: x => x.DiscountId,
                        principalTable: "Discounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserDiscountsActivated",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AccountId = table.Column<int>(type: "integer", nullable: false),
                    DiscountId = table.Column<int>(type: "integer", nullable: false),
                    DateActivateDiscount = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDiscountsActivated", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDiscountsActivated_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserDiscountsActivated_Discounts_DiscountId",
                        column: x => x.DiscountId,
                        principalTable: "Discounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserDiscountsActivatedHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AccountId = table.Column<int>(type: "integer", nullable: false),
                    DiscountId = table.Column<int>(type: "integer", nullable: false),
                    DateActivateDiscount = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateDelete = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDiscountsActivatedHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDiscountsActivatedHistory_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserDiscountsActivatedHistory_Discounts_DiscountId",
                        column: x => x.DiscountId,
                        principalTable: "Discounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserDiscountsHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AccountId = table.Column<int>(type: "integer", nullable: false),
                    DiscountId = table.Column<int>(type: "integer", nullable: false),
                    DateAccruals = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateDelete = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDiscountsHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDiscountsHistory_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserDiscountsHistory_Discounts_DiscountId",
                        column: x => x.DiscountId,
                        principalTable: "Discounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserDiscounts_AccountId",
                table: "UserDiscounts",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDiscounts_DiscountId",
                table: "UserDiscounts",
                column: "DiscountId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDiscountsActivated_AccountId",
                table: "UserDiscountsActivated",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDiscountsActivated_DiscountId",
                table: "UserDiscountsActivated",
                column: "DiscountId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDiscountsActivatedHistory_AccountId",
                table: "UserDiscountsActivatedHistory",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDiscountsActivatedHistory_DiscountId",
                table: "UserDiscountsActivatedHistory",
                column: "DiscountId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDiscountsHistory_AccountId",
                table: "UserDiscountsHistory",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDiscountsHistory_DiscountId",
                table: "UserDiscountsHistory",
                column: "DiscountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserDiscounts");

            migrationBuilder.DropTable(
                name: "UserDiscountsActivated");

            migrationBuilder.DropTable(
                name: "UserDiscountsActivatedHistory");

            migrationBuilder.DropTable(
                name: "UserDiscountsHistory");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Discounts");
        }
    }
}
