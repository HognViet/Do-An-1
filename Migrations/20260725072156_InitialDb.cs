using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace San_Pham_Do_An1.Migrations
{
    public partial class InitialDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tb_BlogCategory",
                columns: table => new
                {
                    BlogCategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Alias = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_BlogCategory", x => x.BlogCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "tb_Color",
                columns: table => new
                {
                    ColorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ColorName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ColorCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_Color", x => x.ColorId);
                });

            migrationBuilder.CreateTable(
                name: "tb_Customer",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Birthday = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Avatar = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LastLogin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_Customer", x => x.CustomerId);
                });

            migrationBuilder.CreateTable(
                name: "tb_Menu",
                columns: table => new
                {
                    MenuId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Alias = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Levels = table.Column<int>(type: "int", nullable: true),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    Position = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_Menu", x => x.MenuId);
                });

            migrationBuilder.CreateTable(
                name: "tb_ProductCategory",
                columns: table => new
                {
                    CategoryProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Alias = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Position = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_ProductCategory", x => x.CategoryProductId);
                });

            migrationBuilder.CreateTable(
                name: "tb_Role",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_Role", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "tb_Size",
                columns: table => new
                {
                    SizeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SizeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SizeOrder = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_Size", x => x.SizeId);
                });

            migrationBuilder.CreateTable(
                name: "tb_Product",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Alias = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CategoryProductId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Detail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PriceSale = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsNew = table.Column<bool>(type: "bit", nullable: false),
                    IsBestSeller = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    Star = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_Product", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_tb_Product_tb_ProductCategory_CategoryProductId",
                        column: x => x.CategoryProductId,
                        principalTable: "tb_ProductCategory",
                        principalColumn: "CategoryProductId");
                });

            migrationBuilder.CreateTable(
                name: "tb_Account",
                columns: table => new
                {
                    AccountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: true),
                    LastLogin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_Account", x => x.AccountId);
                    table.ForeignKey(
                        name: "FK_tb_Account_tb_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "tb_Role",
                        principalColumn: "RoleId");
                });

            migrationBuilder.CreateTable(
                name: "tb_ProductReview",
                columns: table => new
                {
                    ProductReviewId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    Detail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Star = table.Column<int>(type: "int", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_ProductReview", x => x.ProductReviewId);
                    table.ForeignKey(
                        name: "FK_tb_ProductReview_tb_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "tb_Customer",
                        principalColumn: "CustomerId");
                    table.ForeignKey(
                        name: "FK_tb_ProductReview_tb_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "tb_Product",
                        principalColumn: "ProductId");
                });

            migrationBuilder.CreateTable(
                name: "tb_Blog",
                columns: table => new
                {
                    BlogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Alias = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BlogCategoryId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Detail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AccountId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_Blog", x => x.BlogId);
                    table.ForeignKey(
                        name: "FK_tb_Blog_tb_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "tb_Account",
                        principalColumn: "AccountId");
                    table.ForeignKey(
                        name: "FK_tb_Blog_tb_BlogCategory_BlogCategoryId",
                        column: x => x.BlogCategoryId,
                        principalTable: "tb_BlogCategory",
                        principalColumn: "BlogCategoryId");
                });

            migrationBuilder.InsertData(
                table: "tb_BlogCategory",
                columns: new[] { "BlogCategoryId", "Alias", "CreatedDate", "Description", "Image", "Title" },
                values: new object[,]
                {
                    { 1, "thoi-trang", new DateTime(2026, 7, 2, 9, 14, 27, 318, DateTimeKind.Unspecified), "Tin tức về thời trang", "/assets/img/product/small-product1.png", "Thời trang" },
                    { 2, "meo-mac-dep", new DateTime(2026, 7, 8, 16, 42, 51, 604, DateTimeKind.Unspecified), "Hướng dẫn phối đồ", "/assets/img/product/small-product2.png", "Mẹo mặc đẹp" },
                    { 3, "khuyen-mai", new DateTime(2026, 7, 17, 20, 5, 36, 147, DateTimeKind.Unspecified), "Các ưu đãi và giảm giá", "/assets/img/product/small-product3.png", "Tin khuyến mại" },
                    { 4, "song", new DateTime(2026, 7, 28, 11, 57, 13, 892, DateTimeKind.Unspecified), "Lifestyle & tips", "/assets/img/product/small-product4.png", "Phong cách sống" }
                });

            migrationBuilder.InsertData(
                table: "tb_Color",
                columns: new[] { "ColorId", "ColorCode", "ColorName", "IsActive" },
                values: new object[,]
                {
                    { 1, "#FFFFFF", "Trắng", true },
                    { 2, "#000000", "Đen", true },
                    { 3, "#808080", "Xám", true },
                    { 4, "#FF0000", "Đỏ", true },
                    { 5, "#0000FF", "Xanh dương", true },
                    { 6, "#00AA00", "Xanh lá", true },
                    { 7, "#FFFF00", "Vàng", true },
                    { 8, "#FFC0CB", "Hồng", true },
                    { 9, "#8B4513", "Nâu", true },
                    { 10, "#800080", "Tím", true }
                });

            migrationBuilder.InsertData(
                table: "tb_Customer",
                columns: new[] { "CustomerId", "Avatar", "Birthday", "Email", "IsActive", "LastLogin", "Location", "Name", "Password", "Phone", "Username" },
                values: new object[,]
                {
                    { 1, null, new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "a@example.com", true, new DateTime(2026, 7, 5, 7, 24, 31, 842, DateTimeKind.Unspecified), "Hanoi", "Nguyen Van A", "$2y$10$hash1", "0901111001", "nguyena" },
                    { 2, null, new DateTime(1992, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "b@example.com", true, new DateTime(2026, 7, 17, 18, 53, 46, 275, DateTimeKind.Unspecified), "HCMC", "Le Thi B", "$2y$10$hash2", "0901111002", "lethib" },
                    { 3, null, new DateTime(1988, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "c@example.com", true, new DateTime(2026, 7, 26, 22, 11, 8, 503, DateTimeKind.Unspecified), "Da Nang", "Tran Van C", "$2y$10$hash3", "0901111003", "tranvc" }
                });

            migrationBuilder.InsertData(
                table: "tb_Menu",
                columns: new[] { "MenuId", "Alias", "CreatedBy", "CreatedDate", "Description", "IsActive", "Levels", "ModifiedBy", "ModifiedDate", "ParentId", "Position", "Title" },
                values: new object[,]
                {
                    { 1, "Home", "admin", new DateTime(2026, 7, 3, 8, 14, 25, 317, DateTimeKind.Unspecified), "Menu Trang chủ", true, 1, null, null, null, 1, "Trang chủ" },
                    { 2, "Product", "admin", new DateTime(2026, 7, 7, 17, 42, 18, 904, DateTimeKind.Unspecified), "Menu Sản phẩm", true, 1, null, null, null, 2, "Sản phẩm" },
                    { 3, "tin-tuc", "admin", new DateTime(2026, 7, 10, 21, 9, 56, 481, DateTimeKind.Unspecified), "Menu Tin tức", false, 1, null, null, null, 3, "Tin tức" },
                    { 4, "Blog", "admin", new DateTime(2026, 7, 12, 11, 36, 43, 225, DateTimeKind.Unspecified), "Menu Blog", true, 1, null, null, null, 4, "Bài viết" },
                    { 5, "About", "admin", new DateTime(2026, 7, 15, 9, 51, 7, 638, DateTimeKind.Unspecified), "Menu Giới thiệu", true, 1, null, null, null, 5, "Giới thiệu" },
                    { 6, "Contact", "admin", new DateTime(2026, 7, 18, 14, 27, 31, 752, DateTimeKind.Unspecified), "Menu Liên hệ", true, 1, null, null, null, 6, "Liên hệ" },
                    { 7, "khuyen-mai", "admin", new DateTime(2026, 7, 20, 20, 13, 49, 116, DateTimeKind.Unspecified), "Menu KM", true, 2, null, null, 3, 1, "Khuyến mại" },
                    { 8, "nam", "admin", new DateTime(2026, 7, 23, 7, 45, 12, 593, DateTimeKind.Unspecified), "Danh mục nam", true, 2, null, null, 2, 1, "Nam" },
                    { 9, "nu", "admin", new DateTime(2026, 7, 26, 16, 18, 54, 871, DateTimeKind.Unspecified), "Danh mục nữ", true, 2, null, null, 2, 2, "Nữ" },
                    { 10, "the-thao", "admin", new DateTime(2026, 7, 29, 23, 58, 36, 404, DateTimeKind.Unspecified), "Danh mục thể thao", true, 2, null, null, 2, 3, "Thể thao" }
                });

            migrationBuilder.InsertData(
                table: "tb_ProductCategory",
                columns: new[] { "CategoryProductId", "Alias", "CreatedDate", "Description", "Icon", "Position", "Title" },
                values: new object[,]
                {
                    { 1, "thoi-trang-nam", new DateTime(2026, 7, 14, 8, 23, 17, 451, DateTimeKind.Unspecified), "Quần áo & phụ kiện dành cho nam", "/assets/img/product/small-product1.png", 1, "Thời trang nam" },
                    { 2, "thoi-trang-nu", new DateTime(2026, 7, 3, 19, 45, 52, 183, DateTimeKind.Unspecified), "Quần áo & phụ kiện dành cho nữ", "/assets/img/product/small-product2.png", 2, "Áo sơ mi" },
                    { 3, "giay-dep", new DateTime(2026, 7, 28, 13, 11, 36, 924, DateTimeKind.Unspecified), "Các loại giày dép", "/assets/img/product/small-product3.png", 3, "Áo khoác" },
                    { 4, "tui-xach", new DateTime(2026, 7, 9, 22, 58, 9, 615, DateTimeKind.Unspecified), "Túi xách thời trang", "/assets/img/product/small-product4.png", 4, "Áo hoodie" },
                    { 5, "phu-kien", new DateTime(2026, 7, 21, 5, 34, 41, 207, DateTimeKind.Unspecified), "Phụ kiện", "/assets/img/product/small-product5.png", 5, "Áo polo" },
                    { 6, "do-the-thao", new DateTime(2026, 7, 17, 16, 7, 58, 332, DateTimeKind.Unspecified), "Trang phục thể thao", "/assets/img/product/small-product6.png", 6, "Áo len" },
                    { 7, "do-tre-em", new DateTime(2026, 7, 30, 11, 52, 24, 786, DateTimeKind.Unspecified), "Sản phẩm cho trẻ em", "/assets/img/product/small-product7.png", 7, "Áo cardigan" },
                    { 8, "dien-tu", new DateTime(2026, 7, 12, 0, 16, 33, 549, DateTimeKind.Unspecified), "Phụ kiện điện tử", "/assets/img/product/small-product8.png", 8, "Áo dài tay" },
                    { 9, "sac-dep", new DateTime(2026, 7, 6, 21, 29, 14, 968, DateTimeKind.Unspecified), "Mỹ phẩm & chăm sóc", "/assets/img/product/small-product9.png", 9, "Áo thể thao" },
                    { 10, "nha-cua", new DateTime(2026, 7, 25, 9, 40, 27, 125, DateTimeKind.Unspecified), "Đồ dùng gia đình", "/assets/img/product/small-product10.png", 10, "Áo vest" }
                });

            migrationBuilder.InsertData(
                table: "tb_Role",
                columns: new[] { "RoleId", "Description", "RoleName" },
                values: new object[,]
                {
                    { 1, "Quản trị", "Admin" },
                    { 2, "Người bán hàng", "Seller" }
                });

            migrationBuilder.InsertData(
                table: "tb_Size",
                columns: new[] { "SizeId", "IsActive", "SizeName", "SizeOrder" },
                values: new object[,]
                {
                    { 1, true, "XS", 1 },
                    { 2, true, "S", 2 },
                    { 3, true, "M", 3 }
                });

            migrationBuilder.InsertData(
                table: "tb_Size",
                columns: new[] { "SizeId", "IsActive", "SizeName", "SizeOrder" },
                values: new object[,]
                {
                    { 4, true, "L", 4 },
                    { 5, true, "XL", 5 },
                    { 6, true, "XXL", 6 },
                    { 7, true, "OneSize", 7 },
                    { 8, true, "36", 8 },
                    { 9, true, "37", 9 },
                    { 10, true, "38", 10 }
                });

            migrationBuilder.InsertData(
                table: "tb_Account",
                columns: new[] { "AccountId", "Email", "FullName", "IsActive", "LastLogin", "Password", "Phone", "RoleId", "Username" },
                values: new object[,]
                {
                    { 1, "admin@example.com", "ADMIN", true, new DateTime(2026, 7, 18, 8, 23, 17, 451, DateTimeKind.Unspecified), "admin", "0901000001", 1, "admin" },
                    { 2, "editor@example.com", "NHAN VIEN", true, new DateTime(2026, 7, 9, 19, 45, 52, 183, DateTimeKind.Unspecified), "nhanvien", "0901000002", 2, "nhanvien" },
                    { 13, "hognviet@gmail.com", "Viet Viet", true, new DateTime(2026, 7, 27, 13, 11, 36, 924, DateTimeKind.Unspecified), "81dc9bdb52d04dc20036dbd8313ed055", "0345472946", 1, "hognviet" },
                    { 14, "son@gmail.com", "Ngoo Son", true, new DateTime(2026, 7, 5, 22, 58, 9, 615, DateTimeKind.Unspecified), "81dc9bdb52d04dc20036dbd8313ed055", "0145678923", 2, "son" }
                });

            migrationBuilder.InsertData(
                table: "tb_Product",
                columns: new[] { "ProductId", "Alias", "CategoryProductId", "CreatedBy", "CreatedDate", "Description", "Detail", "Image", "IsActive", "IsBestSeller", "IsNew", "ModifiedBy", "ModifiedDate", "Price", "PriceSale", "Quantity", "Star", "Title" },
                values: new object[,]
                {
                    { 1, "ao-thun-cotton-nam", 1, "admin", new DateTime(2026, 7, 3, 8, 15, 27, 416, DateTimeKind.Unspecified), "Áo thun cotton thoáng mát", null, "/assets/img/product/product1.png", true, true, true, null, null, 250000.00m, 199000.00m, 190, 5.0, "Áo thun cotton nam" },
                    { 2, "quan-jeans-nam", 1, "admin", new DateTime(2026, 7, 8, 17, 42, 51, 783, DateTimeKind.Unspecified), "Quần jeans co giãn", null, "/assets/img/product/product2.png", true, true, false, null, null, 300000.00m, 399000.00m, 210, 4.0, "Quần jeans nam" },
                    { 3, "dam-nu-maxi", 2, "admin", new DateTime(2026, 7, 14, 12, 36, 18, 592, DateTimeKind.Unspecified), "Đầm maxi nữ", null, "/assets/img/product/product3.png", true, false, true, null, null, 550000.00m, 499000.00m, 240, 5.0, "Đầm nữ maxi" },
                    { 4, "giay-sneakers", 3, "admin", new DateTime(2026, 7, 21, 20, 9, 45, 138, DateTimeKind.Unspecified), "Giày thể thao unisex", null, "/assets/img/product/product4.png", true, true, false, null, null, 800000.00m, 650000.00m, 330, 4.0, "Áo phông nữ" },
                    { 5, "tui-xach-da", 4, "admin", new DateTime(2026, 7, 29, 9, 58, 12, 964, DateTimeKind.Unspecified), "Túi xách da thật", null, "/assets/img/product/product5.png", true, false, false, null, null, 1200000.00m, 999000.00m, 150, 5.0, "Áo khoác da" }
                });

            migrationBuilder.InsertData(
                table: "tb_Blog",
                columns: new[] { "BlogId", "AccountId", "Alias", "BlogCategoryId", "CreatedBy", "CreatedDate", "Description", "Detail", "Image", "IsActive", "ModifiedBy", "ModifiedDate", "Title" },
                values: new object[,]
                {
                    { 1, 2, null, 1, "admin", new DateTime(2026, 7, 4, 8, 36, 19, 421, DateTimeKind.Unspecified), "Những xu hướng nổi bật 2025", null, "/assets/img/blog/blog1.png", true, null, null, "Xu hướng thời trang 2025" },
                    { 2, 2, "cach-phoi-do", 2, "admin", new DateTime(2026, 7, 15, 14, 28, 42, 765, DateTimeKind.Unspecified), "Mẹo phối đồ", null, "/assets/img/blog/blog2.png", true, null, null, "Cách phối đồ cơ bản" },
                    { 3, 2, "huong-dan-nuoc-hoa", 4, "admin", new DateTime(2026, 7, 25, 22, 49, 8, 233, DateTimeKind.Unspecified), "Nước hoa tự chế", null, "/assets/img/blog/blog3.png", true, null, null, "Hướng dẫn chưng cất nước hoa" }
                });

            migrationBuilder.InsertData(
                table: "tb_ProductReview",
                columns: new[] { "ProductReviewId", "CreatedDate", "CustomerId", "Detail", "IsActive", "ProductId", "Star" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 7, 6, 10, 32, 19, 647, DateTimeKind.Unspecified), 1, "Sản phẩm tốt, chất lượng ổn.", true, 1, 5 },
                    { 2, new DateTime(2026, 7, 19, 15, 47, 58, 114, DateTimeKind.Unspecified), 2, "Vải mềm, giao hàng nhanh.", true, 1, 2 },
                    { 3, new DateTime(2026, 7, 30, 21, 6, 42, 389, DateTimeKind.Unspecified), 3, "Form đẹp nhưng hơi chật.", true, 2, 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_tb_Account_RoleId",
                table: "tb_Account",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_Blog_AccountId",
                table: "tb_Blog",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_Blog_BlogCategoryId",
                table: "tb_Blog",
                column: "BlogCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_Product_CategoryProductId",
                table: "tb_Product",
                column: "CategoryProductId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_ProductReview_CustomerId",
                table: "tb_ProductReview",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_ProductReview_ProductId",
                table: "tb_ProductReview",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_Blog");

            migrationBuilder.DropTable(
                name: "tb_Color");

            migrationBuilder.DropTable(
                name: "tb_Menu");

            migrationBuilder.DropTable(
                name: "tb_ProductReview");

            migrationBuilder.DropTable(
                name: "tb_Size");

            migrationBuilder.DropTable(
                name: "tb_Account");

            migrationBuilder.DropTable(
                name: "tb_BlogCategory");

            migrationBuilder.DropTable(
                name: "tb_Customer");

            migrationBuilder.DropTable(
                name: "tb_Product");

            migrationBuilder.DropTable(
                name: "tb_Role");

            migrationBuilder.DropTable(
                name: "tb_ProductCategory");
        }
    }
}
