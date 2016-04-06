using System;
using System.Linq;
using Domain.Common;
using GameEngine.Commands;
using GameEngine.Commands.PlayerCommands;
using GameEngine.Common;
using GameEngine.Renderers;

namespace TestHarness.TestHarnesses.ConsoleHarness
{
    public class ConsoleHarness : Player
    {
        private char playerKey;

        public ConsoleHarness(string name) : base(name)
        {
        }

        public override void StartGame(GameMap gameState)
        {
            playerKey = gameState.RegisteredPlayerEntities.First(x => x.Name == Name).Key;

            NewRoundStarted(gameState);
        }

        public override void NewRoundStarted(GameMap gameState)
        {
            ConsoleRender.RenderToConsolePretty(gameState, playerKey);

            Console.WriteLine("To move, type a,w,s,d and press enter");
            Console.WriteLine("To plant a bomb, type z and enter");
            Console.WriteLine("You can reduce the bomb timer to 1 using x");
            Console.WriteLine();
            Console.WriteLine("Movement for player " + Name);
            var line = System.Console.ReadLine();

            if (line == "w")
                PublishCommand(new MovementCommand(MovementCommand.Direction.Up));
            else if (line == "s")
                PublishCommand(new MovementCommand(MovementCommand.Direction.Down));
            else if (line == "a")
                PublishCommand(new MovementCommand(MovementCommand.Direction.Left));
            else if (line == "d")
                PublishCommand(new MovementCommand(MovementCommand.Direction.Right));
            else if (line == "z")
                PublishCommand(new PlaceBombCommand());
            else if (line == "x")
                PublishCommand(new TriggerBombCommand());
            else 
                PublishCommand(new DoNothingCommand());
        }

        public override void GameEnded(GameMap gameMap)
        {
        }

        public override void PlayerKilled(GameMap gameMap)
        {
            System.Console.WriteLine("Player " + Name + " has been killed");
        }

        public override void PlayerCommandFailed(ICommand command, string reason)
        {
            System.Console.WriteLine("Could not process player command: " + reason);
        }

        public override void Dispose()
        {
        }
    }
}
