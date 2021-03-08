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
    public class ArbitrosCsvDumper : CsvDumper, IArbitrosCsvDumper, ISingletonService
    {
        protected override string FileName { get; set; } = "arbitros.csv";

        private bool HasHeader = true;

        public override Task WriteLines(IEnumerable<Jogo> jogos)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                // Don't write the header again.
                Encoding = Encoding.UTF8,
                HasHeaderRecord = HasHeader,
                // HasHeaderRecord = true,
            };

            var records = jogos
                .SelectMany(jogo => jogo.Arbitros.Select(arbitro => new { jogo, arbitro }))
                .Select(line => new ArbitroCsvDto
                {
                    Rodada = line.jogo.Rodada,
                    JogoLocal = line.jogo.Local,
                    JogoData = line.jogo.Data,
                    JogoNumero = line.jogo.Numero,
                    JogoTimeA = line.jogo.TimeA.Nome,
                    JogoTimeB = line.jogo.TimeB.Nome,
                    Nome = line.arbitro.Nome,
                    Funcao = line.arbitro.Funcao,
                    Categoria = line.arbitro.Categoria,
                    Federacao = line.arbitro.Federacao
                });

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
    }
}
