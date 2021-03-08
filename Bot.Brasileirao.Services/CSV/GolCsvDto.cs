using Bot.Brasileirao.Gols;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bot.Brasileirao.CSV
{
    public class GolCsvDto
    {
        public ushort Rodada { get; set; }
        public string JogoLocal { get; set; }
        public DateTime JogoData { get; set; }
        public int JogoNumero { get; set; }
        public string Time { get; set; }
        public string Jogador { get; set; }
        public ushort Tempo { get; set; }
        public string Periodo { get; set; }

        public static string GetPeriodoString(Periodo periodo)
        {
            switch (periodo)
            {
                case Bot.Brasileirao.Gols.Periodo.Primeiro: return "Primeiro";
                case Bot.Brasileirao.Gols.Periodo.Segundo: return "Segundo";
                case Bot.Brasileirao.Gols.Periodo.Prorrogacao1: return "Prorrogação 1";
                case Bot.Brasileirao.Gols.Periodo.Prorrogacao2: return "Prorrogação 2";
                case Bot.Brasileirao.Gols.Periodo.Desempate: return "Desempate";
            }

            return "";
        }
    }
}
