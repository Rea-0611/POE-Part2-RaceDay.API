using Microsoft.AspNetCore.DataProtection;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add this to fix the Docker DataProtection warning
        builder.Services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(@"/app/keys"));

        builder.Services.AddControllersWithViews();
        builder.Services.AddHttpClient();

        // 1. Add Session
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(60);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        // app.UseHttpsRedirection();
        app.UseStaticFiles();

        // 2. Use Session (MUST be before Routing)
        app.UseSession();
        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        // Allow Docker to route traffic to port 8081
        // app.Urls.Add("http://*:8081"); // Docker port binding        app.Urls.Add("http://localhost:8081");

        app.Run();
    }
}