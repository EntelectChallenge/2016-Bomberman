using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bomberman.Rules.GameRules
{
    public class PlayerRulesContainer : RuleContainer
    {
        public override string GetTitle()
        {
            return "Player Rules";
        }

        public override string GetDescription()
        {
            return
                "Players can either be consol players or bots.  Both follow the same game engine rules.  When playing on Unity, the rules will follow the actual game as close as possible, with exceptions made for real time play.";
        }

        public override List<Rule> GetRules()
        {
            return new List<Rule>()
            {
                {new Rule("Players will only be able submit one command per round.  The game engine will reject any additional commands sent by the player.")},
                {new Rule("Only one of the following commands can be submitted by the player during a round:")
                {
                    SubRules = new List<Rule>()
                    {
                        {new Rule("Move Command – Left, Right, Up, Down.")},
                        {new Rule("Place Bomb Command – Places a bomb underneath the player.")},
                        {new Rule("Reduce Bomb Timer – Reduces the timer of the bomb with the lowest timer for the player to 1.")},
                        {new Rule("Do Nothing Command – Player skips the round and remains on the same block.")},
                    }
                }},
                {new Rule("Players will start with a bomb bag containing 1 bomb.")},
                {new Rule("Players will start with a bomb radius of 1.")},
                {new Rule("Players will start with a bomb timer of 4 rounds.")},
                {new Rule("Bot players will have the following additional rules")
                {
                    SubRules = new List<Rule>()
                    {
                        {new Rule("Bot processes will be terminated after 4 seconds")},
                        {new Rule("Bots will not be allowed to exceed a total processor time of 2 seconds")},
                        {new Rule("Bots processes will run with elevated processor priority. (For this reason the game has to be run with administrator privileges)")},
                        {new Rule("Calibrations will be done at the start of a game to determine additional processor time.  So if the calibration bot takes 200ms to read the files and make a move descision then your bot will be allowed an additional 200ms to complete.")},
                        {new Rule("Malfunctioning bots or bots that exceed their time limit will send back a do nothing command.")},
                        {new Rule("Bot players that post more than 20 do nothing commands in a row will automatically place a bomb to kill themselves in an attempt to save the game")}
                    }
                }},
            };
        }
    }
}
