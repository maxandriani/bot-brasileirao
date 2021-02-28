using Bot.Brasileirao.Arbitros;
using Bot.Brasileirao.Gols;
using Bot.Brasileirao.Times;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Bot.Brasileirao.Jogos
{
    public class JogoParser : IJogoParser
    {
        private static string JogoIdCssQuery = @".section-placar-header .col-sm-3.text-right span";
        private static string LocalCssQuery = @".section-content-header span.text-2:nth-child(1)";
        private static string DataCssQuery = @".section-content-header span.text-2:nth-child(2)";
        private static string HoraCssQuery = @".section-content-header span.text-2:nth-child(3)";

        private readonly ILogger<JogoParser> logger;
        private readonly IArbitroParser arbitroParser;
        private readonly IGolParser golParser;
        private readonly ITimeParser timeParser;

        public JogoParser(ILogger<JogoParser> logger,
                          IArbitroParser arbitroParser,
                          IGolParser golParser,
                          ITimeParser timeParser)
        {
            this.logger = logger;
            this.arbitroParser = arbitroParser;
            this.timeParser = timeParser;
        }

        public Jogo ParseJogo(HtmlDocument page)
        {
            var jogo = new Jogo
            {
                Numero = ParseJogoId(page.DocumentNode.QuerySelector(JogoIdCssQuery)),
                Local = ParseLocal(page.DocumentNode.QuerySelector(LocalCssQuery)),
                Data = ParseDateTime(page.DocumentNode.QuerySelector(DataCssQuery), page.DocumentNode.QuerySelector(HoraCssQuery)),

                TimeA = timeParser.ParseTimeA(page),
                TimeB = timeParser.ParseTimeB(page),

                Arbitros = arbitroParser.ParseArbitros(page)
            };

            logger.LogInformation($"Jogo extraído com sucesso!");

            return jogo;
        }

        private int ParseJogoId(HtmlNode node)
        {
            var rex = new Regex(@"(\d+)");
            var match = rex.Match(node.InnerText.Trim());

            return int.Parse(match.Value);
        }

        private string ParseLocal(HtmlNode node)
        {
            return node.InnerText.Trim();
        }

        private DateTime ParseDateTime(HtmlNode date, HtmlNode time)
        {
            var dateReg = new Regex(@"(\d+)\sde\s(\w+)\sde\s(\d+)$");
            var dateMatch = dateReg.Match(date.InnerText.Trim());
            var timeReg = new Regex(@"(\d+)\:(\d+)$");
            var timeMatch = timeReg.Match(time.InnerText.Trim());

            if (!dateMatch.Success || !timeMatch.Success)
            {
                throw new Exception($"Não foi possível serializar a data {date.InnerText.Trim()} {time.InnerText.Trim()}");
            }

            var year = int.Parse(dateMatch.Groups[3].Value);
            var month = ParseMonth(dateMatch.Groups[2].Value);
            var days = int.Parse(dateMatch.Groups[1].Value);
            var hours = int.Parse(timeMatch.Groups[1].Value);
            var minutes = int.Parse(timeMatch.Groups[2].Value);

            return new DateTime(year, month, days, hours, minutes, 0);
        }

        private int ParseMonth(string month)
        {
            switch (month)
            {
                case "Janeiro": return 1;
                case "Fevereiro": return 2;
                case "Março": return 3;
                case "Abril": return 4;
                case "Maio": return 5;
                case "Junho": return 6;
                case "Julho": return 7;
                case "Agosto": return 8;
                case "Setembro": return 9;
                case "Outubro": return 10;
                case "Novembro": return 11;
                case "Dezembro": return 12;
            }
            return 0;
        }
    }
}
