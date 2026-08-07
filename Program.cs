using Microsoft.EntityFrameworkCore;
using San_Pham_Do_An1.Models;
using San_Pham_Do_An1.Services;
using San_Pham_Do_An1.Settings;

namespace San_Pham_Do_An1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddDbContext<WedQuanAoDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("ketnoituongduong"));
            });

            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add<San_Pham_Do_An1.Filters.AdminAuthorizationFilter>();
            }).AddRazorRuntimeCompilation();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSession();
            builder.Services.AddHttpClient();


            builder.Services.Configure<VnPaySettings>(builder.Configuration.GetSection("VnPay"));
            builder.Services.AddScoped<IVnPayService, VnPayService>();

            var app = builder.Build();


            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}