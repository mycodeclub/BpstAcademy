using BpstEdu.Application.Abstractions;
using BpstEdu.Infrastructure.Common;
using BpstEdu.Infrastructure.Identity;
using BpstEdu.Infrastructure.Persistence;
using BpstEdu.Infrastructure.Persistence.Interceptors;
using BpstEdu.Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BpstEdu.Infrastructure;

public static class DependencyInjection
{
    public const string ConnectionStringName = "Default";

    /// <summary>
    /// Registers the database and infrastructure services. The connection string comes from
    /// <c>ConnectionStrings:Default</c> (user secrets locally, environment variable on the server); it is never committed.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ConnectionStringName)
            ?? throw new InvalidOperationException(
                $"Connection string '{ConnectionStringName}' is missing. Locally run: " +
                "dotnet user-secrets set \"ConnectionStrings:Default\" \"Host=localhost;Port=5432;Database=bpstedu;Username=bpst;Password=<from .env>\" --project src/BpstEdu.Web");

        services.AddSingleton<IClock, SystemClock>();
        services.AddScoped<AuditingInterceptor>();
        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            ConfigureNpgsql(options, connectionString);
            options.AddInterceptors(sp.GetRequiredService<AuditingInterceptor>());
        });

        // Users and roles; the sign-in cookie is configured by the web app.
        services.AddIdentity<AppUser, AppRole>(o =>
            {
                o.User.RequireUniqueEmail = true;
                o.Lockout.MaxFailedAccessAttempts = 5;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
        services.AddScoped<IdentitySeeder>();

        services.AddHealthChecks().AddDbContextCheck<AppDbContext>("database");
        return services;
    }

    internal static void ConfigureNpgsql(DbContextOptionsBuilder options, string connectionString) =>
        options
            .UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history"))
            .UseSnakeCaseNamingConvention();
}
