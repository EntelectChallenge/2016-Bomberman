using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotCalibrationDotNet
{
    class Program
    {
        static int Main(string[] args)
        {
            var playerKey = args[0];
            var workDir = args[1].Replace("\"", "");

            var readJsonState = File.ReadAllLines(Path.Combine(workDir, "state.json"));

            return 0;
        }
    }
}
