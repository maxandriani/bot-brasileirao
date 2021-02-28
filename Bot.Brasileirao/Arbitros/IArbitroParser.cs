using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bot.Brasileirao.Arbitros
{
    public interface IArbitroParser
    {
        IList<Arbitro> ParseArbitros(HtmlDocument page);
    }
}
