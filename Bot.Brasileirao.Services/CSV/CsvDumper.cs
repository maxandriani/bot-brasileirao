using Bot.Brasileirao.Jogos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Brasileirao.CSV
{
    public abstract class CsvDumper : IDumper
    {
        protected abstract string FileName { get; set; }
        protected string FilePath { get; set; } = "./";

        protected Stream CreateStream()
        {
           return File.Open(FilePath, FileMode.Append);
        }

        protected StreamWriter CreateWritter(Stream fileStream)
        {
            return new StreamWriter(fileStream);
        }

        public void SetupOutDir(string outDir)
        {
            Directory.CreateDirectory(outDir);
            FilePath = $"{outDir}/{FileName}";
        }

        public Task WriteLine(Jogo jogo)
        {
            return WriteLines(new List<Jogo> { jogo });
        }

        public abstract Task WriteLines(IEnumerable<Jogo> jogos);
    }
}
