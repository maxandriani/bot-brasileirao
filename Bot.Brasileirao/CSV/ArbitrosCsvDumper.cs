using Bot.Brasileirao.Jogos;
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
    public class ArbitrosCsvDumper : CsvDumper, IArbitrosCsvDumper
    {
        protected override string FileName { get; set; } = "arbitros.csv";

        private bool HasHeader = true;

        public Task WriteLine(Jogo jogo)
        {
            var records = jogo.Arbitros
                .Select(arbitro => new ArbitroCsvModel
                {
                    Rodada = jogo.Rodada,
                    JogoLocal = jogo.Local,
                    JogoData = jogo.Data,
                    JogoNumero = jogo.Numero,
                    JogoTimeA = jogo.TimeA.Nome,
                    JogoTimeB = jogo.TimeB.Nome,
                    Nome = arbitro.Nome,
                    Funcao = arbitro.Funcao,
                    Categoria = arbitro.Categoria,
                    Federacao = arbitro.Federacao
                })
                .ToList();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                // Don't write the header again.
                Encoding = Encoding.UTF8,
                HasHeaderRecord = HasHeader,
                // HasHeaderRecord = true,
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
    }
}
