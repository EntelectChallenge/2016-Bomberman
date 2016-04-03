using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;
using GameEngine.Commands;
using GameEngine.Common;

namespace GameEnginetest.Entities
{
    public class TestPlayer : Player
    {
        public TestPlayer(string name) : base(name)
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
