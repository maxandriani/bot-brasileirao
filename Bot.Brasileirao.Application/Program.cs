using Bot.Brasileirao.Bot;
using Bot.Brasileirao.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.Threading.Tasks;
using System;
using System.IO;

namespace Bot.Brasileirao.Application
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await CreateHostBuilder(args)
                .RunConsoleAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .SetupConfiguration(args)
                .AutoRegisterServices()
                .AddBotHostedService();
    }

    public static class SetupExtensions
    {
        public static IHostBuilder AddBotHostedService(this IHostBuilder builder) => 
            builder.ConfigureServices((context, services) => 
                services.AddHostedService<BotHostedService>());

        public static IHostBuilder SetupConfiguration(this IHostBuilder builder, string[] args) =>
            builder
                .ConfigureHostConfiguration(config =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    if (args != null)
                    {
                        // enviroment from command line
                        // e.g.: dotnet run --environment "Staging"
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureAppConfiguration((context, builder) =>
                    builder
                        .AddJsonFile("appsettings.json", optional: false)
                        .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true)
                        .AddEnvironmentVariables())
                .ConfigureServices((context, services) =>
                {
                    services.Configure<BotConfig>(context.Configuration.GetSection("App"));
                });

        public static IHostBuilder AutoRegisterServices(this IHostBuilder builder) =>
            builder.ConfigureServices((context, services) =>
                services.Scan(scan => scan
                // We start out with all types in the assembly of ITransientService
                .FromAssemblyOf<IService>()
                    .AddClasses(classes => classes.AssignableTo<ITransientService>())
                        .AsMatchingInterface()
                        .WithTransientLifetime()
                    .AddClasses(classes => classes.AssignableTo<IScopedService>())
                        .AsMatchingInterface()
                        .WithScopedLifetime()
                    .AddClasses(classes => classes.AssignableTo<ISingletonService>())
                        .AsMatchingInterface()
                        .WithSingletonLifetime()));
    }
}
