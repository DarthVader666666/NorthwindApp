using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Northwind.Application.Services;
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
builder.Services.AddSession();

string? connectionString = config["ConnectionStrings:SQL_Server"];

if (connectionString == null)
{
    builder.Services.AddDbContext<NorthwindDbContext>(options => options.UseInMemoryDatabase("Northwind_Azure"));
}
else
{
    builder.Services.AddDbContext<NorthwindDbContext>(options => options.UseSqlServer(connectionString));
}

builder.Services.AddDefaultIdentity<NorthwindUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<NorthwindDbContext>();

builder.Services.AddScoped<IRepository<Employee>, EmployeeRepository>();
builder.Services.AddScoped<IRepository<Category>, CategoryRepository>();
builder.Services.AddScoped<IRepository<Product>, ProductRepository>();
builder.Services.AddScoped<IRepository<Supplier>, SupplierRepository>();
builder.Services.AddScoped<IRepository<Customer>, CustomerRepository>();
builder.Services.AddScoped<IRepository<Order>, OrderRepository>();
builder.Services.AddScoped<IRepository<OrderDetail>, OrderDetailRepository>();
builder.Services.AddScoped<IRepository<Shipper>, ShipperRepository>();

builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddScoped<ISelectListFiller, SelectListFiller>();

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
    var dbContext = services.GetRequiredService<NorthwindDbContext>();

    if (app.Environment.EnvironmentName == "Production")
    {
        await dbContext.SeedDatabase();
    }
    else
    {
        await Migrate(dbContext, config);
    }
}

async Task Migrate(NorthwindDbContext dbContext, ConfigurationManager config)
{
    var seedScriptPath = config["SeedScriptPath"];
    var ownerScriptPath = config["OwnerScriptPath"];
    var url = config["ScriptUrl"];

    var ownerEmail = config["OwnerEmail"];
    var ownerPasswordHash = config["OwnerPasswordHash"];
    var ownerSecurityStamp = config["OwnerSecurityStamp"];
    var ownerConcurrencyStamp = config["OwnerConcurrencyStamp"];

    await FileDownloader.DownloadScriptFileAsync(url, seedScriptPath);
    SqlScriptGenerator.GenerateOwnerScript(ownerScriptPath!, ownerEmail!, ownerPasswordHash!, ownerSecurityStamp!, ownerConcurrencyStamp!);

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

    if (File.Exists(seedScriptPath))
    {
        File.Delete(seedScriptPath);
    }

    if (File.Exists(ownerScriptPath))
    {
        File.Delete(ownerScriptPath);
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
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
    );

app.Run();

public partial class Program { }