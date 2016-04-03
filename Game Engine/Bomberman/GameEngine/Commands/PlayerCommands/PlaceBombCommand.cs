using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Common;
using Domain.Entities;
using GameEngine.Exceptions;
using GameEngine.Properties;

namespace GameEngine.Commands.PlayerCommands
{
    public class PlaceBombCommand : ICommand
    {
        private readonly int _timerMultiplier;

        public PlaceBombCommand()
        {
            _timerMultiplier = Settings.Default.BombTimerMultiplier;
        }

        /// <summary>
        /// User for testing purposes
        /// </summary>
        /// <param name="timerMultiplier">The timerMultiplier for the bomb</param>
        public PlaceBombCommand(int timerMultiplier)
        {
            _timerMultiplier = timerMultiplier;
        }

        public void PerformCommand(GameMap gameMap, PlayerEntity player, CommandTransaction commandTransaction)
        {
            if (player.BombBag - gameMap.GetPlayerBombCount(player) == 0)
                throw new InvalidCommandException(String.Format("Already placed all the bombs for player {0}", player));

            var playerBlock = gameMap.GetBlockAtLocation(player.Location.X, player.Location.Y);

            if (playerBlock.Bomb != null)
            {
                throw new InvalidCommandException(String.Format("There is already a bomb placed here {0}", playerBlock.Location));
            }

            var bombTimer = Math.Min(9, (player.BombBag * _timerMultiplier)) + 1;
            playerBlock.PlantBomb(bombTimer);
        }

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}
