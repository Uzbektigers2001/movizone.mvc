using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using MovizoneApp.Application.Interfaces;
using MovizoneApp.Application.Mappings;
using MovizoneApp.Application.Services;
using MovizoneApp.Core.Interfaces;
using MovizoneApp.Data;
using MovizoneApp.Infrastructure.Repositories;
using MovizoneApp.Infrastructure.Services;
using MovizoneApp.Middleware;
using MovizoneApp.Services;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build())
    .Enrich.FromLogContext()
    .CreateLogger();

try
{
    Log.Information("Starting Movizone application");

    var builder = WebApplication.CreateBuilder(args);

    // Add Serilog
    builder.Host.UseSerilog();

    // Add DbContext - Choose between PostgreSQL and InMemory based on environment
    // Set USE_POSTGRES=true environment variable to use PostgreSQL
    var usePostgres = builder.Configuration.GetValue<bool>("USE_POSTGRES", false);

    if (usePostgres)
    {
        Log.Information("Using PostgreSQL database");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorCodesToAdd: null)));
    }
    else
    {
        Log.Information("Using InMemory database (set USE_POSTGRES=true to use PostgreSQL)");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("MovizoneDb"));
    }

    // Add JWT Authentication
    var jwtSettings = builder.Configuration.GetSection("Jwt");
    var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";
    var key = Encoding.ASCII.GetBytes(secretKey);

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

    builder.Services.AddAuthorization();

    // Add AutoMapper
    builder.Services.AddAutoMapper(typeof(MappingProfile));

    // Add session support (for backward compatibility with existing views)
    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromHours(2);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

    // Add HttpContextAccessor for LocalizationService
    builder.Services.AddHttpContextAccessor();

    // Add Localization Service
    builder.Services.AddSingleton<ILocalizationService, LocalizationService>();

    // Register Repositories (Infrastructure Layer)
    builder.Services.AddScoped<IMovieRepository, MovieRepository>();
    builder.Services.AddScoped<ITVSeriesRepository, TVSeriesRepository>();
    builder.Services.AddScoped<IActorRepository, ActorRepository>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
    builder.Services.AddScoped<IWatchlistRepository, WatchlistRepository>();
    builder.Services.AddScoped<IPricingPlanRepository, PricingPlanRepository>();

    // Register Infrastructure Services
    builder.Services.AddScoped<IJwtService, JwtService>();

    // Register Application Services (Business Logic Layer)
    builder.Services.AddScoped<IMovieApplicationService, MovieApplicationService>();
    builder.Services.AddScoped<ITVSeriesApplicationService, TVSeriesApplicationService>();
    builder.Services.AddScoped<IActorApplicationService, ActorApplicationService>();
    builder.Services.AddScoped<IUserApplicationService, UserApplicationService>();
    builder.Services.AddScoped<IReviewApplicationService, ReviewApplicationService>();
    builder.Services.AddScoped<IWatchlistApplicationService, WatchlistApplicationService>();
    builder.Services.AddScoped<IPricingApplicationService, PricingApplicationService>();

    // Keep old services for backward compatibility (will be removed after migration)
    builder.Services.AddSingleton<IMovieService, MovieService>();
    builder.Services.AddSingleton<ITVSeriesService, TVSeriesService>();
    builder.Services.AddSingleton<IActorService, ActorService>();
    builder.Services.AddSingleton<IPricingService, PricingService>();
    builder.Services.AddSingleton<IUserService, UserService>();
    builder.Services.AddSingleton<IReviewService, ReviewService>();
    builder.Services.AddSingleton<ICommentService, CommentService>();
    builder.Services.AddSingleton<IWatchlistService, WatchlistService>();
    builder.Services.AddSingleton<IEpisodeService, EpisodeService>();
    builder.Services.AddSingleton<ISiteSettingsService, SiteSettingsService>();

    // Add Controllers with Views
    builder.Services.AddControllersWithViews();

    // Add CORS (for API endpoints if needed)
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    var app = builder.Build();

    // Seed database
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var logger = services.GetRequiredService<ILogger<Program>>();
            var seeder = new DbSeeder(context, logger);
            await seeder.SeedAsync();
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating or seeding the database");
        }
    }

    // Configure the HTTP request pipeline
    app.UseMiddleware<ErrorHandlingMiddleware>();

    if (!app.Environment.IsDevelopment())
    {
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseCors("AllowAll");

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseSession();

    app.UseSerilogRequestLogging();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    Log.Information("Movizone application started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
