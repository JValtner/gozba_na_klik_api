using System.Security.Claims;
using System.Text;
using Gozba_na_klik.Data;
using Gozba_na_klik.Hubs;
using Gozba_na_klik.Models;
using Gozba_na_klik.Repositories;
using Gozba_na_klik.Services;
using Gozba_na_klik.Services.AddressServices;
using Gozba_na_klik.Services.EmailServices;
using Gozba_na_klik.Services.OrderAutoAssignerServices;
using Gozba_na_klik.Services.Pdf;
using Gozba_na_klik.Services.Reporting;
using Gozba_na_klik.Services.Snapshots;
using Gozba_na_klik.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------
// Serilog logger
// ---------------------------
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddSerilog(logger);

// ---------------------------
// PostgreSQL & DbContext
// ---------------------------
builder.Services.AddDbContext<GozbaNaKlikDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ---------------------------
// SMTP
// ---------------------------
builder.Services.Configure<SmtpSettings>(
    builder.Configuration.GetSection("SmtpSettings"));

// ---------------------------
// Identity setup
// ---------------------------
builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 10;
})
.AddEntityFrameworkStores<GozbaNaKlikDbContext>()
.AddDefaultTokenProviders();

// ---------------------------
// Add data protection
// ---------------------------
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "Keys")))
    .SetApplicationName("GozbaNaKlik"); // consistent app name


// ---------------------------
// Authentication JWT
// ---------------------------
builder.Services.AddAuthentication(options =>
{ // Naglašavamo da koristimo JWT
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
    };
    
    // Dozvoli zahtevima bez tokena ili sa nevažećim tokenom da prođu dalje za PublicPolicy endpoint-e
    options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
    {
        OnChallenge = context =>
        {
            // Proveri da li endpoint koristi PublicPolicy ili AllowAnonymous
            var endpoint = context.HttpContext.GetEndpoint();
            if (endpoint != null)
            {
                var allowAnonymous = endpoint.Metadata?.GetMetadata<Microsoft.AspNetCore.Authorization.IAllowAnonymous>();
                var authorizeData = endpoint.Metadata?.GetMetadata<Microsoft.AspNetCore.Authorization.IAuthorizeData>();
                
                if (allowAnonymous != null || (authorizeData != null && authorizeData.Policy == "PublicPolicy"))
                {
                    context.HandleResponse();
                    return Task.CompletedTask;
                }
            }
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            // Proveri da li endpoint koristi PublicPolicy ili AllowAnonymous
            var endpoint = context.HttpContext.GetEndpoint();
            if (endpoint != null)
            {
                var allowAnonymous = endpoint.Metadata?.GetMetadata<Microsoft.AspNetCore.Authorization.IAllowAnonymous>();
                var authorizeData = endpoint.Metadata?.GetMetadata<Microsoft.AspNetCore.Authorization.IAuthorizeData>();
                
                if (allowAnonymous != null || (authorizeData != null && authorizeData.Policy == "PublicPolicy"))
                {
                    context.NoResult();
                    return Task.CompletedTask;
                }
            }
            return Task.CompletedTask;
        }
    };
});

// Optional: cookie auth
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/api/account/login";
    options.AccessDeniedPath = "/api/account/accessdenied";
});

// ---------------------------
// Authorization Policies
// ---------------------------
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("PublicPolicy", policy => 
        policy.RequireAssertion(_ => true));

    options.AddPolicy("RegisteredPolicy", policy =>
        policy.RequireAssertion(context =>
            context.User.IsInRole("RestaurantOwner") || context.User.IsInRole("User") || context.User.IsInRole("Admin") || context.User.IsInRole("RestaurantEmployee") || context.User.IsInRole("DeliveryPerson")
        ));

    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("RestaurantOwnerPolicy", policy =>
        policy.RequireRole("RestaurantOwner"));

    options.AddPolicy("OwnerOrUserPolicy", policy =>
        policy.RequireAssertion(context =>
            context.User.IsInRole("RestaurantOwner") || context.User.IsInRole("User")
        ));

    options.AddPolicy("OwnerOrAdminPolicy", policy =>
        policy.RequireAssertion(context =>
            context.User.IsInRole("RestaurantOwner") || context.User.IsInRole("Admin")
        ));

    options.AddPolicy("EmployeePolicy", policy =>
        policy.RequireRole("RestaurantEmployee"));

    options.AddPolicy("EmployeeOrAdminPolicy", policy =>
        policy.RequireAssertion(context =>
            context.User.IsInRole("RestaurantEmployee") || context.User.IsInRole("Admin")
        ));

    options.AddPolicy("DeliveryPerson", policy =>
        policy.RequireRole("DeliveryPerson"));

    options.AddPolicy("UserPolicy", policy =>
        policy.RequireRole("User"));
});

// ---------------------------
// AutoMapper
// ---------------------------
builder.Services.AddAutoMapper(cfg => { cfg.AddProfile<MappingProfile>(); });

// ---------------------------
// Repositories & Services
// ---------------------------
builder.Services.AddScoped<IOrderRepository, OrderDbRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddScoped<IDeliveryPersonScheduleRepository, DeliveryPersonScheduleRepository>();
builder.Services.AddScoped<IDeliveryPersonScheduleService, DeliveryPersonScheduleService>();

builder.Services.AddScoped<IEmployeeService, EmployeeService>();

builder.Services.AddScoped<IUsersRepository, UsersDbRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<ISuspensionRepository, SuspensionRepository>();
builder.Services.AddTransient<IEmailService, SmtpEmailService>();

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

builder.Services.AddScoped<IReviewsRepository, ReviewsDbRepository>();
builder.Services.AddScoped<IReviewService, ReviewService>();

builder.Services.AddScoped<IPdfService, PdfService>();

builder.Services.AddScoped<IComplaintRepository, ComplaintRepository>();
builder.Services.AddScoped<IComplaintService, ComplaintService>();

// Invoice services - MongoDB
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();

builder.Services.AddScoped<IFileService, FileService>();

builder.Services.AddScoped<IOrderAutoAssignerService, OrderAutoAssignerService>();
builder.Services.AddHostedService<OrderAutoAssignerBackgroundService>();

builder.Services.AddScoped<IReportingRepository, ReportingRepository>();
builder.Services.AddScoped<MonthlyReportBuilder>();
builder.Services.AddScoped<IReportingService, ReportingService>();
builder.Services.AddScoped<IPdfRenderer, QuestPdfRenderer>();
builder.Services.AddScoped<IPdfReportService, PdfReportService>();
builder.Services.AddScoped<IPdfReportRepository, PdfReportRepository>();
builder.Services.AddHostedService<PdfSnapshotScheduler>();

// SignalR
builder.Services.AddSignalR();

// ---------------------------
// Configure MongoDb
// ---------------------------
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("MongoDb")
        ?? "mongodb://localhost:27017";
    return new MongoClient(connectionString);
});

builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var databaseName = builder.Configuration["MongoDb:DatabaseName"] ?? "gozba_na_klik_invoices";
    return client.GetDatabase(databaseName);
});

// ---------------------------
// CORS
// ---------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ---------------------------
// Controllers & JSON options
// ---------------------------
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// ---------------------------
// Swagger
// ---------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Building Example API", Version = "v1" });

    // Definisanje JWT Bearer autentifikacije
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insert JWT token"
    });

    // Primena Bearer autentifikacije
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
  {
    {
      new OpenApiSecurityScheme
      {
        Reference = new OpenApiReference
        {
          Type = ReferenceType.SecurityScheme,
          Id = "Bearer"
        }
      },
      Array.Empty<string>()
    }
  });
});

// ---------------------------
// Exception handling middleware
// ---------------------------
builder.Services.AddTransient<ExceptionHandlingMiddleware>();

var app = builder.Build();

// Middleware pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();

// SignalR
app.MapHub<CourierLocationHub>("/locationHub");

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

// HTTPS & CORS
// Only redirect to HTTPS in production, not in development
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseCors("FrontendPolicy");

app.UseAuthentication(); // <--- Identity
app.UseAuthorization();

app.MapControllers();

// ---------------------------
// Seed Identity Roles & Users
// ---------------------------
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
    var startupLogger = services.GetRequiredService<ILogger<Program>>();

    await DataSeeder.SeedAsync(
        services.GetRequiredService<GozbaNaKlikDbContext>(),
        services.GetRequiredService<UserManager<User>>(),
        roleManager,
        startupLogger);

    await RoleValidator.ValidateRolesAsync(roleManager, startupLogger);
}

app.Run();
