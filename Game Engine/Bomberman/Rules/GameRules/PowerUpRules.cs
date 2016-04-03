using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bomberman.Rules.GameRules
{
    public class PowerUpRules : RuleContainer
    {
        public override string GetTitle()
        {
            return "Power Ups";
        }

        public override string GetDescription()
        {
            return "Power ups can be collected by players to improve their players abilities";
        }

        public override List<Rule> GetRules()
        {
            return new List<Rule>()
            {
                {new Rule("The bomb bag power up will give the player an additional bomb to plant on the map while the timers on other bombs are decreasing.")},
                {new Rule("The bomb radius power up will multiply the current bomb radius of the player by two.")},
                {new Rule("The special power up will give the following bonuses:")
                {
                    SubRules = new List<Rule>()
                    {
                        {new Rule("Bomb bag power up")},
                        {new Rule("Bomb radius power up")},
                        {new Rule("50 points")},
                    }
                }},
                
            };
        }
    }
}
