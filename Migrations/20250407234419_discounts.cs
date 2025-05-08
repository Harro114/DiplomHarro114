using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Diplom.Migrations
{
    /// <inheritdoc />
    public partial class discounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discription",
                table: "ExpChanges",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "CategoriesStore",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    isActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriesStore", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductsStore",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    isActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductsStore", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Discounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    isActive = table.Column<bool>(type: "boolean", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProductsStoreId = table.Column<int>(type: "integer", nullable: false),
                    CategoriesStoreId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Discounts_CategoriesStore_CategoriesStoreId",
                        column: x => x.CategoriesStoreId,
                        principalTable: "CategoriesStore",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Discounts_ProductsStore_ProductsStoreId",
                        column: x => x.ProductsStoreId,
                        principalTable: "ProductsStore",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CategoriesStoreDiscounts",
                columns: table => new
                {
                    CategoriesIdId = table.Column<int>(type: "integer", nullable: false),
                    DiscountsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriesStoreDiscounts", x => new { x.CategoriesIdId, x.DiscountsId });
                    table.ForeignKey(
                        name: "FK_CategoriesStoreDiscounts_CategoriesStore_CategoriesIdId",
                        column: x => x.CategoriesIdId,
                        principalTable: "CategoriesStore",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoriesStoreDiscounts_Discounts_DiscountsId",
                        column: x => x.DiscountsId,
                        principalTable: "Discounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiscountsProductsStore",
                columns: table => new
                {
                    DiscountsId = table.Column<int>(type: "integer", nullable: false),
                    ProductsIdId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountsProductsStore", x => new { x.DiscountsId, x.ProductsIdId });
                    table.ForeignKey(
                        name: "FK_DiscountsProductsStore_Discounts_DiscountsId",
                        column: x => x.DiscountsId,
                        principalTable: "Discounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiscountsProductsStore_ProductsStore_ProductsIdId",
                        column: x => x.ProductsIdId,
                        principalTable: "ProductsStore",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoriesStoreDiscounts_DiscountsId",
                table: "CategoriesStoreDiscounts",
                column: "DiscountsId");

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_CategoriesStoreId",
                table: "Discounts",
                column: "CategoriesStoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_ProductsStoreId",
                table: "Discounts",
                column: "ProductsStoreId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountsProductsStore_ProductsIdId",
                table: "DiscountsProductsStore",
                column: "ProductsIdId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoriesStoreDiscounts");

            migrationBuilder.DropTable(
                name: "DiscountsProductsStore");

            migrationBuilder.DropTable(
                name: "Discounts");

            migrationBuilder.DropTable(
                name: "CategoriesStore");

            migrationBuilder.DropTable(
                name: "ProductsStore");

            migrationBuilder.DropColumn(
                name: "Discription",
                table: "ExpChanges");
        }
    }
}
