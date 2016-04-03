using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Domain.Common;
using Domain.Entities;
using Domain.Serialization;
using Newtonsoft.Json;

namespace GameEngine.Renderers
{
    public class ConsoleRender : GameMapRender
    {
        public ConsoleRender(GameMap gameMap) : base(gameMap)
        {
        }

        public override StringBuilder RenderTextGameState()
        {
            var sb = new StringBuilder();
            var map = RenderMap();
            var playerInfo = RenderPlayerInfo();

            using (var mapReader = new StringReader(map.ToString()))
            {
                using (var playerReader = new StringReader(playerInfo.ToString()))
                {
                    var mapCurrent = mapReader.ReadLine();
                    sb.AppendLine(mapCurrent);
                    mapCurrent = mapReader.ReadLine();
                    while (mapCurrent != null)
                    {
                        sb.Append(mapCurrent);

                        var playerInfoCurrent = playerReader.ReadLine();
                        if (playerInfoCurrent != null)
                        {
                            sb.Append("\t\t").Append(playerInfoCurrent);
                        }
                        sb.AppendLine();
                        mapCurrent = mapReader.ReadLine();
                    }

                    var playerInfoExtended = playerReader.ReadLine();
                    while (playerInfoExtended != null)
                    {
                        for (int i = 1; i < GameMap.MapWidth; i++)
                        {
                            sb.Append(" ");
                        }
                        sb.Append("\t\t").Append(playerInfoExtended);
                        playerInfoExtended = playerReader.ReadLine();
                        sb.AppendLine();
                    }
                }
            }

            return sb;
        }

        public static void RenderToConsolePretty(GameMap gameMap, char playerKey)
        {
            Console.Clear();
            var render = new ConsoleRender(gameMap);

            bool insideMap = false;

            foreach (var character in render.RenderTextGameState().ToString())
            {
                if (character == '#')
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    insideMap = true;
                }
                if (character == '\t')
                {
                    insideMap = false;
                }
                if (character == '+')
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                }
                if (character == '!')
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                if (character == '&')
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }
                if (character == '$')
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                if (character == '*')
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                if (insideMap && (character == playerKey || Char.ToUpperInvariant(character) == playerKey))
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                }

                Console.Write(character);

                Console.ResetColor();
            }
        }
    }
}
