using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BpstAcademy.Infrastructure.Persistence;

/// <summary>
/// Used only by <c>dotnet ef migrations add</c>, which needs the model but not a real database.
/// Commands that do touch the database (<c>database update</c>, bundles) get the connection string with <c>--connection</c>.
/// </summary>
internal sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>();
        DependencyInjection.ConfigureNpgsql(options, "Host=localhost;Database=bpstedu_design");
        return new AppDbContext(options.Options);
    }
}
