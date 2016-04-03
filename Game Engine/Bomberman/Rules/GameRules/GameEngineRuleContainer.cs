using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bomberman.Rules.GameRules
{
    public class GameEngineRuleContainer : RuleContainer
    {
        public override string GetTitle()
        {
            return "Game Engine Rules";
        }

        public override string GetDescription()
        {
            return "The following rules describe how the game engine will run and process the game";
        }

        public override List<Rule> GetRules()
        {
            return new List<Rule>()
            {
                {new Rule("The game engine contains the following entities:")
                {
                    SubRules = new List<Rule>()
                    {
                        {new Rule("Indestructible Wall")},
                        {new Rule("Destructible Wall")},
                        {new Rule("Player")},
                        {new Rule("Bomb")},
                        {new Rule("Power Ups")
                        {
                            SubRules = new List<Rule>()
                            {
                                {new Rule("Bomb Bag")},
                                {new Rule("Bomb Radius")},
                                {new Rule("Super Power Up")},
                            }
                        }},
                    }
                }},
                {
                    new Rule("A game block can only have one of the following entities at a single time:")
                    {
                        SubRules = new List<Rule>()
                        {
                            {new Rule("Indestructible Wall")},
                            {new Rule("Destructible Wall")},
                            {new Rule("Player")},
                            {new Rule("Bomb")},
                            {new Rule("Bomb with a player on top after planting")},
                        }
                    }
                },
                {new Rule("Power ups will only be revealed once the destructible wall has been destroyed as a result of a bomb blast.")},
                {new Rule("The game engine will process rounds in the following order:")
                {
                    SubRules = new List<Rule>()
                    {
                        {new Rule("Remove old explosions from the map")},
                        {new Rule("Decrease all bomb timers")},
                        {new Rule("Detonate bombs with a timer value of 0")},
                        {new Rule("Trigger bombs that fall within the explosion range of another bomb")},
                        {new Rule("Mark entities for destruction (Any players within a bomb blast at this moment will be killed)")},
                        {new Rule("Process player commands")},
                        {new Rule("Mark entities for destruction (If a player moved into a bomb blast, they will be killed)")},
                        {new Rule("Apply power ups")},
                        {new Rule("Remove marked (Killed/Destroyed) entities from the map")},
                        {new Rule("Apply player movement bonus")},
                    }
                }},
                {new Rule("A player entity will not able to move to a space containing another entity, with the exception of power ups.")},
                {new Rule("A player can only plant a bomb if they have bombs available in their bomb bag.  Planting a bomb removes a bomb from the bomb bag and will be returned once a bomb explodes.")},
                {new Rule("Two player entities will not be able to move onto the same space during a round, if this does happen the game engine will randomly choose a player whose move will be discarded.")},
                {new Rule("Bombs will start with a timer based on the players current bomb bag. The formula is (bombag size * 3) + 1.  The bomb timers will be capped to 10.")},
                {new Rule("Bomb timers will decrease by 1 every round.")},
                {new Rule("Bomb radius will equal the radius bonus of the player at the time of planting. Obtaining a radius power up afterwards will not increase bomb radius of bombs currently on the map.")},
                {new Rule("Destructible Walls can only be destroyed if they fall within the blast radius of a bomb.")},
                {new Rule("Indestructible Walls will absorb the damage from a bomb and prevent it from continuing past the wall.")},
                {new Rule("Bombs will absorb the damage from other bombs and prevent it from continuing past the bomb, this will however will cause the affected bomb to detonate causing a chain of detonations.")},
                {new Rule("If a player is in the range of a bomb blast radius at the start of the round and is killed as result, their commands for that round will be ignored.")},
                {new Rule("If a player moves into the range of a bomb blast during a round, the player will be killed as a result.")},
                {new Rule("The game engine will be restricted to a certain amount of rounds.  The max rounds for each map will be calculated as follows (map width * map height).")},
                {new Rule("The leader board for the game will be based on the following")
                {
                    SubRules = new List<Rule>()
                    {
                        {new Rule("Players alive")},
                        {new Rule("Then points for the players")},
                        {new Rule("Then the round the players were killed")},
                    }
                }},
            };
        }
    }
}
