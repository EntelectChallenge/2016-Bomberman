using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Domain.Common;
using GameEngine.Common;
using GameEngine.Engine;
using GameEngine.Loggers;
using GameEngine.Renderers;

namespace SocketHost
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Waiting for players to connect");
            Console.WriteLine("To start the game enter start and then enter");
            Console.WriteLine("To end the game enter end and then enter");
            Console.WriteLine("");

            List<Player> players;
            var line = "";
            using (var registration = new ClientRegistrationService())
            {
                players = registration.Players;
                line = Console.ReadLine();
                while (line != "start" && line != "end")
                {
                    line = Console.ReadLine();
                }
            }

            if (line == "start")
            {
                StartNewGame(players);
            }

            while (line != "end")
            {
                line = Console.ReadLine();
            }

            foreach (var player in players)
            {
                player.Dispose();
            }
        }

        private static void StartNewGame(List<Player> players)
        {
            var game = new BombermanEngine();
            game.Logger = new ConsoleLogger();
            game.RoundComplete += GameOnRoundComplete;
            game.PrepareGame(players, new Random().Next());
            game.StartNewGame();
        }

        private static void GameOnRoundComplete(GameMap gameMap, int round)
        {
            Console.WriteLine("");
            Console.WriteLine("Round Complete!");
            Console.WriteLine(new GameMapRender(gameMap).RenderTextGameState());
            Console.WriteLine("");
        }
    }
}
