using Bot.Brasileirao.Gols;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bot.Brasileirao.Times
{
    public class TimeParser : ITimeParser
    {
        private static string TimeACssQuery = ".placar-wrapper div.time-left h3.time-nome";
        private static string TimeBCssQuery = ".placar-wrapper div.time-right h3.time-nome";

        private readonly ILogger<TimeParser> logger;
        private readonly IGolParser golParser;

        public TimeParser(ILogger<TimeParser> logger,
                          IGolParser golParser)
        {
            this.logger = logger;
            this.golParser = golParser;
        }

        public Time ParseTimeA(HtmlDocument page)
        {
            var time = ParseTime(page.DocumentNode.QuerySelector(TimeACssQuery));
            time.Gols = golParser.ParseTimeAGols(page);
            return time;
        }

        public Time ParseTimeB(HtmlDocument page)
        {
            var time = ParseTime(page.DocumentNode.QuerySelector(TimeBCssQuery));
            time.Gols = golParser.ParseTimeBGols(page);
            return time;
        }

        private Time ParseTime(HtmlNode node)
        {
            var time = new Time
            {
                Nome = node.InnerText.Trim()
            };

            logger.LogInformation($"Time {time} extraído com sucesso!");

            return time;
        }
    }
}
