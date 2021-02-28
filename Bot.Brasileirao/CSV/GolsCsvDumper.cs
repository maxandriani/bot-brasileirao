using Bot.Brasileirao.Jogos;
using Bot.Brasileirao.Rodadas;
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
    public class GolsCsvDumper : CsvDumper, IGolsCsvDumper
    {
        protected override string FileName { get; set; } = "gols.csv";

        private bool HasHeader = true;

        public Task WriteLine(Jogo jogo)
        {
            var records = new List<GolCsvModel>();

            records.AddRange(ExtractFromTime(jogo.TimeA, jogo));
            records.AddRange(ExtractFromTime(jogo.TimeB, jogo));

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

        private IList<GolCsvModel> ExtractFromTime(Time time, Jogo jogo)
        {
            return time
                .Gols
                .Select(gol => new GolCsvModel
                    {
                        Rodada = jogo.Rodada,
                        JogoNumero = jogo.Numero,
                        JogoLocal = jogo.Local,
                        JogoData = jogo.Data,
                        Time = time.Nome,
                        Jogador = gol.Jogador,
                        Tempo = gol.Tempo,
                        Periodo = GolCsvModel.GetPeriodoString(gol.Periodo)
                    })
                .ToList();
        }
    }
}
