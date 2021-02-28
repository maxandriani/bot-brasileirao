using Bot.Brasileirao.Arbitros;
using Bot.Brasileirao.Rodadas;
using Bot.Brasileirao.Times;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bot.Brasileirao.Jogos
{
    public class Jogo
    {
        public ushort Rodada { get; set; }
        public string Local { get; set; }
        public DateTime Data { get; set; }
        public int Numero { get; set; }
        public Time Vencedor {
            get => (TimeA.Gols.Count > TimeB.Gols.Count) 
                ? TimeA 
                : (TimeA.Gols.Count < TimeB.Gols.Count)
                    ? TimeB
                    : null;
        }
        public Time TimeA { get; set; }
        public int TotalGolsTimeA { get => TimeA.Gols.Count; }
        public Time TimeB { get; set; }
        public int TotalGolsTimeB { get => TimeB.Gols.Count; }
        public IList<Arbitro> Arbitros { get; set; } = new List<Arbitro>();
    }
}
