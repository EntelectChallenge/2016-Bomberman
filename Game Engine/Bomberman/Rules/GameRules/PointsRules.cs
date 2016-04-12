using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bomberman.Rules.GameRules
{
    public class PointsRules : RuleContainer
    {
        public override string GetTitle()
        {
            return "Points";
        }

        public override string GetDescription()
        {
            return
                "Players will collect points during game play.  Points will be used (along with other conditions) to determine the player leaderboard and ultimately the winner";
        }

        public override List<Rule> GetRules()
        {
            return new List<Rule>()
            {
                {new Rule("Players will receive 10 points for destroying destructible walls.")
                {
                    SubRules = new List<Rule>()
                    {
                        {new Rule("If two bombs hit the same wall, both players will receive 10 points for destroying the wall.  Unless the wall was destroyed as result of a chain explosion.")},
                    }
                }},
                {new Rule("Players will receive points for killing another player based on the following equation ((100 + Max point per map for destructible walls) / players on map).  So on map with 10 destructible walls with 4 players the points for killing a player will be 50.")
                {
                    SubRules = new List<Rule>()
                    {
                        {new Rule("If two bombs hit another player, both players will receive points for killing the player.  Unless the player was killed as result of chain explosion.")},
                    }
                }},
                {new Rule("Players will receive points based on map coverage:")
                {
                    SubRules = new List<Rule>()
                    {
                        {new Rule("Points will only be calculated for each new block touched by a player.")},
                        {new Rule("Points will determine player coverage on the map, with a map coverage of 100% giving the player 100 points.")},
                    }
                }},
                {new Rule("Players obtaining the Super Power up will receive additional points.")},
                {new Rule("When multiple player bombs are triggered in a bomb chain, all players with bombs forming part of the chain will recieve the points for all entities destroyed in the chain.")},
                {new Rule("The round in which a player is killed will cause the player to forfeit all points earned in that round, and the player will lose points equal to the points earned when killing another player.")},
            };
        }
    }
}
