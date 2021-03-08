using Bot.Brasileirao.Jogos;
using Bot.Brasileirao.Rodadas;
using Bot.Brasileirao.Services;
using Bot.Brasileirao.Times;
using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Brasileirao.CSV
{
    public class GolsCsvDumper : CsvDumper, IGolsCsvDumper, ISingletonService
    {
        protected override string FileName { get; set; } = "gols.csv";

        private bool HasHeader = true;

        public override Task WriteLines(IEnumerable<Jogo> jogos)
        {
            var records = jogos
                .SelectMany(jogo => new List<(Jogo Jogo, Time Time)> { (jogo, jogo.TimeA), (jogo, jogo.TimeB) })
                .SelectMany(tuple => ExtractFromTime(tuple.Time, tuple.Jogo));

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Encoding = Encoding.UTF8,
                HasHeaderRecord = HasHeader,
            };

            using (var file = CreateStream())
            using (var writer = CreateWritter(file))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(records);
                csv.Flush();
            }

            HasHeader = false;

            return Task.CompletedTask;
        }

        private IEnumerable<GolCsvDto> ExtractFromTime(Time time, Jogo jogo)
        {
            return time
                .Gols
                .Select(gol => new GolCsvDto
                    {
                        Rodada = jogo.Rodada,
                        JogoNumero = jogo.Numero,
                        JogoLocal = jogo.Local,
                        JogoData = jogo.Data,
                        Time = time.Nome,
                        Jogador = gol.Jogador,
                        Tempo = gol.Tempo,
                        Periodo = GolCsvDto.GetPeriodoString(gol.Periodo)
                    });
        }
    }
}
