using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Bot.Brasileirao.Rodadas
{
    public class RodadaParser : IRodadaParser
    {
        private static string RodadasCssQuery = @".aside-rodadas .swiper-slide";
        private static string RodadaTitleCssQuery = @".aside-header > h3";
        private static string JogosLinkCssQuery = @".aside-content .partida-desc > a.btn";

        private readonly ILogger<RodadaParser> logger;

        public RodadaParser(ILogger<RodadaParser> logger)
        {
            this.logger = logger;
        }

        public IList<Rodada> ParseRodadas(HtmlDocument page)
        {
            var rodadas = new List<Rodada>();

            logger.LogInformation($"Extraindo rodadas...");
            var rodadasNode = page.DocumentNode.QuerySelectorAll(RodadasCssQuery);

            foreach(var node in rodadasNode)
            {
                rodadas.Add(ParseRodada(node));
            }

            logger.LogInformation($"Extração de rodadas concluída!");

            return rodadas;
        }

        private Rodada ParseRodada(HtmlNode node)
        {
            var rodada = new Rodada
            {
                Numero = ExtractRodaNumber(node.QuerySelector(RodadaTitleCssQuery)),
                Jogos = ParseJogos(node.QuerySelectorAll(JogosLinkCssQuery))
            };

            logger.LogInformation($"Rodada {rodada} extraída com sucesso!");

            return rodada;
        }

        private ushort ExtractRodaNumber(HtmlNode node)
        {
            var reg = new Regex(@"(\d+)");
            var match = reg.Match(node.InnerText.Trim());
            return ushort.Parse(match.Value);
        }

        private IList<string> ParseJogos(IList<HtmlNode> jogos)
        {
            return jogos.Select(a => a.GetAttributeValue("href", "").Trim()).ToList();
        }
        
    }
}
