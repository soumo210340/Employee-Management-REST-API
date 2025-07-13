using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using EmployeeManagement.Data;
using EmployeeManagement.Services;
using EmployeeManagement.Middleware;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Enable CORS for frontend running on different origin
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        builder =>
        {
            builder.WithOrigins("http://127.0.0.1:5500")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});

// Swagger configuration with JWT support and login example
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Employee Management API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter: Bearer {your JWT token}\n\nExample: Bearer eyJhbGciOiJIUzI1NiIs...",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
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
            new string[] {}
        }
    });
});

// Database configuration
builder.Services.AddDbContext<EmployeeDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

// JWT Configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];
if (string.IsNullOrEmpty(secretKey))
    throw new InvalidOperationException("JWT SecretKey is missing in configuration.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // Simplify testing
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

builder.Services.AddAuthorization();

// Register services
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add basic health check (fallback version)
builder.Services.AddHealthChecks().AddCheck("Basic", () => HealthCheckResult.Healthy("Service is running"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions {
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
    ),
    RequestPath = ""
});

// Enable CORS for frontend
app.UseCors("AllowFrontend");

// Global exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Map health check endpoint
app.MapHealthChecks("/health");

// Fallback 404 route with JSON
app.Use(async (context, next) =>
{
    await next();
    if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("{ \"error\": \"Endpoint not found\" }");
    }
});

// Seed initial data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<EmployeeDbContext>();

    // Smart DB seeding - Add default admin only if it doesn't exist
    if (!context.Employees.Any(e => e.Username == "admin"))
    {
        context.Employees.Add(new EmployeeManagement.Models.Employee
        {
            Username = "admin",
            Email = "admin@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            FirstName = "Admin",
            LastName = "User",
            Department = "Administration",
            Position = "Administrator",
            Salary = 0,
            HireDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true
        });
        context.SaveChanges();
    }

    // ✅ Test database connection and show name
    try
    {
        var canConnect = context.Database.CanConnect();
        var dbName = context.Database.GetDbConnection().Database;

        if (canConnect)
        {
            Console.WriteLine($"✅ Connected successfully to database: {dbName}");
        }
        else
        {
            Console.WriteLine($"❌ Failed to connect to database: {dbName}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Exception while checking database connection: {ex.Message}");
    }
}

app.Run();
// Run the application
Console.WriteLine("Employee Management API is running...");
