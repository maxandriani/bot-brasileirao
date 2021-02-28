using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Bot.Brasileirao.Gols
{
    public class GolParser : IGolParser
    {
        private static string TimeACssQuery = ".placar-wrapper div.text-left p.time-jogador";
        private static string TimeBCssQuery = ".placar-wrapper div.text-right p.time-jogador";

        private readonly ILogger<GolParser> logger;

        public GolParser(ILogger<GolParser> logger)
        {
            this.logger = logger;
        }

        public IList<Gol> ParseTimeAGols(HtmlDocument page)
        {
            logger.LogInformation($"Pesquisando gols do Time A");
            return ParseGolsFromNodes(page.DocumentNode.QuerySelectorAll(TimeACssQuery));
        }

        public IList<Gol> ParseTimeBGols(HtmlDocument page)
        {
            return ParseGolsFromNodes(page.DocumentNode.QuerySelectorAll(TimeBCssQuery));
        }

        private IList<Gol> ParseGolsFromNodes(IList<HtmlNode> nodes)
        {
            var gols = new List<Gol>();

            foreach(var node in nodes)
            {
                gols.Add(ParseGolFromNode(node));
            };

            return gols;
        }

        private Gol ParseGolFromNode(HtmlNode node)
        {
            var reg = new Regex(@"([\w\s]+)\s(\d+)(?:\+(\d+))?\s\((\d).+\).*?$", RegexOptions.IgnoreCase);
            var match = reg.Match(node.InnerText.Replace("&#039;", "").Trim());
            
            if (!match.Success)
            {
                throw new Exception($"Não foi possível serializar o gol '{node.InnerText.Trim()}'");
            }

            var Tempo = ushort.Parse(match.Groups[2].Value);
            var OverTime = match.Groups[3].Value.Length > 0 ? ushort.Parse(match.Groups[3].Value) : 0;

            var gol = new Gol
            {
                Jogador = match.Groups[1].Value,
                Tempo = ((ushort)(Tempo + OverTime)),
                Periodo = GetPeriodoFromMatch(match.Groups[4].Value)
            };

            logger.LogInformation($"Gol {gol} extraído com sucesso!");

            return gol;
        }

        private Periodo GetPeriodoFromMatch(string periodo)
        {
            switch (periodo)
            {
                case "1": return Periodo.Primeiro;
                case "2": return Periodo.Segundo;
                case "3": return Periodo.Prorrogacao1;
                case "4": return Periodo.Prorrogacao2;
                default: return Periodo.Desempate;
            }
        }
    }
}
