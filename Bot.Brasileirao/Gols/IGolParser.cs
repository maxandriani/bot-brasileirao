using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bot.Brasileirao.Gols
{
    public interface IGolParser
    {
        public IList<Gol> ParseTimeAGols(HtmlDocument page);

        public IList<Gol> ParseTimeBGols(HtmlDocument page);
    }
}
