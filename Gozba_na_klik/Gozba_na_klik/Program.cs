using System;
using Gozba_na_klik.Models;
using Gozba_na_klik.Repositories.AlergenRepositories;
using Gozba_na_klik.Repositories.MealAddonsRepositories;
using Gozba_na_klik.Repositories.MealRepositories;
using Gozba_na_klik.Repositories.RestaurantRepositories;
using Gozba_na_klik.Repositories.UserRepositories;
using Gozba_na_klik.Services.AlergenServices;
using Gozba_na_klik.Services.FileServices;
using Gozba_na_klik.Services.MealAddonServices;
using Gozba_na_klik.Services.MealServices;
using Gozba_na_klik.Services.RestaurantServices;
using Gozba_na_klik.Services.UserServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register User repositories and services
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


builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });





var app = builder.Build();
// Serve static files from the "assets" directory
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "assets")),
    RequestPath = "/assets"
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Aktiviraj CORS sa definisanom politikom
app.UseCors("FrontendPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();


