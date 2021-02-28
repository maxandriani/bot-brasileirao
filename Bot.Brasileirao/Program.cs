using Bot.Brasileirao.Arbitros;
using Bot.Brasileirao.Bots;
using Bot.Brasileirao.CSV;
using Bot.Brasileirao.Gols;
using Bot.Brasileirao.Jogos;
using Bot.Brasileirao.Rodadas;
using Bot.Brasileirao.Times;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Serilog;
using Serilog.Formatting.Elasticsearch;
using System;
using System.Threading.Tasks;

namespace Bot.Brasileirao
{
    class Program
    {
        static Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args)
                .Build();

            return host.RunAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.SetMinimumLevel(LogLevel.Information);
                    logging.AddSerilog();
                    logging.AddConsole();
                })
                .UseSerilog()
                .ConfigureServices((_, services) =>
                    services.AddHostedService<BotHostedService>()
                            .AddSingleton<IArbitroParser, ArbitroParser>()
                            .AddSingleton<IGolParser, GolParser>()
                            .AddSingleton<IJogoParser, JogoParser>()
                            .AddSingleton<IRodadaParser, RodadaParser>()
                            .AddSingleton<ITimeParser, TimeParser>()
                            .AddSingleton<IArbitrosCsvDumper, ArbitrosCsvDumper>()
                            .AddSingleton<IGolsCsvDumper, GolsCsvDumper>()
                            .AddSingleton<IJogosCsvDumper, JogosCsvDumper>());
    }
}
