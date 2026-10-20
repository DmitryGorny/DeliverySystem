using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orders.Migrations
{
    /// <inheritdoc />
    public partial class FixIndexAddPricePositions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_positions_orders_OrdersId",
                table: "positions");

            migrationBuilder.DropIndex(
                name: "IX_positions_OrdersId",
                table: "positions");

            migrationBuilder.DropColumn(
                name: "OrdersId",
                table: "positions");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "positions",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_positions_OrderId",
                table: "positions",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_positions_orders_OrderId",
                table: "positions",
                column: "OrderId",
                principalTable: "orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_positions_orders_OrderId",
                table: "positions");

            migrationBuilder.DropIndex(
                name: "IX_positions_OrderId",
                table: "positions");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "positions");

            migrationBuilder.AddColumn<Guid>(
                name: "OrdersId",
                table: "positions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_positions_OrdersId",
                table: "positions",
                column: "OrdersId");

            migrationBuilder.AddForeignKey(
                name: "FK_positions_orders_OrdersId",
                table: "positions",
                column: "OrdersId",
                principalTable: "orders",
                principalColumn: "Id");
        }
    }
}
