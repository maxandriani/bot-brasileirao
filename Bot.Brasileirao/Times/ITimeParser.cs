using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bot.Brasileirao.Times
{
    public interface ITimeParser
    {
        Time ParseTimeA(HtmlDocument page);
        Time ParseTimeB(HtmlDocument page);
    }
}
