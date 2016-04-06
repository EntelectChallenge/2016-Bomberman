using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleBot
{
    class Program
    {
        public static void Main(string[] args)
        {
            var stopwatch = Stopwatch.StartNew();

            RunBot(args);
            
            stopwatch.Stop();
            Console.WriteLine("[BOT]\tBot finished in {0} ms.", stopwatch.ElapsedMilliseconds);
        }

        private static void RunBot(string[] args)
        {
            if (args.Length != 2)
            {
                PrintUsage();
                Environment.Exit(1);
            }

            var workingDirectory = args[1];
            if (!Directory.Exists(workingDirectory))
            {
                PrintUsage();
                Console.WriteLine();
                Console.WriteLine("Error: Working directory \"" + workingDirectory + "\" does not exist.");
                Environment.Exit(1);
            }

            Bot bot = new Bot(args[0], args[1]);
            bot.Execute();
        }

        private static void PrintUsage()
        {
            Console.WriteLine("C# SampleBot usage: SampleBot.exe <PlayerKey> <WorkingDirectoryFilename>");
            Console.WriteLine();
            Console.WriteLine("\tPlayerKey\tThe key assigned to your bot.");
            Console.WriteLine("\tWorkingDirectoryFilename\tThe working directory folder where the match runner will output map and state files and look for the move file.");
        }
    }
}
