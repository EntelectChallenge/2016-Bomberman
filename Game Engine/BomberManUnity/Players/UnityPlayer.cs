using System;
using System.Collections.Generic;
using System.Text;
using Domain.Common;
using GameEngine.Commands;
using GameEngine.Common;

namespace BomberManUnity.Players
{
    /// <summary>
    /// The unity engine will be interacting directly with the game engine, so no need for players that do anything, 
    /// We just need it for the game engine to function correctly
    /// </summary>
    public class UnityPlayer : Player
    {
        public UnityPlayer(string name) : base(name)
        {
        }

        public override void StartGame(GameMap gameState)
        {
        }

        public override void NewRoundStarted(GameMap gameState)
        {
        }

        public override void GameEnded(GameMap gameMap)
        {
        }

        public override void PlayerKilled(GameMap gameMap)
        {
        }

        public override void PlayerCommandFailed(ICommand command, string reason)
        {
        }

        public override void Dispose()
        {
        }
    }
}
