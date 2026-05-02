using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Order.Infrastructure.Persistence;

// Used by EF Core CLI tooling:
// dotnet ef migrations add <Name> --project src/Order.Infrastructure --startup-project src/Order.Api
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=asyncordersdb;Username=postgres;Password=postgres")
            .Options;

        return new AppDbContext(options);
    }
}
