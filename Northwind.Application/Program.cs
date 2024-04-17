using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Northwind.Bll.Interfaces;
using Northwind.Bll.Services;
using Northwind.Data;
using Northwind.Data.Entities;
using NorthwindApp.ConfigureServices;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddLogging(builder => builder.AddConsole());

var connectionString = config["ConnectionStrings:SQL_Server"];

builder.Services.AddDbContext<NorthwindInMemoryDbContext>();
builder.Services.AddDbContext<NorthwindDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDbContext<NorthwindIdentityDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<NorthwindIdentityDbContext>();

builder.Services.AddScoped<IRepository<Employee>, EmployeeRepository>();
builder.Services.AddScoped<IRepository<Category>, CategoryRepository>();
builder.Services.AddScoped<IRepository<Product>, ProductRepository>();
builder.Services.AddScoped<IRepository<Supplier>, SupplierRepository>();

builder.Services.AddScoped<IGuestRepository<Employee>, GuestEmployeeRepository>();
builder.Services.AddScoped<IGuestRepository<Category>, GuestCategoryRepository>();

builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "AspNetCore.Identity.Application";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    options.SlidingExpiration = true;
});

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.ConfigureAutoMapper();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var seedScriptPath = config["SeedScriptPath"];
    var adminScriptPath = config["AdminScriptPath"];
    var url = config["ScriptUrl"];

    var adminEmail = config["AdminEmail"];
    var adminPasswordHash = config["AdminPasswordHash"];
    var adminSecurityStamp = config["AdminSecurityStamp"];
    var adminConcurrencyStamp = config["AdminConcurrencyStamp"];

    await FileDownloader.DownloadScriptFileAsync(url, seedScriptPath);
    SqlScriptGenerator.GenerateAdminScript(adminScriptPath!, adminEmail!, adminPasswordHash!, adminSecurityStamp!, adminConcurrencyStamp!);

    var dbContext = services.GetRequiredService<NorthwindDbContext>();

    var count = 10;

    while (count > 0)
    {
        try
        {
            dbContext.Database.Migrate();
            break;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Database request timed out. " + ex.Message);
            count--;

            if (count == 0)
            {
                throw;
            }

            Thread.Sleep(5000);
        }
    }

    var identityDbContext = services.GetRequiredService<NorthwindIdentityDbContext>();
    identityDbContext.Database.Migrate();

    var inMemoryDbContext = services.GetRequiredService<NorthwindInMemoryDbContext>();
    await inMemoryDbContext.SeedDatabase();

    if (File.Exists(seedScriptPath))
    {
        File.Delete(seedScriptPath);
    }

    if (File.Exists(adminScriptPath))
    {
        File.Delete(adminScriptPath);
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

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

public partial class Program { }