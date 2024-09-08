using Microsoft.EntityFrameworkCore;
using SmartElectronicsApi.DataAccess.Data;
using SmartElectronicsApi.Api;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var config = builder.Configuration;

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<SmartElectronicsDbContext>(options =>
    options.UseSqlServer(config.GetConnectionString("AppConnectionString"))
);

// Register custom services
builder.Services.Register(config);

// Swagger configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
