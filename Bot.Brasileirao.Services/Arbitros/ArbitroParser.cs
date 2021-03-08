using Bot.Brasileirao.Services;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bot.Brasileirao.Arbitros
{
    public class ArbitroParser : IArbitroParser, ITransientService
    {
        private static string ArbitrosCssQuery = @"[id*='arbitros'] .table > tbody > tr";

        private readonly ILogger<ArbitroParser> logger;

        public ArbitroParser(ILogger<ArbitroParser> logger)
        {
            this.logger = logger;
        }

        public IList<Arbitro> ParseArbitros(HtmlDocument page)
        {
            return ParseArbitros(page.DocumentNode.QuerySelectorAll(ArbitrosCssQuery));
        }

        private IList<Arbitro> ParseArbitros(IList<HtmlNode> nodes)
        {
            var arbitros = new List<Arbitro>();
            
            logger.LogInformation($"Escaneando arbitros...");

            foreach(var node in nodes)
            {
                arbitros.Add(ParseArbitro(node));
            }

            logger.LogInformation($"Arbitros escaneados com sucesso!");

            return arbitros;
        }

        private Arbitro ParseArbitro(HtmlNode node)
        {
            var map = node.QuerySelectorAll("td")
                .Select(node => node.InnerText.Trim())
                .ToList();

            var funcao = node.QuerySelector("th").InnerText.Trim();
            var nome = map[0];
            var categoria = map[1];
            var federacao = map[2];

            var arbitro = new Arbitro
            {
                Funcao = funcao,
                Nome = nome,
                Categoria = categoria,
                Federacao = federacao
            };

            logger.LogInformation($"Arbitro {arbitro} extraído com sucesso!");

            return arbitro;
        }
    }
}
