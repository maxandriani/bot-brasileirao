using System;
using System.Collections.Generic;
using System.Text;

namespace Bot.Brasileirao.Arbitros
{
    public class Arbitro
    {
        public string Funcao { get; set; }
        public string Nome { get; set; }
        public string Categoria { get; set; }
        public string Federacao { get; set; }

        public override string ToString()
        {
            return $"Arbitro {Nome}/{Funcao}";
        }
    }
}
