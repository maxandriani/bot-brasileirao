using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bot.Brasileirao.Jogos
{
    public interface IJogoParser
    {
        Jogo ParseJogo(HtmlDocument page);
    }
}
