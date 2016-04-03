using System;
using Domain.Common;
using Domain.Entities;
using Domain.Interfaces;
using GameEngine.Commands;
using GameEngine.Loggers;

namespace GameEngine.Common
{
    /// <summary>
    /// The base class for all players/test harnesses of the game to extend.
    /// Processing of players happens in the following order.
    /// 1. Player is registered to a game map, and PlayerRegistered is called.
    /// 2. New Game is Started.
    /// 3. Player Published a Command
    /// 4. Player Waits for round to complete
    /// 5. Round is Completed.
    /// 6. Steps 3-5 until GameEnded is called
    /// 
    /// A player may only submit one command per round.  Any subsuquent commands will be ignored by the game engine. Exception may be thrown
    /// A player may not submit commands to the game egine, any attempts to do so will be ignored by the game egnine. Exception may be thrown
    /// 
    /// The game engine places no restrictions on players, and expects each player to submit a command each round, while the player is alive.
    /// 
    /// If the test harness imposes any rules on the hosting player, such as a time limit, that restriction has to be enforced by the harness.
    /// If the test harness hosts a player that can be instable, such as a network connection, or a malfunctioning bot, it must cater for that by
    /// still submitting a do nothing command to the game engine every round until the game is ended, or the player is killed.
    /// 
    /// The player entity registered to the player is linked directly to the game engine, and should this NOT BE MODIFIED IN ANY WAY!  Yeah I can
    /// make a protected entity, but we are in control of the players/test harnesses, so do not mess with the game egine by chaning the entity.
    /// 
    /// If you want to log information from the game engine for the player specifically, please provide a logger by setting the property, the game engine
    /// will then pass game logs on to you player.
    /// </summary>
    public abstract class Player : IDisposable
    {
        public delegate void PublishCommandHandler(Player player, ICommand command);

        /// <summary>
        /// Username for this player
        /// </summary>
        public String Name { get; private set; }
        /// <summary>
        /// The player key on the game map for this player.  This will be set when the player is registered for a game, and should not be changed
        /// </summary>
        public PlayerEntity PlayerEntity { get; private set; }
        /// <summary>
        /// Command listener, all commands will be published to the listener, and then processed once all player commands were received by the game engine
        /// </summary>
        public event PublishCommandHandler CommandListener;

        public ILogger Logger { get; set; }

        protected Player(String name)
        {
            Name = name;
        }

        /// <summary>
        /// Called once the player has been registered on a new game map
        /// </summary>
        /// <param name="playerEntity">The registered player entity</param>
        public void PlayerRegistered(PlayerEntity playerEntity)
        {
            PlayerEntity = playerEntity;
        }

        /// <summary>
        /// Convenience method to publish a player command.  Can only be called once per turn.
        /// </summary>
        /// <param name="command"></param>
        protected void PublishCommand(ICommand command)
        {
            if (CommandListener != null)
                CommandListener(this, command);
        }

        /// <summary>
        /// Notifies the player that a new game has been started, and the game engine will start listening for commands
        /// </summary>
        /// <param name="gameState"></param>
        public abstract void StartGame(GameMap gameState);
        /// <summary>
        /// Notifies the player that the current round of commands is complete, and a new round has been started
        /// </summary>
        /// <param name="gameState"></param>
        public abstract void NewRoundStarted(GameMap gameState);

        /// <summary>
        /// Notifies the player that the game has ended, as determined by the game engine
        /// </summary>
        /// <param name="gameMap">The current game map</param>
        public abstract void GameEnded(GameMap gameMap);

        /// <summary>
        /// Notifies the player that they have been killed, and will not longer be allowed to publish commands
        /// </summary>
        /// <param name="gameMap">The current game map</param>
        public abstract void PlayerKilled(GameMap gameMap);
        /// <summary>
        /// Notifies the player that the command issued by them has failed
        /// </summary>
        /// <param name="command"></param>
        /// <param name="reason"></param>
        public abstract void PlayerCommandFailed(ICommand command, String reason);

        /// <summary>
        /// This will be called right before new round started so players can do cleanup for the current round
        /// </summary>
        public virtual void RoundComplete(GameMap gameMap, int round)
        {
            
        }
        public abstract void Dispose();

        public override string ToString()
        {
            var playerKey = PlayerEntity == null ? '?' : PlayerEntity.Key;
            return String.Format("Player {0}: {1}", playerKey, Name);
        }
    }
}
