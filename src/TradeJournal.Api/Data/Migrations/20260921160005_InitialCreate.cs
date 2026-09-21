using System;
using Microsoft.EntityFrameworkCore.Migrations;
using TradeJournal.Api.Models;

#nullable disable

namespace TradeJournal.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:trade_side", "buy,sell")
                .Annotation("Npgsql:Enum:trade_status", "open,closed");

            migrationBuilder.CreateTable(
                name: "accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BaseCurrency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "trades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    Symbol = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Side = table.Column<TradeSide>(type: "trade_side", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(28,10)", precision: 28, scale: 10, nullable: false),
                    EntryPrice = table.Column<decimal>(type: "numeric(28,10)", precision: 28, scale: 10, nullable: false),
                    ExitPrice = table.Column<decimal>(type: "numeric(28,10)", precision: 28, scale: 10, nullable: true),
                    Fees = table.Column<decimal>(type: "numeric(28,10)", precision: 28, scale: 10, nullable: false),
                    RealizedPnl = table.Column<decimal>(type: "numeric(28,10)", precision: 28, scale: 10, nullable: true),
                    Status = table.Column<TradeStatus>(type: "trade_status", nullable: false),
                    OpenedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ClosedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trades", x => x.Id);
                    table.CheckConstraint("ck_trades_entry_price_positive", "\"EntryPrice\" > 0");
                    table.CheckConstraint("ck_trades_fees_non_negative", "\"Fees\" >= 0");
                    table.CheckConstraint("ck_trades_quantity_positive", "\"Quantity\" > 0");
                    table.ForeignKey(
                        name: "FK_trades_accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "trade_tags",
                columns: table => new
                {
                    TagsId = table.Column<Guid>(type: "uuid", nullable: false),
                    TradesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trade_tags", x => new { x.TagsId, x.TradesId });
                    table.ForeignKey(
                        name: "FK_trade_tags_tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trade_tags_trades_TradesId",
                        column: x => x.TradesId,
                        principalTable: "trades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_accounts_Name",
                table: "accounts",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tags_Name",
                table: "tags",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_trade_tags_TradesId",
                table: "trade_tags",
                column: "TradesId");

            migrationBuilder.CreateIndex(
                name: "IX_trades_AccountId_OpenedAt",
                table: "trades",
                columns: new[] { "AccountId", "OpenedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_trades_Symbol_Status",
                table: "trades",
                columns: new[] { "Symbol", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "trade_tags");

            migrationBuilder.DropTable(
                name: "tags");

            migrationBuilder.DropTable(
                name: "trades");

            migrationBuilder.DropTable(
                name: "accounts");
        }
    }
}
