using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bomberman.Rules.RulePrinters
{
    public class ConsolePrinter : IRulePrinter
    {
        public void PrintRules(IEnumerable<RuleContainer> rules)
        {
            foreach (var ruleContainer in rules)
            {
                Console.WriteLine();
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(ruleContainer.GetTitle());
                Console.ResetColor();
                Console.WriteLine(ruleContainer.GetDescription());

                PrintRules(0, ruleContainer.GetRules());
            }
        }

        private void PrintRules(int depth, List<Rule> rules)
        {
            if(rules == null)
                return;

            for (int r = 0; r < rules.Count; r++)
            {
                var rule = rules[r];
                Console.WriteLine();
                for (int i = 0; i < depth; i++)
                {
                    Console.Write("   ");
                }

                Console.Write(r + 1);
                Console.Write(". ");
                Console.Write(rule.RuleDescription);

                PrintRules(depth + 1, rule.SubRules);
            }
        }
    }
}
