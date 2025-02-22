using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using PopulationGame.Interfaces;
using PopulationGame.Repositories;

namespace PopulationGame;

class Program
{
    public static async Task Main(string[] args)
    {
        using IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((hostContext, services) =>
            {
                string connectionString = hostContext.Configuration.GetConnectionString("PopulationGame");
                services.AddSingleton<IDbConnectionFactory>(new SqlDbConnectionFactory(connectionString));
                services.AddSingleton<DapperContext>();
                services.AddTransient<IUserRepository, UserRepository>();
                services.AddTransient<ICountryRepository, CountryRepository>();
                services.AddTransient<IUserGameLogRepository, UserGameLogRepository>();
                services.AddTransient<GameRunner>();
            })
            .Build();

        var gameRunner = host.Services.GetRequiredService<GameRunner>();
        gameRunner.Run();

        await host.RunAsync();
    }

}
