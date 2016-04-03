using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bomberman.Rules
{
    public abstract class RuleContainer
    {
        public abstract String GetTitle();
        public abstract String GetDescription();
        public abstract List<Rule> GetRules();
    }

    public class Rule
    {
        public String RuleDescription { get; set; }
        public List<Rule> SubRules { get; set; }

        public Rule()
        {
        }

        public Rule(String description)
        {
            RuleDescription = description;
        }
    }
}
