using Bot.Brasileirao.CSV;
using Bot.Brasileirao.Jogos;
using Bot.Brasileirao.Rodadas;
using HtmlAgilityPack;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.Brasileirao.Bots
{
    public class BotHostedService : BackgroundService, IHostedService
    {
        private int totalTreads { get; set; } = 12;
        private string uri { get; set; } = "";
        private Queue<(ushort Rodada, string JogoUri)> FilaDeJogos = new Queue<(ushort, string)>();

        private readonly ILogger<BotHostedService> logger;
        private readonly IRodadaParser rodadaParser;
        private readonly IJogoParser jogoParser;
        private readonly IGolsCsvDumper golsCsvDumper;
        private readonly IJogosCsvDumper jogosCsvDumper;
        private readonly IArbitrosCsvDumper arbitrosCsvDumper;

        public BotHostedService(ILogger<BotHostedService> logger,
                          IRodadaParser rodadaParser,
                          IJogoParser jogoParser,
                          IGolsCsvDumper golsCsvDumper,
                          IJogosCsvDumper jogosCsvDumper,
                          IArbitrosCsvDumper arbitrosCsvDumper)
        {
            this.logger = logger;
            this.rodadaParser = rodadaParser;
            this.jogoParser = jogoParser;
            this.golsCsvDumper = golsCsvDumper;
            this.jogosCsvDumper = jogosCsvDumper;
            this.arbitrosCsvDumper = arbitrosCsvDumper;
        }

        public BotHostedService FromUri(string uri)
        {
            this.uri = uri;
            return this;
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            if (String.IsNullOrEmpty(uri))
            {
                logger.LogError($"Uri do campeonato não foi informado! Encerrando importação.");
                return;
            }

            try
            {
                var htmlWeb = new HtmlWeb();
                var rodadaDocument = await htmlWeb.LoadFromWebAsync(uri);

                var rodadas = rodadaParser.ParseRodadas(rodadaDocument);

                foreach (var item in rodadas
                    .SelectMany(rodada => rodada.Jogos.Select(jogo => (rodada.Numero, jogo)))
                    .ToArray())
                {
                    FilaDeJogos.Enqueue(item);
                }

                while (!HasFinished())
                {
                    await RunNext();
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Não foi possível conectar à página, {ex.Message}", ex);
                return;
            }
        }

        private bool HasFinished()
        {
            return FilaDeJogos.Count == 0;
        }

        private async Task RunNext()
        {
            var tasks = new List<Task>();
            var limit = 1;

            while (!HasFinished() && limit <= totalTreads && FilaDeJogos.TryDequeue(out var item))
            {
                tasks.Add(ExcractGameInfo(item.Rodada, item.JogoUri));
                limit++;
            }

            limit = 0;
            await Task.WhenAll(tasks);
        }

        private async Task ExcractGameInfo(ushort rodada, string jogoUri)
        {
            try
            {
                logger.LogInformation($"Iniciando extração da Rodada {rodada}, Jogo {jogoUri}");
                var htmlWeb = new HtmlWeb();
                var jogoDocument = await htmlWeb.LoadFromWebAsync(jogoUri);

                var jogo = jogoParser.ParseJogo(jogoDocument);
                jogo.Rodada = rodada;

                logger.LogInformation($"Gravando CSV {jogo}");
                await arbitrosCsvDumper.WriteLine(jogo);
                await golsCsvDumper.WriteLine(jogo);
                await jogosCsvDumper.WriteLine(jogo);

                logger.LogInformation($"Extração do Jogo {jogo} concluído.");
            } catch (Exception ex)
            {
                logger.LogError($"Erro ao processa jogo {ex.Message}", ex, rodada, jogoUri);
            }
        }

        public BotHostedService WithTreads(int threads)
        {
            totalTreads = threads;
            return this;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Bem Vindo ao importador do Brasileirão!");
            Console.WriteLine("Informe a URI do campeonato: ");
            FromUri(Console.ReadLine().Trim());
            Console.WriteLine("Informe o número de robôs simultâneos: ");
            WithTreads(int.Parse(Console.ReadLine()));

            return Run(stoppingToken);
        }
    }
}
