using System;
using System.Collections.Generic;
using System.Text;
using Domain.Common;
using Domain.Entities;
using GameEngine.Commands.PlayerCommands;
using GameEngine.Common;
using GameEngine.Engine;
using GameEngine.Loggers;

namespace BomberManUnity.Engine
{
    /// <summary>
    /// The unity round processor will work with a bit more real time type of play.  Due to the nature of the game
    /// and how things work, the rules will differ slighty, such as a player being able to move of a bomb block
    /// before that bom starts to tick.  In the normal engine when the player moves from the bomb the tick would have already happened.
    /// This is something we have to sacrifice to ensure a smooth gameplay experience.
    /// 
    /// There is also no notion of rounds here, so the round counter will not be incremented as in normal play
    /// </summary>
    public class UnityRoundProcessor : GameRoundProcessor
    {
        public UnityRoundProcessor(int round, GameMap gameMap, ILogger logger) : base(round, gameMap, logger)
        {
        }

        /// <summary>
        /// This should tick every couple of milliseconds to process player commands etc
        /// </summary>
        public void TickMinor()
        {
            ProcessPlayerCommands();
            ApplyPowerUps();
            ApplyMovementBonus();
        }

        /// <summary>
        /// Major ticks should occur every second, this will run all of the other round processing stuff like bomb triggering etc
        /// </summary>
        public void TickMajor()
        {
            RemoveExplosionsFromMap();
            DescreaseBombTimers();
            DetonateBombs();
            MarkEntitiesForDestruction();
            ProcessPlayerCommands();
            ApplyPowerUps();
            DestroyMarkedEntities();
            ApplyMovementBonus();
        }

        /// <summary>
        /// Will cause the trigger bomb command to be send for the player.  The game engine will probably have to update after this to indicate the new bomb timer
        /// </summary>
        /// <param name="player">The player for whom the bomb should be triggered</param>
        public void TriggerBombForPlayer(Player player)
        {
            AddPlayerCommand(player, new TriggerBombCommand());
            TickMinor();
        }

        public void MovePlayerToLocation(Player player, int x, int y)
        {
            var whileCount = 0;
            while (player.PlayerEntity.Location.X != x || player.PlayerEntity.Location.Y != y)
            {
                whileCount++;
                if (player.PlayerEntity.Location.X > x)
                {
                    AddPlayerCommand(player, new MovementCommand(MovementCommand.Direction.Left));
                }
                if (player.PlayerEntity.Location.X < x)
                {
                    AddPlayerCommand(player, new MovementCommand(MovementCommand.Direction.Right));
                }
                if (player.PlayerEntity.Location.Y > y)
                {
                    AddPlayerCommand(player, new MovementCommand(MovementCommand.Direction.Up));
                }
                if (player.PlayerEntity.Location.Y < y)
                {
                    AddPlayerCommand(player, new MovementCommand(MovementCommand.Direction.Down));
                }

                TickMinor();

                if (whileCount > 10)
                {
                    throw new Exception(String.Format("Player did not move to location in a timely manner, requested to move to location {0}:{1} but is still at {2}:{3} ", x, y, player.PlayerEntity.Location.X, player.PlayerEntity.Location.Y));
                }
            }
        }
    }
}
