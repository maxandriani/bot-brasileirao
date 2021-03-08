using System;
using System.Collections.Generic;
using System.Text;

namespace Bot.Brasileirao.CSV
{
    public class ArbitroCsvDto
    {
        public ushort Rodada { get; set; }
        public string JogoLocal { get; set; }
        public DateTime JogoData { get; set; }
        public int JogoNumero { get; set; }
        public string JogoTimeA { get; set; }
        public string JogoTimeB { get; set; }
        public string Funcao { get; set; }
        public string Nome { get; set; }
        public string Categoria { get; set; }
        public string Federacao { get; set; }
    }
}
