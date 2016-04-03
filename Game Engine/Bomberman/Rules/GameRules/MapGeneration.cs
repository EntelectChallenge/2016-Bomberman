using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bomberman.Rules.GameRules
{
    public class MapGeneration : RuleContainer
    {
        public override string GetTitle()
        {
            return "Map Generation";
        }

        public override string GetDescription()
        {
            return
                "The maps in the game will be generated randomly based on seed provided to the game engine.  The game seed will be random for each match, but can be the same if matches need to be re-run.  The map will be divided into four quadrants for generation purposes.";
        }

        public override List<Rule> GetRules()
        {
            return new List<Rule>()
            {
                {new Rule("The map will be surrounded with indestructible walls")},
                {new Rule("The default map size for 2-4 players will be 21x21 blocks")},
                {new Rule("Every second square, starting from the outer boundary, will be an indestructible wall.  The only exception to this rule will be the centre block on the map.")},
                {new Rule("Each quadrant will be generated such that the entire map will be symmetrical, with each quadrant appearing the same from each users perspective.")},
                {new Rule("Players will always be placed in a corner of the map.  In case the map contains more than 4 players, the remaining players be placed equidistant from the other players along the sides of the map.")},
                {new Rule("Every player on the map will have a 2 block safe zone horizontally and vertically.")},
                {new Rule("The centre of the map will always contain a Super power up in the centre, in place of the indestructible wall.")},
                {new Rule("The centre power up will always be surrounded by a 5x5 area of destructible walls.")},
                {new Rule("Power ups will be placed randomly across the map, with each quadrant of the map receiving the same amount and type of power ups.  When four players are present, a fairness algorithm will be applied to ensure players have the same chance of finding a power up within a certain distance from them.")},
                {
                    new Rule("Power ups on the map will be determined with the following algorithm")
                    {
                        SubRules = new List<Rule>()
                        {
                            {new Rule("Two bomb bag power ups will be placed on the map per player.")},
                            {new Rule("Four bomb radius power ups will be placed on the map per player.")}
                        }
                    }
                },
                {new Rule("The tournament will only have 2 or 4 players per map.  But in some scenarios more players will be placed on the maps, in which case the map size (Width/Height) will dynamically change to accommodate more players.")}
            };
        }
    }
}
