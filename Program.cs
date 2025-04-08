using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.OpenApi.Models;
using SuYolu.Data;

namespace SuYolu;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Servisleri container'a ekle
#if DEBUG
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Bağlantı dizesi 'DefaultConnection' bulunamadı.");
#else
        var connectionString = builder.Configuration.GetConnectionString("ServerConnection") ?? throw new InvalidOperationException("Bağlantı dizesi 'DefaultConnection' bulunamadı.");
#endif
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        // Bu satırı IdentityUser yerine ApplicationUser kullanacak şekilde güncelle
        builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
      .AddEntityFrameworkStores<ApplicationDbContext>();

        // API ve Swagger desteği ekle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "SuYolu API'si",
                Version = "v1",
                Description = "SuYolu uygulaması için API"
            });
        });

        builder.Services.AddControllers();
        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        // HTTP istek hattını yapılandır
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SuYolu API v1"));
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            // Varsayılan HSTS değeri 30 gündür. Üretim senaryoları için bunu değiştirmek isteyebilirsiniz, bkz: https://aka.ms/aspnetcore-hsts
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseAuthorization();

        app.MapStaticAssets();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();
        app.MapRazorPages()
           .WithStaticAssets();

        app.Run();
    }
}
