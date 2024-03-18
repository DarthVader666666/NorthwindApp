using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Northwind.Data;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = config["ConnectionStrings:Azure_SQL_Server"];

//builder.Services.AddDbContext<NorthwindDbContext>();
builder.Services.AddDbContext<NorthwindDbContext>(options => options.UseSqlServer(connectionString));



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(jwtBearerOptions =>
    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
    { 
        ValidIssuer = config["Issuer"],
        ValidAudience = config["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["SecretKey"]!)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true
    }
);

builder.Services.AddAuthorization(authOptions => 
    authOptions.AddPolicy("Admin", policy =>
        policy.RequireClaim(ClaimTypes.Role, "admin"))    
    );

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<NorthwindDbContext>();

    //context.Database.EnsureCreated();

    context.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();
