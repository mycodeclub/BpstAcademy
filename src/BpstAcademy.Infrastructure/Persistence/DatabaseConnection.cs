using Microsoft.Extensions.Configuration;
using Npgsql;

namespace BpstAcademy.Infrastructure.Persistence;

/// <summary>
/// Picks the connection string. <c>Database:Target</c> (set by the "Docker DB", "Local DB" and "Live DB" launch profiles)
/// selects <c>ConnectionStrings:{Target}</c>; without a target, <c>ConnectionStrings:Default</c> is used (production, tests).
/// Values live in user secrets locally and in server configuration in production; never in the repo.
/// </summary>
public static class DatabaseConnection
{
    public const string TargetKey = "Database:Target";
    public const string Default = "Default";
    public const string Docker = "Docker";
    public const string Local = "Local";
    public const string Live = "Live";

    private static readonly string[] Targets = [Docker, Local, Live];

    public static (string Name, string ConnectionString) Resolve(IConfiguration configuration)
    {
        var target = configuration[TargetKey];
        var name = string.IsNullOrWhiteSpace(target)
            ? Default
            : Targets.FirstOrDefault(t => t.Equals(target, StringComparison.OrdinalIgnoreCase))
              ?? throw new InvalidOperationException(
                  $"Unknown {TargetKey} '{target}'. Use one of: {string.Join(", ", Targets)}.");

        var connectionString = configuration.GetConnectionString(name);
        if (!string.IsNullOrWhiteSpace(connectionString)) return (name, connectionString);

        throw new InvalidOperationException(name == Default
            ? "Connection string 'Default' is missing. Locally, run with the \"Docker DB\", \"Local DB\" or \"Live DB\" launch profile " +
              "(user secrets load only when ASPNETCORE_ENVIRONMENT=Development). On a server, set the environment variable " +
              "ConnectionStrings__Default or put it in appsettings.Production.json next to the app."
            : $"Connection string '{name}' is missing. Set it once: dotnet user-secrets set \"ConnectionStrings:{name}\" " +
              "\"<connection string>\" --project src/BpstAcademy.Web");
    }

    /// <summary>Host, port and database without credentials, for logs.</summary>
    public static string Describe(string connectionString)
    {
        var b = new NpgsqlConnectionStringBuilder(connectionString);
        return $"{b.Host}:{b.Port}/{b.Database}";
    }
}
