using System;
using Gozba_na_klik.Models;
using Gozba_na_klik.Repositories;
using Gozba_na_klik.Repository;
using Gozba_na_klik.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register User repository
builder.Services.AddScoped<UsersDbRepository>();

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

builder.Services.AddScoped<IUsersRepository, UsersDbRepository>();
builder.Services.AddScoped<IUserService, UserService>();

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


