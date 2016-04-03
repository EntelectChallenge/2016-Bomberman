using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Reference.Domain.Map;
using Reference.Strategies.AStar;

namespace Reference
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                char playerKey;
                string outputLocation;
                string inputMap;
                if (args == null || args.Length == 0)
                {
                    playerKey = 'A';
                    outputLocation = Path.Combine(@"C:\Users\hennie.brink\Desktop\Bomberman\Game\", playerKey.ToString());
                    inputMap =
                        File.ReadAllText(@"C:\Users\hennie.brink\Desktop\Bomberman\Replays\259502815\10\A\state.json");
                }
                else
                {
                    playerKey = Char.Parse(args[0]);
                    outputLocation = args[1];
                    inputMap = File.ReadAllText(Path.Combine(outputLocation, @"state.json"));
                }

                var map = GameMap.FromJson(inputMap);
                var gameStrategy = new AStarStrategy();

                var command = gameStrategy.ExecuteStrategy(map, playerKey);

                Console.WriteLine("Sending Back command " + command);
                File.WriteAllText(Path.Combine(outputLocation, "move.txt"), ((int)command).ToString());

                return 0;
            }
            catch (Exception ex)
            {
                Console.Write(ex);

                return -1;
            }
        }
    }
}
