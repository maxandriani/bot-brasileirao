using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bot.Brasileirao.Rodadas
{
    public interface IRodadaParser
    {
        IList<Rodada> ParseRodadas(HtmlDocument page);
    }
}
