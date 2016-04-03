using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Common;
using Domain.Entities;

namespace GameEngine.Commands
{
    public interface ICommand
    {
        /// <summary>
        /// Tells this command to perform the required action within the command transaction provided
        /// </summary>
        /// <param name="gameMap">The game map to make command calculations</param>
        /// <param name="player">The issuing player for this command</param>
        /// <param name="commandTransaction">The transaction that will performa the command once all player commands have been processed</param>
        void PerformCommand(GameMap gameMap, PlayerEntity player, CommandTransaction commandTransaction);
    }
}
