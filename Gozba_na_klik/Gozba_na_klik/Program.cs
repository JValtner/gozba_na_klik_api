using System;
using AutoMapper;
using Gozba_na_klik.Models;
using Gozba_na_klik.Repositories;
using Gozba_na_klik.Services;
using Gozba_na_klik.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// AutoMapper (scan all assemblies)
builder.Services.AddAutoMapper(cfg => {cfg.AddProfile<MappingProfile>();
});

// Register repositories and services
builder.Services.AddScoped<IUsersRepository, UsersDbRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IRestaurantRepository, RestaurantDbRepository>();
builder.Services.AddScoped<IRestaurantService, RestaurantService>();

builder.Services.AddScoped<IMealAddonsRepository, MealAddonsDbRepository>();
builder.Services.AddScoped<IMealAddonService, MealAddonService>();

builder.Services.AddScoped<IMealsRepository, MealsDbRepository>();
builder.Services.AddScoped<IMealService, MealService>();

builder.Services.AddScoped<IAlergensRepository, AlergensDbRepository>();
builder.Services.AddScoped<IAlergenService, AlergenService>();

builder.Services.AddScoped<IFileService, FileService>();

// Configure PostgreSQL database connection
builder.Services.AddDbContext<GozbaNaKlikDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Controllers with JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Exception handling middleware
builder.Services.AddTransient<ExceptionHandlingMiddleware>();

// Serilog logger
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

var app = builder.Build();

// Middleware pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Serve static files from "assets" directory
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "assets")),
    RequestPath = "/assets"
});

// Swagger in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable CORS
app.UseCors("FrontendPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
