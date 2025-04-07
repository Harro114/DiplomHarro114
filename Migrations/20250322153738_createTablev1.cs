using System;
using Diplom.Models;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Diplom.Migrations
{
    /// <inheritdoc />
    public partial class createTablev1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    UserLastName = table.Column<string>(type: "text", nullable: false),
                    UserFirstName = table.Column<string>(type: "text", nullable: false),
                    Sex = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExpUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AccountId = table.Column<int>(type: "integer", nullable: false),
                    ExpValue = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpUsers_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExpChanges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AccountId = table.Column<int>(type: "integer", nullable: false),
                    ExpUserId = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<int>(type: "integer", nullable: false),
                    CurrentBalance = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpChanges_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExpChanges_ExpUsers_ExpUserId",
                        column: x => x.ExpUserId,
                        principalTable: "ExpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpChanges_AccountId",
                table: "ExpChanges",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpChanges_ExpUserId",
                table: "ExpChanges",
                column: "ExpUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpUsers_AccountId",
                table: "ExpUsers",
                column: "AccountId",
                unique: true);

            migrationBuilder.InsertData(
                table: "Config",
                columns: new[] { "Name", "ValueFloat"},
                values: new object[] {"rublesToExp", 1f}
                
            );
            
            migrationBuilder.InsertData(
                table: "Config",
                columns: new[] { "Name" },
                values: new object[] { "lastDateOrder" }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpChanges");

            migrationBuilder.DropTable(
                name: "ExpUsers");

            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
