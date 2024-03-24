using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Northwind.Bll.Services;
using Northwind.Data;
using NorthwindApp.ConfigureServices;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddLogging(builder => builder.AddConsole());

var connectionString = config["ConnectionStrings:SQL_Server"];

builder.Services.AddDbContext<NorthwindDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDbContext<NorthwindIdentityDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<NorthwindIdentityDbContext>();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.ConfigureAutoMapper();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var path = app.Configuration["ScriptPath"];
    var url = app.Configuration["ScriptUrl"];

    FileDownloader.DownloadScriptFileAsync(url, path);

    DbContext context = services.GetRequiredService<NorthwindDbContext>();
    context.Database.Migrate();

    context = services.GetRequiredService<NorthwindIdentityDbContext>();
    context.Database.Migrate();

    if (File.Exists(path))
    {
        File.Delete(path);
    }
}

// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
//}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
