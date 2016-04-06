using System;
using System.Diagnostics;
using System.IO;
using SampleBot.Domain.Enums;
using SampleBot.Properties;

namespace SampleBot
{
    public class Bot
    {
        protected string WorkingDirectory { get; set; }
        protected string Key { get; set; }

        public Bot(string key, string workingDirectory)
        {
            WorkingDirectory = workingDirectory;
            Key = key;
        }

        public void Execute()
        {
            var state = LoadState();

            var move = MakeMove();

            WriteMove(move);
        }

        private Moves MakeMove()
        {
            var random = new Random();
            var possibleShipCommands = Enum.GetValues(typeof (Moves));
            return (Moves) possibleShipCommands.GetValue(random.Next(0, possibleShipCommands.Length));
        }

        private string LoadState()
        {
            var filename = Path.Combine(WorkingDirectory, Settings.Default.StateFile);
            try
            {
                string jsonText;
                using (var file = new StreamReader(filename))
                {
                    jsonText = file.ReadToEnd();
                }

                return jsonText;
            }
            catch (IOException e)
            {
                Log(String.Format("Unable to read state file: {0}", filename));
                var trace = new StackTrace(e);
                Log(String.Format("Stacktrace: {0}", trace));
                return null;
            }
        }

        private void WriteMove(Moves move)
        {
            var moveInt = (int) move;
            var filename = Path.Combine(WorkingDirectory, Settings.Default.OutputFile);

            try
            {
                using (var file = new StreamWriter(filename))
                {
                    file.WriteLine(moveInt);
                }

                Log("Command: " + moveInt);
            }
            catch (IOException e)
            {
                Log(String.Format("Unable to write command file: {0}", filename));

                var trace = new StackTrace(e);
                Log(String.Format("Stacktrace: {0}", trace));
            }
        }

        private void Log(string message)
        {
            Console.WriteLine("[BOT]\t{0}", message);
        }
    }
}
