using MCBA.Data;
using MCBA.Filter;
using MCBA.Services;
using Microsoft.EntityFrameworkCore;
using Data;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MCBAContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("MCBAContext"), b => b.MigrationsAssembly("MCBA")));




builder.Services.AddHostedService<BillPayBackgroundService>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "MCBA.Session";

    // Make the session cookie essential.
    options.Cookie.IsEssential = true;
});
// Add services to the container.
builder.Services.AddControllersWithViews(options => options.Filters.Add(new AuthorizeLoginAttribute()));
builder.Services.AddScoped<AccountServices>();


var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseSession();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
