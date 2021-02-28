using System;
using System.Collections.Generic;
using System.Text;

namespace Bot.Brasileirao.Gols
{
    public enum Periodo
    {
        Primeiro,
        Segundo,
        Prorrogacao1,
        Prorrogacao2,
        Desempate
    }
    public class Gol
    {
        public string Jogador { get; set; }
        public ushort Tempo { get; set; }
        public Periodo Periodo { get; set; }

        public override string ToString()
        {
            return $"Gol {Jogador}, {Tempo}', {Periodo}";
        }
    }
}
