using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Orders.Migrations
{
    /// <inheritdoc />
    public partial class SeedingIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_orders_OrderStatusId",
                table: "orders");

            migrationBuilder.InsertData(
                table: "order_status",
                columns: new[] { "Id", "Status" },
                values: new object[,]
                {
                    { 1, "AwaitPayment" },
                    { 2, "DelayedPayment" },
                    { 3, "ToDelivery" },
                    { 4, "Delivered" },
                    { 5, "DeliveredWithoutPayment" },
                    { 6, "Done" },
                    { 7, "CancelledWithoutPayment" },
                    { 8, "CancellPaid" },
                    { 9, "CancellDelivered" },
                    { 10, "PaymentExpired" },
                    { 11, "PaymnetDeclined" },
                    { 12, "ReservationFailed" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_orders_OrderStatusId_CreatedAt",
                table: "orders",
                columns: new[] { "OrderStatusId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_orders_OrderStatusId_CreatedAt",
                table: "orders");

            migrationBuilder.DeleteData(
                table: "order_status",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "order_status",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "order_status",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "order_status",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "order_status",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "order_status",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "order_status",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "order_status",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "order_status",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "order_status",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "order_status",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "order_status",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.CreateIndex(
                name: "IX_orders_OrderStatusId",
                table: "orders",
                column: "OrderStatusId");
        }
    }
}
