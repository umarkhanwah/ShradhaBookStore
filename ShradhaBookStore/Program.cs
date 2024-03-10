using Microsoft.EntityFrameworkCore;
using ShradhaBookStore.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var provider = builder.Services.BuildServiceProvider();
var config  = provider.GetRequiredService<IConfiguration>();
builder.Services.AddDbContext<ShradhaBookStoreContext>(item => item.UseSqlServer(config.GetConnectionString("dbcs")));
builder.Services.AddSession();
var app = builder.Build();

//Call this function for login register
app.UseSession();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Index}/{id?}");

app.Run();
