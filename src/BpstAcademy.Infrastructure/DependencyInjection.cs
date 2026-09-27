using BpstAcademy.Application.Abstractions;
using BpstAcademy.Infrastructure.Common;
using BpstAcademy.Infrastructure.Identity;
using BpstAcademy.Infrastructure.Persistence;
using BpstAcademy.Infrastructure.Persistence.Interceptors;
using BpstAcademy.Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BpstAcademy.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Registers the database and infrastructure services. The connection string is chosen by
    /// <see cref="DatabaseConnection.Resolve"/> (user secrets locally, server configuration in production); it is never committed.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var (_, connectionString) = DatabaseConnection.Resolve(configuration);

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
