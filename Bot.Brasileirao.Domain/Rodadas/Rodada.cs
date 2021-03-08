using Bot.Brasileirao.Jogos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Bot.Brasileirao.Rodadas
{
    public class Rodada
    {
        public ushort Numero { get; set; }

        public IList<string> Jogos { get; set; } = new List<string>();

        public override string ToString()
        {
            return $"Rodada {Numero}";
        }
    }
}
