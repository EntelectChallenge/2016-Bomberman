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
    public class TriggerBombCommand : ICommand
    {
        public void PerformCommand(GameMap gameMap, PlayerEntity player, CommandTransaction commandTransaction)
        {
            if(gameMap.GetPlayerBombCount(player) == 0)
                throw new InvalidCommandException("Player has no bombs to trigger");

            var bombs = gameMap.GetPlayerBombs(player);

            var bomb = bombs.Where(x => !x.IsExploding && x.BombTimer >= 1).OrderBy(x => x.BombTimer).FirstOrDefault();
            if (bomb != null)
            {
                bomb.BombTimer = 1;
            }
            else throw new InvalidCommandException("There are no eligible bombs to trigger");
        }
    }
}
