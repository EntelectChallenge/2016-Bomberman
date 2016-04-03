using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Common;
using Domain.Entities;

namespace GameEngine.Commands.PlayerCommands
{
    public class DoNothingCommand : ICommand
    {
        public void PerformCommand(GameMap gameMap, PlayerEntity player, CommandTransaction commandTransaction)
        {
        }

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}
