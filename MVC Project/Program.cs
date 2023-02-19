/*by writing the code below in the PM, EFCore creates models of table from the database and adds it to the given folder (Models)

Scaffold - DbContext "connectionString" Microsoft.EntityFrameworkCore.SqlServer - OutputDir Models*/

using Microsoft.EntityFrameworkCore;
using MVC_Project.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<UnitTestingDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration["SqlConstr"]);
});
// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
