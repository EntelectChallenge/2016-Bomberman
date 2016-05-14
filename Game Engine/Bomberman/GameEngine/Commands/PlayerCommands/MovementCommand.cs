using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Common;
using Domain.Entities;
using GameEngine.Exceptions;

namespace GameEngine.Commands.PlayerCommands
{
    public class MovementCommand : ICommand
    {
        private readonly Direction _direction;

        public MovementCommand(Direction direction)
        {
            _direction = direction;
        }

        public virtual void PerformCommand(GameMap gameMap, PlayerEntity player, CommandTransaction commandTransaction)
        {
            var playerLocation = player.Location;

            Location destinationLocation;
            switch (_direction)
            {
                    case Direction.Up:
                    destinationLocation = new Location(playerLocation.X, playerLocation.Y - 1);
                    break;
                    case Direction.Down:
                    destinationLocation = new Location(playerLocation.X, playerLocation.Y + 1);
                    break;
                    case Direction.Left:
                    destinationLocation = new Location(playerLocation.X - 1, playerLocation.Y);
                    break;
                    case Direction.Right:
                    destinationLocation = new Location(playerLocation.X + 1, playerLocation.Y);
                    break;
                default:
                    throw new Exception("Unkown Direction " + _direction);
            }

            if (destinationLocation.X < 1 || destinationLocation.Y < 1 || destinationLocation.X > gameMap.MapWidth ||
                destinationLocation.Y > gameMap.MapHeight)
            {
                throw new InvalidCommandException(String.Format("Cannot move to {0} as it is outside of the map bounds", destinationLocation));
            }

            var destinationBlock = gameMap.GetBlockAtLocation(destinationLocation.X, destinationLocation.Y);
            if (destinationBlock.Entity != null && destinationBlock.Entity.GetType() != typeof(PlayerEntity))
            {
                throw new InvalidCommandException(String.Format("Cannot move to {0}, {1} already occupies this space", destinationLocation, destinationBlock.Entity));
            }
            if (destinationBlock.Bomb != null)
            {
                throw new InvalidCommandException(String.Format("Cannot move to {0}, there is a bomb planted in this location", destinationLocation));
            }

            commandTransaction.RequestMovement(player, destinationBlock);
        }

        public override string ToString()
        {
            return String.Format("{0}({1})", GetType().Name, _direction);
        }

        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }
    }
}
