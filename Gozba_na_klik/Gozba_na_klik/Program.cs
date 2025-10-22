using System;
using AutoMapper;
using Gozba_na_klik.Models;
using Gozba_na_klik.Repositories;
using Gozba_na_klik.Services;
using Gozba_na_klik.Services.AddressServices;
using Gozba_na_klik.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Gozba_na_klik", Version = "v1" });
    c.AddSecurityDefinition("XUserId", new OpenApiSecurityScheme
    {
        Description = "Temporary auth via X-User-Id header (e.g. 1)",
        Name = "X-User-Id",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "XUserId" }
            },
            Array.Empty<string>()
        }
    });
});



// AutoMapper (scan all assemblies)
builder.Services.AddAutoMapper(cfg => {cfg.AddProfile<MappingProfile>();
});

// Register repositories and services
builder.Services.AddScoped<IOrderRepository, OrderDbRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddScoped<IDeliveryPersonScheduleRepository, DeliveryPersonScheduleRepository>();
builder.Services.AddScoped<IDeliveryPersonScheduleService, DeliveryPersonScheduleService>();

builder.Services.AddScoped<IEmployeeService, EmployeeService>();

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

builder.Services.AddScoped<IAddressRepository, AddressDbRepository>();
builder.Services.AddScoped<IAddressService, AddressService>();

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


// Static files: /assets
var assetsPath = Path.Combine(builder.Environment.ContentRootPath, "assets");
if (!Directory.Exists(assetsPath)) Directory.CreateDirectory(assetsPath);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(assetsPath),
    RequestPath = "/assets"
});

// Swagger in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable CORS
app.UseHttpsRedirection();

app.UseCors("FrontendPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
