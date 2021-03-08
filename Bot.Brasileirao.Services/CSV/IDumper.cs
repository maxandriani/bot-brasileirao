using Bot.Brasileirao.Jogos;
using Bot.Brasileirao.Rodadas;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Brasileirao.CSV
{
    public interface IDumper
    {
        void SetupOutDir(string outDir);
        Task WriteLine(Jogo jogo);
        Task WriteLines(IEnumerable<Jogo> jogos);
    }
}
