using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace San_Pham_Do_An1.Migrations
{
    public partial class AddCartAndCustomer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tb_OrderStatus",
                columns: table => new
                {
                    OrderStatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_OrderStatus", x => x.OrderStatusId);
                });

            migrationBuilder.CreateTable(
                name: "tb_ProductVariant",
                columns: table => new
                {
                    VariantId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    ColorId = table.Column<int>(type: "int", nullable: true),
                    SizeId = table.Column<int>(type: "int", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Sku = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PriceSale = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_ProductVariant", x => x.VariantId);
                    table.ForeignKey(
                        name: "FK_tb_ProductVariant_tb_Color_ColorId",
                        column: x => x.ColorId,
                        principalTable: "tb_Color",
                        principalColumn: "ColorId");
                    table.ForeignKey(
                        name: "FK_tb_ProductVariant_tb_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "tb_Product",
                        principalColumn: "ProductId");
                    table.ForeignKey(
                        name: "FK_tb_ProductVariant_tb_Size_SizeId",
                        column: x => x.SizeId,
                        principalTable: "tb_Size",
                        principalColumn: "SizeId");
                });

            migrationBuilder.CreateTable(
                name: "tb_Order",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    ShippingAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OrderStatusId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentMethod = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_Order", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_tb_Order_tb_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "tb_Customer",
                        principalColumn: "CustomerId");
                    table.ForeignKey(
                        name: "FK_tb_Order_tb_OrderStatus_OrderStatusId",
                        column: x => x.OrderStatusId,
                        principalTable: "tb_OrderStatus",
                        principalColumn: "OrderStatusId");
                });

            migrationBuilder.CreateTable(
                name: "tb_OrderDetail",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_OrderDetail", x => new { x.OrderId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_tb_OrderDetail_tb_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "tb_Order",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tb_OrderDetail_tb_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "tb_Product",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "tb_Blog",
                keyColumn: "BlogId",
                keyValue: 1,
                column: "Description",
                value: "Nhưng xu hướng nổi bật 2025");

            migrationBuilder.InsertData(
                table: "tb_OrderStatus",
                columns: new[] { "OrderStatusId", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Chờ xử lý", "Pending" },
                    { 3, "Đã xử lý", "Processing" },
                    { 4, "Đã gửi hàng", "Shipped" },
                    { 5, "Đã giao", "Delivered" },
                    { 6, "Đã hủy", "Canceled" },
                    { 7, "Đã hoàn tiền", "Refunded" },
                    { 9, "Trả hàng", "Returned" },
                    { 10, "Giao dịch thất bại", "Failed" }
                });

            migrationBuilder.InsertData(
                table: "tb_ProductVariant",
                columns: new[] { "VariantId", "ColorId", "Image", "IsActive", "Price", "PriceSale", "ProductId", "Quantity", "SizeId", "Sku" },
                values: new object[,]
                {
                    { 11, 1, "/files/product1.png", true, 250000.00m, 199000.00m, 1, 40, 1, "ATCN-DEN-S" },
                    { 12, 1, "/files/big-product2.jpg", true, 250000.00m, 199000.00m, 1, 35, 2, "ATCN-DEN-M" },
                    { 13, 1, "/files/big-product3.jpg", true, 250000.00m, 199000.00m, 1, 30, 3, "ATCN-DEN-L" },
                    { 14, 3, "/files/big-product4.jpg", true, 250000.00m, 199000.00m, 1, 45, 1, "ATCN-TRANG-S" },
                    { 15, 3, "/files/big-product5.jpg", true, 250000.00m, 199000.00m, 1, 40, 2, "ATCN-TRANG-M" },
                    { 17, 1, "/files/product2.png", true, 300000.00m, 399000.00m, 2, 40, 1, "QJN-DEN-S" },
                    { 18, 1, "/files/big-product4.jpg", true, 450000.00m, 399000.00m, 2, 35, 2, "QJN-DEN-M" },
                    { 19, 1, "/files/product1.png", true, 450000.00m, 399000.00m, 2, 30, 3, "QJN-DEN-L" },
                    { 20, 2, "/files/big-product4.jpg", true, 450000.00m, 399000.00m, 2, 40, 1, "QJN-XANH-S" },
                    { 21, 2, "/files/big-product5.jpg", true, 450000.00m, 399000.00m, 2, 35, 2, "QJN-XANH-M" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_tb_Order_CustomerId",
                table: "tb_Order",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_Order_OrderStatusId",
                table: "tb_Order",
                column: "OrderStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_OrderDetail_ProductId",
                table: "tb_OrderDetail",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_ProductVariant_ColorId",
                table: "tb_ProductVariant",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_ProductVariant_ProductId",
                table: "tb_ProductVariant",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_ProductVariant_SizeId",
                table: "tb_ProductVariant",
                column: "SizeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_OrderDetail");

            migrationBuilder.DropTable(
                name: "tb_ProductVariant");

            migrationBuilder.DropTable(
                name: "tb_Order");

            migrationBuilder.DropTable(
                name: "tb_OrderStatus");

            migrationBuilder.UpdateData(
                table: "tb_Blog",
                keyColumn: "BlogId",
                keyValue: 1,
                column: "Description",
                value: "Những xu hướng nổi bật 2025");
        }
    }
}
