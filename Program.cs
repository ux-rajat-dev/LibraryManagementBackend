using LibraryManagement.Models;
using LibraryManagementSystem.admin.Interfaces;
using LibraryManagementSystem.admin.Services;
using LibraryManagementSystem.AuthModels;
using LibraryManagementSystem.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.SignalR;
using System.Text;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add Controllers
        builder.Services.AddControllers();

        // Get connection string from appsettings.json or environment variables
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        // ✅ Use PostgreSQL instead of MySQL
        builder.Services.AddDbContext<FreedbLibraryManagementContext>(options =>
            options.UseNpgsql(connectionString)
        );

        // Register services
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IAuthorService, AuthorService>();
        builder.Services.AddScoped<IGenreService, GenreService>();
        builder.Services.AddScoped<IBookService, BookService>();
        builder.Services.AddScoped<IBorrowTransactionService, BorrowTransactionService>();

        // JWT Authentication
        var jwtKey = builder.Configuration["Jwt:Key"];
        var key = Encoding.ASCII.GetBytes(jwtKey);

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

        builder.Services.AddAuthorization();

        // Correct CORS policy
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy.WithOrigins(
        "https://librarymanagementbyrajat.netlify.app",
        "http://localhost:5173"
    )
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });

        // Swagger with JWT support
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter 'Bearer {your token}' to authenticate."
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                    new string[] { }
                }
            });
        });

        // Add SignalR
        builder.Services.AddSignalR();

        var app = builder.Build();

        // ✅ Enable Swagger in both Development and Production
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Library Management API v1");
            c.RoutePrefix = "swagger"; 
        });

        // ⚠️ HTTPS redirection can fail on Render, so it’s optional
        // app.UseHttpsRedirection();

        // Apply CORS **before** authentication and SignalR
        app.UseCors("AllowFrontend");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        // Map SignalR hub
        app.MapHub<NotificationHub>("/notificationHub");

        // Simple health check endpoint
        app.MapGet("/", async (FreedbLibraryManagementContext db) =>
        {
            try
            {
                var canConnect = await db.Database.CanConnectAsync();
                if (canConnect)
                    return Results.Ok("✅ API is working fine and connected to PostgreSQL!");
                else
                    return Results.Problem("⚠️ API is running, but cannot connect to PostgreSQL.");
            }
            catch (Exception ex)
            {
                return Results.Problem($"❌ API failed to connect to database: {ex.Message}");
            }
        });

        // ✅ Render assigns a dynamic PORT env variable
        var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
        app.Run($"http://0.0.0.0:{port}");
    }
}

// SignalR Hub
public class NotificationHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}
