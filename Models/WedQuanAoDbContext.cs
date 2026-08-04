using Microsoft.EntityFrameworkCore;
using System;

namespace San_Pham_Do_An1.Models
{
    public class WedQuanAoDbContext : DbContext
    {
        public WedQuanAoDbContext()
        {
        }

        public WedQuanAoDbContext(DbContextOptions<WedQuanAoDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TbProduct> TbProducts { get; set; } = null!;
        public virtual DbSet<TbProductCategory> TbProductCategories { get; set; } = null!;
        public virtual DbSet<TbBlog> TbBlogs { get; set; } = null!;
        public virtual DbSet<TbBlogCategory> TbBlogCategories { get; set; } = null!;
        public virtual DbSet<TbCustomer> TbCustomers { get; set; } = null!;
        public virtual DbSet<TbProductReview> TbProductReviews { get; set; } = null!;
        public virtual DbSet<TbAccount> TbAccounts { get; set; } = null!;
        public virtual DbSet<TbRole> TbRoles { get; set; } = null!;
        public virtual DbSet<TbMenu> TbMenus { get; set; } = null!;
        public virtual DbSet<TbColor> TbColors { get; set; } = null!;
        public virtual DbSet<TbSize> TbSizes { get; set; } = null!;
        public virtual DbSet<TbProductVariant> TbProductVariants { get; set; } = null!;
        public virtual DbSet<TbOrder> TbOrders { get; set; } = null!;
        public virtual DbSet<TbOrderDetail> TbOrderDetails { get; set; } = null!;
        public virtual DbSet<TbOrderStatus> TbOrderStatuses { get; set; } = null!;
        public virtual DbSet<TbContact> TbContacts { get; set; } = null!;
        public virtual DbSet<TbChatMessage> TbChatMessages { get; set; } = null!;
        public virtual DbSet<TbBlogComment> TbBlogComments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure composite primary key for TbOrderDetail
            modelBuilder.Entity<TbOrderDetail>().HasKey(e => new { e.OrderId, e.ProductId });

            modelBuilder.Entity<TbBlogComment>(entity =>
            {
                entity.HasKey(e => e.CommentId);
                entity.ToTable("tb_BlogComment");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.HasOne(d => d.Blog).WithMany(p => p.TbBlogComments)
                    .HasForeignKey(d => d.BlogId);
                entity.HasOne(d => d.Customer).WithMany(p => p.TbBlogComments)
                    .HasForeignKey(d => d.CustomerId);
            });

            modelBuilder.Entity<TbChatMessage>(entity =>
            {
                entity.HasKey(e => e.MessageId);
                entity.ToTable("tb_ChatMessage");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.GuestToken).HasMaxLength(100);
                entity.Property(e => e.Message).HasColumnType("ntext");
                entity.Property(e => e.Sender).HasMaxLength(10);
                entity.HasOne(d => d.User).WithMany(p => p.TbChatMessages)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<TbContact>(entity =>
            {
                entity.HasKey(e => e.ContactId);
                entity.ToTable("tb_Contact");
                entity.Property(e => e.CreatedBy).HasMaxLength(50);
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.ModifiedBy).HasMaxLength(50);
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(20);
            });

            // Seed Data for tb_Role
            modelBuilder.Entity<TbRole>().HasData(
                new TbRole { RoleId = 1, RoleName = "Admin", Description = "Quản trị" },
                new TbRole { RoleId = 2, RoleName = "Seller", Description = "Người bán hàng" }
            );

            // Seed Data for tb_Account
            modelBuilder.Entity<TbAccount>().HasData(
                new TbAccount { AccountId = 1, Username = "admin", Password = "admin", FullName = "ADMIN", Phone = "0901000001", Email = "admin@example.com", RoleId = 1, IsActive = true, LastLogin = DateTime.Parse("2026-07-18 08:23:17.451") },
                new TbAccount { AccountId = 2, Username = "nhanvien", Password = "nhanvien", FullName = "NHAN VIEN", Phone = "0901000002", Email = "editor@example.com", RoleId = 2, IsActive = true, LastLogin = DateTime.Parse("2026-07-09 19:45:52.183") },
                new TbAccount { AccountId = 13, Username = "hognviet", Password = "81dc9bdb52d04dc20036dbd8313ed055", FullName = "Viet Viet", Phone = "0345472946", Email = "hognviet@gmail.com", RoleId = 1, IsActive = true, LastLogin = DateTime.Parse("2026-07-27 13:11:36.924") },
                new TbAccount { AccountId = 14, Username = "son", Password = "81dc9bdb52d04dc20036dbd8313ed055", FullName = "Ngoo Son", Phone = "0145678923", Email = "son@gmail.com", RoleId = 2, IsActive = true, LastLogin = DateTime.Parse("2026-07-05 22:58:09.615") }
            );

            // Seed Data for tb_ProductCategory
            modelBuilder.Entity<TbProductCategory>().HasData(
                new TbProductCategory { CategoryProductId = 1, Title = "Thời trang nam", Alias = "thoi-trang-nam", Description = "Quần áo & phụ kiện dành cho nam", Icon = "/assets/img/product/small-product1.png", Position = 1, CreatedDate = DateTime.Parse("2026-07-14 08:23:17.451") },
                new TbProductCategory { CategoryProductId = 2, Title = "Áo sơ mi", Alias = "thoi-trang-nu", Description = "Quần áo & phụ kiện dành cho nữ", Icon = "/assets/img/product/small-product2.png", Position = 2, CreatedDate = DateTime.Parse("2026-07-03 19:45:52.183") },
                new TbProductCategory { CategoryProductId = 3, Title = "Áo khoác", Alias = "giay-dep", Description = "Các loại giày dép", Icon = "/assets/img/product/small-product3.png", Position = 3, CreatedDate = DateTime.Parse("2026-07-28 13:11:36.924") },
                new TbProductCategory { CategoryProductId = 4, Title = "Áo hoodie", Alias = "tui-xach", Description = "Túi xách thời trang", Icon = "/assets/img/product/small-product4.png", Position = 4, CreatedDate = DateTime.Parse("2026-07-09 22:58:09.615") },
                new TbProductCategory { CategoryProductId = 5, Title = "Áo polo", Alias = "phu-kien", Description = "Phụ kiện", Icon = "/assets/img/product/small-product5.png", Position = 5, CreatedDate = DateTime.Parse("2026-07-21 05:34:41.207") },
                new TbProductCategory { CategoryProductId = 6, Title = "Áo len", Alias = "do-the-thao", Description = "Trang phục thể thao", Icon = "/assets/img/product/small-product6.png", Position = 6, CreatedDate = DateTime.Parse("2026-07-17 16:07:58.332") },
                new TbProductCategory { CategoryProductId = 7, Title = "Áo cardigan", Alias = "do-tre-em", Description = "Sản phẩm cho trẻ em", Icon = "/assets/img/product/small-product7.png", Position = 7, CreatedDate = DateTime.Parse("2026-07-30 11:52:24.786") },
                new TbProductCategory { CategoryProductId = 8, Title = "Áo dài tay", Alias = "dien-tu", Description = "Phụ kiện điện tử", Icon = "/assets/img/product/small-product8.png", Position = 8, CreatedDate = DateTime.Parse("2026-07-12 00:16:33.549") },
                new TbProductCategory { CategoryProductId = 9, Title = "Áo thể thao", Alias = "sac-dep", Description = "Mỹ phẩm & chăm sóc", Icon = "/assets/img/product/small-product9.png", Position = 9, CreatedDate = DateTime.Parse("2026-07-06 21:29:14.968") },
                new TbProductCategory { CategoryProductId = 10, Title = "Áo vest", Alias = "nha-cua", Description = "Đồ dùng gia đình", Icon = "/assets/img/product/small-product10.png", Position = 10, CreatedDate = DateTime.Parse("2026-07-25 09:40:27.125") }
            );

            // Seed Data for tb_Color
            modelBuilder.Entity<TbColor>().HasData(
                new TbColor { ColorId = 1, ColorName = "Trắng", ColorCode = "#FFFFFF", IsActive = true },
                new TbColor { ColorId = 2, ColorName = "Đen", ColorCode = "#000000", IsActive = true },
                new TbColor { ColorId = 3, ColorName = "Xám", ColorCode = "#808080", IsActive = true },
                new TbColor { ColorId = 4, ColorName = "Đỏ", ColorCode = "#FF0000", IsActive = true },
                new TbColor { ColorId = 5, ColorName = "Xanh dương", ColorCode = "#0000FF", IsActive = true },
                new TbColor { ColorId = 6, ColorName = "Xanh lá", ColorCode = "#00AA00", IsActive = true },
                new TbColor { ColorId = 7, ColorName = "Vàng", ColorCode = "#FFFF00", IsActive = true },
                new TbColor { ColorId = 8, ColorName = "Hồng", ColorCode = "#FFC0CB", IsActive = true },
                new TbColor { ColorId = 9, ColorName = "Nâu", ColorCode = "#8B4513", IsActive = true },
                new TbColor { ColorId = 10, ColorName = "Tím", ColorCode = "#800080", IsActive = true }
            );

            // Seed Data for tb_Size
            modelBuilder.Entity<TbSize>().HasData(
                new TbSize { SizeId = 1, SizeName = "XS", SizeOrder = 1, IsActive = true },
                new TbSize { SizeId = 2, SizeName = "S", SizeOrder = 2, IsActive = true },
                new TbSize { SizeId = 3, SizeName = "M", SizeOrder = 3, IsActive = true },
                new TbSize { SizeId = 4, SizeName = "L", SizeOrder = 4, IsActive = true },
                new TbSize { SizeId = 5, SizeName = "XL", SizeOrder = 5, IsActive = true },
                new TbSize { SizeId = 6, SizeName = "XXL", SizeOrder = 6, IsActive = true },
                new TbSize { SizeId = 7, SizeName = "OneSize", SizeOrder = 7, IsActive = true },
                new TbSize { SizeId = 8, SizeName = "36", SizeOrder = 8, IsActive = true },
                new TbSize { SizeId = 9, SizeName = "37", SizeOrder = 9, IsActive = true },
                new TbSize { SizeId = 10, SizeName = "38", SizeOrder = 10, IsActive = true }
            );

            // Seed Data for tb_Menu
            modelBuilder.Entity<TbMenu>().HasData(
                new TbMenu { MenuId = 1, Title = "Trang chủ", Alias = "Home", Description = "Menu Trang chủ", Levels = 1, ParentId = null, Position = 1, IsActive = true, CreatedDate = DateTime.Parse("2026-07-03 08:14:25.317"), CreatedBy = "admin" },
                new TbMenu { MenuId = 2, Title = "Sản phẩm", Alias = "Product", Description = "Menu Sản phẩm", Levels = 1, ParentId = null, Position = 2, IsActive = true, CreatedDate = DateTime.Parse("2026-07-07 17:42:18.904"), CreatedBy = "admin" },
                new TbMenu { MenuId = 3, Title = "Tin tức", Alias = "tin-tuc", Description = "Menu Tin tức", Levels = 1, ParentId = null, Position = 3, IsActive = false, CreatedDate = DateTime.Parse("2026-07-10 21:09:56.481"), CreatedBy = "admin" },
                new TbMenu { MenuId = 4, Title = "Bài viết", Alias = "Blog", Description = "Menu Blog", Levels = 1, ParentId = null, Position = 4, IsActive = true, CreatedDate = DateTime.Parse("2026-07-12 11:36:43.225"), CreatedBy = "admin" },
                new TbMenu { MenuId = 5, Title = "Giới thiệu", Alias = "About", Description = "Menu Giới thiệu", Levels = 1, ParentId = null, Position = 5, IsActive = true, CreatedDate = DateTime.Parse("2026-07-15 09:51:07.638"), CreatedBy = "admin" },
                new TbMenu { MenuId = 6, Title = "Liên hệ", Alias = "Contact", Description = "Menu Liên hệ", Levels = 1, ParentId = null, Position = 6, IsActive = true, CreatedDate = DateTime.Parse("2026-07-18 14:27:31.752"), CreatedBy = "admin" },
                new TbMenu { MenuId = 7, Title = "Khuyến mại", Alias = "khuyen-mai", Description = "Menu KM", Levels = 2, ParentId = 3, Position = 1, IsActive = true, CreatedDate = DateTime.Parse("2026-07-20 20:13:49.116"), CreatedBy = "admin" },
                new TbMenu { MenuId = 8, Title = "Nam", Alias = "nam", Description = "Danh mục nam", Levels = 2, ParentId = 2, Position = 1, IsActive = true, CreatedDate = DateTime.Parse("2026-07-23 07:45:12.593"), CreatedBy = "admin" },
                new TbMenu { MenuId = 9, Title = "Nữ", Alias = "nu", Description = "Danh mục nữ", Levels = 2, ParentId = 2, Position = 2, IsActive = true, CreatedDate = DateTime.Parse("2026-07-26 16:18:54.871"), CreatedBy = "admin" },
                new TbMenu { MenuId = 10, Title = "Thể thao", Alias = "the-thao", Description = "Danh mục thể thao", Levels = 2, ParentId = 2, Position = 3, IsActive = true, CreatedDate = DateTime.Parse("2026-07-29 23:58:36.404"), CreatedBy = "admin" }
            );

            // Seed Data for tb_BlogCategory
            modelBuilder.Entity<TbBlogCategory>().HasData(
                new TbBlogCategory { BlogCategoryId = 1, Title = "Thời trang", Alias = "thoi-trang", Description = "Tin tức về thời trang", Image = "/assets/img/product/small-product1.png", CreatedDate = DateTime.Parse("2026-07-02 09:14:27.318") },
                new TbBlogCategory { BlogCategoryId = 2, Title = "Mẹo mặc đẹp", Alias = "meo-mac-dep", Description = "Hướng dẫn phối đồ", Image = "/assets/img/product/small-product2.png", CreatedDate = DateTime.Parse("2026-07-08 16:42:51.604") },
                new TbBlogCategory { BlogCategoryId = 3, Title = "Tin khuyến mại", Alias = "khuyen-mai", Description = "Các ưu đãi và giảm giá", Image = "/assets/img/product/small-product3.png", CreatedDate = DateTime.Parse("2026-07-17 20:05:36.147") },
                new TbBlogCategory { BlogCategoryId = 4, Title = "Phong cách sống", Alias = "song", Description = "Lifestyle & tips", Image = "/assets/img/product/small-product4.png", CreatedDate = DateTime.Parse("2026-07-28 11:57:13.892") }
            );

            // Seed Data for tb_Blog
            modelBuilder.Entity<TbBlog>().HasData(
<<<<<<< HEAD
                new TbBlog { BlogId = 1, Title = "Xu hướng thời trang 2025", Alias = null, BlogCategoryId = 1, Description = "Nhưng xu hướng nổi bật 2025", Image = "/assets/img/blog/blog1.png", AccountId = 2, IsActive = true, CreatedDate = DateTime.Parse("2026-07-04 08:36:19.421"), CreatedBy = "admin" },
=======
>>>>>>> son
                new TbBlog { BlogId = 2, Title = "Cách phối đồ cơ bản", Alias = "cach-phoi-do", BlogCategoryId = 2, Description = "Mẹo phối đồ", Image = "/assets/img/blog/blog2.png", AccountId = 2, IsActive = true, CreatedDate = DateTime.Parse("2026-07-15 14:28:42.765"), CreatedBy = "admin" },
                new TbBlog { BlogId = 3, Title = "Hướng dẫn chưng cất nước hoa", Alias = "huong-dan-nuoc-hoa", BlogCategoryId = 4, Description = "Nước hoa tự chế", Image = "/assets/img/blog/blog3.png", AccountId = 2, IsActive = true, CreatedDate = DateTime.Parse("2026-07-25 22:49:08.233"), CreatedBy = "admin" }
            );

            // Seed Data for tb_Product
            modelBuilder.Entity<TbProduct>().HasData(
                new TbProduct { ProductId = 1, Title = "Áo thun cotton nam", Alias = "ao-thun-cotton-nam", CategoryProductId = 1, Description = "Áo thun cotton thoáng mát", Image = "/assets/img/product/product1.png", Price = 250000.00m, PriceSale = 199000.00m, IsNew = true, IsBestSeller = true, IsActive = true, Quantity = 190, Star = 5, CreatedDate = DateTime.Parse("2026-07-03 08:15:27.416"), CreatedBy = "admin" },
                new TbProduct { ProductId = 2, Title = "Quần jeans nam", Alias = "quan-jeans-nam", CategoryProductId = 1, Description = "Quần jeans co giãn", Image = "/assets/img/product/product2.png", Price = 300000.00m, PriceSale = 399000.00m, IsNew = false, IsBestSeller = true, IsActive = true, Quantity = 210, Star = 4, CreatedDate = DateTime.Parse("2026-07-08 17:42:51.783"), CreatedBy = "admin" },
                new TbProduct { ProductId = 3, Title = "Đầm nữ maxi", Alias = "dam-nu-maxi", CategoryProductId = 2, Description = "Đầm maxi nữ", Image = "/assets/img/product/product3.png", Price = 550000.00m, PriceSale = 499000.00m, IsNew = true, IsBestSeller = false, IsActive = true, Quantity = 240, Star = 5, CreatedDate = DateTime.Parse("2026-07-14 12:36:18.592"), CreatedBy = "admin" },
                new TbProduct { ProductId = 4, Title = "Áo phông nữ", Alias = "giay-sneakers", CategoryProductId = 3, Description = "Giày thể thao unisex", Image = "/assets/img/product/product4.png", Price = 800000.00m, PriceSale = 650000.00m, IsNew = false, IsBestSeller = true, IsActive = true, Quantity = 330, Star = 4, CreatedDate = DateTime.Parse("2026-07-21 20:09:45.138"), CreatedBy = "admin" },
                new TbProduct { ProductId = 5, Title = "Áo khoác da", Alias = "tui-xach-da", CategoryProductId = 4, Description = "Túi xách da thật", Image = "/assets/img/product/product5.png", Price = 1200000.00m, PriceSale = 999000.00m, IsNew = false, IsBestSeller = false, IsActive = true, Quantity = 150, Star = 5, CreatedDate = DateTime.Parse("2026-07-29 09:58:12.964"), CreatedBy = "admin" }
            );

            // Seed Data for tb_Customer
            modelBuilder.Entity<TbCustomer>().HasData(
                new TbCustomer { CustomerId = 1, Name = "Nguyen Van A", Phone = "0901111001", Email = "a@example.com", Username = "nguyena", Password = "$2y$10$hash1", Birthday = DateTime.Parse("1990-01-01"), Location = "Hanoi", IsActive = true, LastLogin = DateTime.Parse("2026-07-05 07:24:31.842") },
                new TbCustomer { CustomerId = 2, Name = "Le Thi B", Phone = "0901111002", Email = "b@example.com", Username = "lethib", Password = "$2y$10$hash2", Birthday = DateTime.Parse("1992-02-02"), Location = "HCMC", IsActive = true, LastLogin = DateTime.Parse("2026-07-17 18:53:46.275") },
                new TbCustomer { CustomerId = 3, Name = "Tran Van C", Phone = "0901111003", Email = "c@example.com", Username = "tranvc", Password = "$2y$10$hash3", Birthday = DateTime.Parse("1988-03-03"), Location = "Da Nang", IsActive = true, LastLogin = DateTime.Parse("2026-07-26 22:11:08.503") }
            );

            // Seed Data for tb_ProductReview
            modelBuilder.Entity<TbProductReview>().HasData(
                new TbProductReview { ProductReviewId = 1, CustomerId = 1, Detail = "Sản phẩm tốt, chất lượng ổn.", Star = 5, ProductId = 1, CreatedDate = DateTime.Parse("2026-07-06 10:32:19.647"), IsActive = true },
                new TbProductReview { ProductReviewId = 2, CustomerId = 2, Detail = "Vải mềm, giao hàng nhanh.", Star = 2, ProductId = 1, CreatedDate = DateTime.Parse("2026-07-19 15:47:58.114"), IsActive = true },
                new TbProductReview { ProductReviewId = 3, CustomerId = 3, Detail = "Form đẹp nhưng hơi chật.", Star = 4, ProductId = 2, CreatedDate = DateTime.Parse("2026-07-30 21:06:42.389"), IsActive = true }
            );

            // Seed Data for tb_OrderStatus
            modelBuilder.Entity<TbOrderStatus>().HasData(
                new TbOrderStatus { OrderStatusId = 1, Name = "Pending", Description = "Chờ xử lý" },
                new TbOrderStatus { OrderStatusId = 3, Name = "Processing", Description = "Đã xử lý" },
                new TbOrderStatus { OrderStatusId = 4, Name = "Shipped", Description = "Đã gửi hàng" },
                new TbOrderStatus { OrderStatusId = 5, Name = "Delivered", Description = "Đã giao" },
                new TbOrderStatus { OrderStatusId = 6, Name = "Canceled", Description = "Đã hủy" },
                new TbOrderStatus { OrderStatusId = 7, Name = "Refunded", Description = "Đã hoàn tiền" },
                new TbOrderStatus { OrderStatusId = 9, Name = "Returned", Description = "Trả hàng" },
                new TbOrderStatus { OrderStatusId = 10, Name = "Failed", Description = "Giao dịch thất bại" }
            );

            // Seed Data for tb_ProductVariant
            modelBuilder.Entity<TbProductVariant>().HasData(
                new TbProductVariant { VariantId = 11, ProductId = 1, ColorId = 1, SizeId = 1, Image = "/files/product1.png", Sku = "ATCN-DEN-S", Price = 250000.00m, PriceSale = 199000.00m, Quantity = 40, IsActive = true },
                new TbProductVariant { VariantId = 12, ProductId = 1, ColorId = 1, SizeId = 2, Image = "/files/big-product2.jpg", Sku = "ATCN-DEN-M", Price = 250000.00m, PriceSale = 199000.00m, Quantity = 35, IsActive = true },
                new TbProductVariant { VariantId = 13, ProductId = 1, ColorId = 1, SizeId = 3, Image = "/files/big-product3.jpg", Sku = "ATCN-DEN-L", Price = 250000.00m, PriceSale = 199000.00m, Quantity = 30, IsActive = true },
                new TbProductVariant { VariantId = 14, ProductId = 1, ColorId = 3, SizeId = 1, Image = "/files/big-product4.jpg", Sku = "ATCN-TRANG-S", Price = 250000.00m, PriceSale = 199000.00m, Quantity = 45, IsActive = true },
                new TbProductVariant { VariantId = 15, ProductId = 1, ColorId = 3, SizeId = 2, Image = "/files/big-product5.jpg", Sku = "ATCN-TRANG-M", Price = 250000.00m, PriceSale = 199000.00m, Quantity = 40, IsActive = true },
                new TbProductVariant { VariantId = 17, ProductId = 2, ColorId = 1, SizeId = 1, Image = "/files/product2.png", Sku = "QJN-DEN-S", Price = 300000.00m, PriceSale = 399000.00m, Quantity = 40, IsActive = true },
                new TbProductVariant { VariantId = 18, ProductId = 2, ColorId = 1, SizeId = 2, Image = "/files/big-product4.jpg", Sku = "QJN-DEN-M", Price = 450000.00m, PriceSale = 399000.00m, Quantity = 35, IsActive = true },
                new TbProductVariant { VariantId = 19, ProductId = 2, ColorId = 1, SizeId = 3, Image = "/files/product1.png", Sku = "QJN-DEN-L", Price = 450000.00m, PriceSale = 399000.00m, Quantity = 30, IsActive = true },
                new TbProductVariant { VariantId = 20, ProductId = 2, ColorId = 2, SizeId = 1, Image = "/files/big-product4.jpg", Sku = "QJN-XANH-S", Price = 450000.00m, PriceSale = 399000.00m, Quantity = 40, IsActive = true },
                new TbProductVariant { VariantId = 21, ProductId = 2, ColorId = 2, SizeId = 2, Image = "/files/big-product5.jpg", Sku = "QJN-XANH-M", Price = 450000.00m, PriceSale = 399000.00m, Quantity = 35, IsActive = true }
            );
        }
    }
}
