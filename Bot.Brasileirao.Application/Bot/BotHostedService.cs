using Bot.Brasileirao.CSV;
using Bot.Brasileirao.Jogos;
using Bot.Brasileirao.Rodadas;
using HtmlAgilityPack;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.Brasileirao.Bot
{
    public class BotHostedService : BackgroundService, IHostedService
    {
        private Queue<(ushort Rodada, string JogoUri)> FilaDeJogos = new Queue<(ushort, string)>();

        private readonly ILogger<BotHostedService> logger;
        private readonly IRodadaParser rodadaParser;
        private readonly IJogoParser jogoParser;
        private readonly IGolsCsvDumper golsCsvDumper;
        private readonly IJogosCsvDumper jogosCsvDumper;
        private readonly IArbitrosCsvDumper arbitrosCsvDumper;
        private readonly IOptions<BotConfig> config;

        public BotHostedService(ILogger<BotHostedService> logger,
                                IOptions<BotConfig> options,
                                IRodadaParser rodadaParser,
                                IJogoParser jogoParser,
                                IGolsCsvDumper golsCsvDumper,
                                IJogosCsvDumper jogosCsvDumper,
                                IArbitrosCsvDumper arbitrosCsvDumper)
        {
            this.config = options;
            this.logger = logger;
            this.rodadaParser = rodadaParser;
            this.jogoParser = jogoParser;
            this.golsCsvDumper = golsCsvDumper;
            this.jogosCsvDumper = jogosCsvDumper;
            this.arbitrosCsvDumper = arbitrosCsvDumper;
        }

        public BotHostedService FromUri(string uri)
        {
            config.Value.Uri = uri;
            return this;
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            if (String.IsNullOrEmpty(config.Value.Uri))
            {
                logger.LogError($"Uri do campeonato não foi informado! Encerrando importação.");
                return;
            }

            try
            {
                var htmlWeb = new HtmlWeb();
                var rodadaDocument = await htmlWeb.LoadFromWebAsync(config.Value.Uri);

                var rodadas = rodadaParser.ParseRodadas(rodadaDocument);

                arbitrosCsvDumper.SetupOutDir(config.Value.OutDir);
                golsCsvDumper.SetupOutDir(config.Value.OutDir);
                jogosCsvDumper.SetupOutDir(config.Value.OutDir);

                foreach (var item in rodadas
                    .SelectMany(rodada => rodada.Jogos.Select(jogo => (rodada.Numero, jogo)))
                    .ToArray())
                {
                    FilaDeJogos.Enqueue(item);
                }

                while (!HasFinished() && !cancellationToken.IsCancellationRequested)
                {
                    await RunNext();
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    logger.LogInformation("Cancelamento solicitado! Desligando robôs graciosamente.");
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
            var tasks = new List<Task<Jogo>>();
            var limit = 1;

            while (!HasFinished() && limit <= config.Value.Bots && FilaDeJogos.TryDequeue(out var item))
            {
                tasks.Add(ExcractGameInfo(item.Rodada, item.JogoUri));
                limit++;
            }

            limit = 0;
            var jogos = (await Task.WhenAll(tasks))
                .Where(jogo => jogo != null);

            logger.LogInformation($"Gravando CSV...");
            await Task.WhenAll(arbitrosCsvDumper.WriteLines(jogos),
                               golsCsvDumper.WriteLines(jogos),
                               jogosCsvDumper.WriteLines(jogos));
        }

        private async Task<Jogo> ExcractGameInfo(ushort rodada, string jogoUri)
        {
            try
            {
                logger.LogInformation($"Iniciando extração da Rodada {rodada}, Jogo {jogoUri}");
                var htmlWeb = new HtmlWeb();
                var jogoDocument = await htmlWeb.LoadFromWebAsync(jogoUri);

                var jogo = jogoParser.ParseJogo(jogoDocument);
                jogo.Rodada = rodada;

                logger.LogInformation($"Extração do Jogo {jogo} concluído.");

                return jogo;
            } catch (Exception ex)
            {
                logger.LogError($"Erro ao processar jogo {ex.Message}", ex, rodada, jogoUri);
                // FilaDeJogos.Enqueue((rodada, jogoUri));
            }

            return null;
        }

        public BotHostedService WithTreads(ushort threads)
        {
            config.Value.Bots = threads;
            return this;
        }

        public BotHostedService WithOutDir(string outDir)
        {
            config.Value.OutDir = outDir;
            return this;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                Console.Clear();
                Console.WriteLine("Bem Vindo ao importador do Brasileirão!");
                Console.WriteLine("Informe a URI do campeonato: ");
                FromUri(Console.ReadLine().Trim());
                Console.WriteLine($"Informe o número de robôs simultâneos: (Padrão: {config.Value.Bots})");
                var threads = Console.ReadLine();

                if (!string.IsNullOrEmpty(threads))
                {
                    WithTreads(ushort.Parse(threads));
                }

                Console.WriteLine($"Informe a pasta de destino dos arquivos (Padrão: {config.Value.OutDir})");

                var outDir = Console.ReadLine();

                if (!string.IsNullOrEmpty(outDir))
                {
                    WithOutDir(outDir);
                }

                Console.WriteLine("Iniciando baixa....");
                Console.WriteLine("Pressione Ctrl + C para encerrar o processo!");
                await Run(stoppingToken);
                Console.WriteLine("Baixa Concluída!");
            }
            while (ShouldContinue());

            _ = StopAsync(stoppingToken);
        }

        private bool ShouldContinue()
        {
            Console.WriteLine("Pressione 1 para continuar, ou enter para encerrar!");
            return Console.ReadLine() == "1";
        }
    }
}
