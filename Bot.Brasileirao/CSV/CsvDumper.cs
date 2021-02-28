using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bot.Brasileirao.CSV
{
    public abstract class CsvDumper
    {
        protected abstract string FileName { get; set; }

        protected Stream CreateStream()
        {
           return File.Open(FileName, FileMode.Append);
        }

        protected StreamWriter CreateWritter(Stream fileStream)
        {
            return new StreamWriter(fileStream);
        }
    }
}
