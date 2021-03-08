using Bot.Brasileirao.Jogos;
using Bot.Brasileirao.Services;
using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Brasileirao.CSV
{
    public class JogosCsvDumper : CsvDumper, IJogosCsvDumper, ISingletonService
    {
        protected override string FileName { get; set; } = "jogos.csv";

        private bool HasHeader = true;

        public override Task WriteLines(IEnumerable<Jogo> jogos)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Encoding = Encoding.UTF8,
                HasHeaderRecord = HasHeader,
            };

            using (var file = CreateStream())
            using (var writer = CreateWritter(file))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(jogos
                    .Select(jogo => new JogoCsvDto
                        {
                            Rodada = jogo.Rodada,
                            Data = jogo.Data,
                            Local = jogo.Local,
                            Numero = jogo.Numero,
                            TimeA = jogo.TimeA.Nome,
                            TimeB = jogo.TimeB.Nome,
                            TotalGolsTimeA = jogo.TotalGolsTimeA,
                            TotalGolsTimeB = jogo.TotalGolsTimeB,
                            Vencedor = jogo.Vencedor?.Nome
                        }));

                csv.Flush();
            }

            HasHeader = false;

            return Task.CompletedTask;
        }
    }
}
