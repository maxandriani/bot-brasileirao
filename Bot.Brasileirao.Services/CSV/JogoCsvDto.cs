using System;
using System.Collections.Generic;
using System.Text;

namespace Bot.Brasileirao.CSV
{
    public class JogoCsvDto
    {
        public ushort Rodada { get; set; }
        public string Local { get; set; }
        public DateTime Data { get; set; }
        public int Numero { get; set; }
        public string Vencedor { get; set; }
        public string TimeA { get; set; }
        public int TotalGolsTimeA { get; set; }
        public string TimeB { get; set; }
        public int TotalGolsTimeB { get; set; }
    }
}
