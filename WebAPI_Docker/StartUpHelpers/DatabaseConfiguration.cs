using Microsoft.EntityFrameworkCore;
using WebAPI_Docker.Data;

namespace WebAPI_Docker.StartUpHelpers;

public static class DatabaseConfiguration
{
    public static async void MigrateDatabase(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<WebApiDockerDbContext>();

        var maximumRetries = 120;
        var retryCount = 0;
        while (retryCount++ < maximumRetries)
        {
            try
            {
                context.Database.CanConnect();
                break;
            }
            catch (Exception)
            {
            }
            Console.WriteLine("Waiting for database to be ready");
            Thread.Sleep(1000);
        }

        try
        {
            await context.Database.MigrateAsync();

            if (context.Departments.Any())
                return;

            var seeder = serviceProvider.GetRequiredService<IDatabaseSeeder>();
            await seeder.Seed();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }
}
