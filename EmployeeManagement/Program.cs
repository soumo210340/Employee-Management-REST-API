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

public class EmployeeManagementApp
{
    private readonly WebApplication _app;

    public EmployeeManagementApp(WebApplication app)
    {
        _app = app;
    }

    public void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policyBuilder =>
            {
                policyBuilder.WithOrigins("http://127.0.0.1:5500")
                             .AllowAnyHeader()
                             .AllowAnyMethod()
                             .AllowCredentials();
            });
        });

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

        builder.Services.AddDbContext<EmployeeDbContext>(options =>
            options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
            ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
                };
            });

        builder.Services.AddAuthorization();
        builder.Services.AddScoped<IEmployeeService, EmployeeService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();
        builder.Services.AddHealthChecks().AddCheck("Basic", () => HealthCheckResult.Healthy("Service is running"));
    }

    public void ConfigureMiddleware()
    {
        if (_app.Environment.IsDevelopment())
        {
            _app.UseSwagger();
            _app.UseSwaggerUI();
        }

        _app.UseHttpsRedirection();
        _app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
            RequestPath = ""
        });

        _app.UseCors("AllowFrontend");
        _app.UseMiddleware<ExceptionHandlingMiddleware>();
        _app.UseAuthentication();
        _app.UseAuthorization();
        _app.MapControllers();
        _app.MapHealthChecks("/health");
        _app.Use(async (context, next) =>
        {
            await next();
            if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
            {
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{ \"error\": \"Endpoint not found\" }");
            }
        });
    }

    public void SeedData()
    {
        using (var scope = _app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<EmployeeDbContext>();

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
    }
}

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
var employeeManagementApp = new EmployeeManagementApp(app);

employeeManagementApp.ConfigureServices(builder);
employeeManagementApp.ConfigureMiddleware();
employeeManagementApp.SeedData();

app.Run();
// Run the application
Console.WriteLine("Employee Management API is running...");
