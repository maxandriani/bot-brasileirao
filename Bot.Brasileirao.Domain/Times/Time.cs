using Bot.Brasileirao.Gols;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bot.Brasileirao.Times
{
    public class Time
    {
        public string Nome { get; set; }
        public IList<Gol> Gols = new List<Gol>();

        public override string ToString()
        {
            return $"Time {Nome}, {Gols.Count} Gols";
        }
    }
}
