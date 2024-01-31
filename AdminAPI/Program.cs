using AdminAPI.Repository;
using Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<MCBAContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MCBAContext"), b => b.MigrationsAssembly("MCBA")));
builder.Services.AddScoped<CustomerManager>();


var app = builder.Build();



// Configure the HTTP request pipeline.

app.UseHttpsRedirection();


app.MapControllers();

app.Run();
