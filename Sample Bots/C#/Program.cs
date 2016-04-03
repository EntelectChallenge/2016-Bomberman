using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleBot
{
    class Program
    {
        static int Main(string[] args)
        {
            var key = args[0];
            var dir = args[1];

            File.Create(Path.Combine(dir.Replace("\"",""),  "TestArgs.txt"));

            return new Random().Next(1,6);
        }
    }
}
