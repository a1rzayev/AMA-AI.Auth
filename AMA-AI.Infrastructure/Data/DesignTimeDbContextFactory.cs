using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AMA_AI.Infrastructure.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        optionsBuilder.UseSqlServer("Server=localhost,1433;Database=amaai_localdb;User Id=sa;Password=2411200a.;TrustServerCertificate=True;");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}