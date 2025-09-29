using System;
using Gozba_na_klik.Models;
using Gozba_na_klik.Repositories;
using Gozba_na_klik.Repositories.RestaurantRepositories;
using Gozba_na_klik.Services;
using Gozba_na_klik.Services.RestaurantServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Gozba_na_klik.Repositories.AddressRepositories;
using Gozba_na_klik.Services.AddressServices;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Controllers + JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Swagger + X-User-Id auth a Swagger UI-ban
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Gozba_na_klik", Version = "v1" });
    c.AddSecurityDefinition("XUserId", new OpenApiSecurityScheme
    {
        Description = "Temporary auth via X-User-Id header (pl. 1)",
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

// (opcionális) konkrét típus regisztráció törölhető
builder.Services.AddScoped<UsersDbRepository>();

// PostgreSQL
builder.Services.AddDbContext<GozbaNaKlikDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// DI – Users
builder.Services.AddScoped<IUsersRepository, UsersDbRepository>();
builder.Services.AddScoped<IUserService, UserService>();

// DI – Restaurants
builder.Services.AddScoped<IRestaurantRepository, RestaurantDbRepository>();
builder.Services.AddScoped<IRestaurantService, RestaurantService>();

// DI – Addresses (KP7)
builder.Services.AddScoped<IAddressRepository, AddressDbRepository>();
builder.Services.AddScoped<IAddressService, AddressService>();

var app = builder.Build();

// Static files: /assets
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "assets")),
    RequestPath = "/assets"
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// (ajánlott)
app.UseHttpsRedirection();

// CORS
app.UseCors("FrontendPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
