using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Bomberman.Rules;
using Bomberman.Rules.GameRules;
using Bomberman.Rules.RulePrinters;
using CommandLine;
using Domain.Common;
using Domain.Meta;
using Domain.Serialization;
using GameEngine.Common;
using GameEngine.Engine;
using GameEngine.Loggers;
using GameEngine.Renderers;
using Newtonsoft.Json;
using TestHarness.TestHarnesses.Bot;
using TestHarness.TestHarnesses.ConsoleHarness;
using TestHarness.TestHarnesses.SocketHarness;

namespace Bomberman
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var options = new Options();
                if (!Parser.Default.ParseArguments(args, options))
                {
                    return;
                }
                if (options.ShowRules)
                {
                    IRulePrinter printer = new ConsolePrinter();
                    printer.PrintRules(GameRules);

                    printer = new MarkdownPrinter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rules.md"));
                    printer.PrintRules(GameRules);

                    printer = new HtmlPrinter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rules.html"));
                    printer.PrintRules(GameRules);
                    return;
                }
                Console.WriteLine("Starting new game");

                var game = new BombermanGame();
                if (options.ConsoleLogger)
                {
                    game.Logger = new CombinedLogger(new InMemoryLogger(), new ConsoleLogger());
                }
                game.StartNewGame(options);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }


        public static readonly List<RuleContainer> GameRules = new List<RuleContainer>()
        {
            {new MapGeneration()},
            {new PlayerRulesContainer()},
            {new GameEngineRuleContainer()},
            {new PowerUpRules()},
            {new PointsRules()}
        }; 
    }
}
