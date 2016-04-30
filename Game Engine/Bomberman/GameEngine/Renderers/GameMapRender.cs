using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Common;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Serialization;
using Newtonsoft.Json;

namespace GameEngine.Renderers
{
    public class GameMapRender
    {
        protected readonly GameMap GameMap;
        private readonly Boolean minify;

        public GameMapRender(GameMap gameMap)
            : this(gameMap, false)
        {
        }

        public GameMapRender(GameMap gameMap, bool minify)
        {
            GameMap = gameMap;
            this.minify = minify;
        }

        public StringBuilder RenderJsonGameState()
        {
            return new StringBuilder(JsonConvert.SerializeObject(GameMap, minify ? Formatting.None : Formatting.Indented));
        }

        public virtual StringBuilder RenderTextGameState()
        {
            var sb = new StringBuilder();
            sb.Append(RenderMap());
            sb.Append(RenderPlayerInfo());

            return sb;
        }

        public StringBuilder RenderMap()
        {
            var sb = new StringBuilder();

            sb.AppendLine(String.Format("Map Width: {0}, Map Height: {1}, Current Round: {2}, Seed: {3}", GameMap.MapWidth, GameMap.MapHeight, GameMap.CurrentRound, GameMap.MapSeed));

            using (var enumerator = GameMap.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var gameBlock = enumerator.Current;
                    sb.Append(gameBlock.GetMapSymbol());

                    if (gameBlock.Location.X == GameMap.MapWidth)
                        sb.AppendLine();
                }
            }

            return sb;
        }

        public StringBuilder RenderPlayerInfo()
        {
            var sb = new StringBuilder();

            using (var enumerator = GameMap.RegisteredPlayerEntities.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var playerEntity = enumerator.Current;
                    if(playerEntity == null)
                        continue;

                    sb.AppendLine("---------------------------")
                        .AppendLine(String.Format("Player Name: {0}", playerEntity.Name))
                        .AppendLine(String.Format("Key: {0}", playerEntity.Key))
                        .AppendLine(String.Format("Points: {0}", playerEntity.Points))
                        .AppendLine(String.Format("Status: {0}", !playerEntity.Killed ? "Alive" : "Dead"))
                        .Append("Bombs: ");

                    var playerBombs = FindPlayerBombs(playerEntity);
                    var bombCount = playerBombs.Count;
                    foreach(var bomb in playerBombs)
                    {
                        sb.Append(String.Format("{{x:{0},y:{1},fuse:{2},radius:{3}}}", bomb.Location.X, bomb.Location.Y,
                            bomb.BombTimer, bomb.BombRadius));

                        if (--bombCount <= 0) continue;
                        
                        if (minify) sb.Append(",");
                        else sb.AppendLine(",");
                    }

                    sb.AppendLine()
                        .AppendLine(String.Format("BombBag: {0}", playerEntity.BombBag - playerBombs.Count))
                        .AppendLine(String.Format("BlastRadius: {0}", playerEntity.BombRadius))
                        .AppendLine("---------------------------");
                }
            }

            return sb;
        }

        private List<BombEntity> FindPlayerBombs(PlayerEntity playerEntity)
        {
            var playerBombs = new List<BombEntity>();
            using (var enumerator = GameMap.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var gameBlock = enumerator.Current;
                    if (gameBlock.Bomb != null && gameBlock.Bomb.Owner == playerEntity)
                    {
                        playerBombs.Add(gameBlock.Bomb);
                    }
                }
            }

            return playerBombs;
        }
    }
}
