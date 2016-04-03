using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Bomberman.Rules.RulePrinters
{
    public class MarkdownPrinter : IRulePrinter
    {
        private readonly string _filePath;

        public MarkdownPrinter(string filePath)
        {
            _filePath = filePath;
        }

        public void PrintRules(IEnumerable<RuleContainer> rules)
        {
            var sb = new StringBuilder();
            foreach (var ruleContainer in rules)
            {
                sb.AppendLine();
                sb.AppendLine();

                sb.Append("### ");
                sb.AppendLine(ruleContainer.GetTitle());
                sb.AppendLine();
                sb.AppendLine(ruleContainer.GetDescription());

                PrintRules(sb, 0, ruleContainer.GetRules());
            }

            if (!File.Exists(_filePath))
                File.Create(_filePath).Close();

            File.WriteAllText(_filePath, sb.ToString(), Encoding.UTF8);
        }

        private void PrintRules(StringBuilder sb, int depth, List<Rule> rules)
        {
            if (rules == null)
                return;

            for (int r = 0; r < rules.Count; r++)
            {
                var rule = rules[r];
                sb.AppendLine();
                for (int i = 0; i < depth; i++)
                {
                    sb.Append("  ");
                }

                sb.Append(r + 1);
                sb.Append(". ");
                sb.Append(rule.RuleDescription);

                PrintRules(sb, depth + 1, rule.SubRules);
            }
        }
    }
}
