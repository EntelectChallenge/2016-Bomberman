using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Bomberman.Rules.RulePrinters
{
    public class HtmlPrinter  : IRulePrinter
    {
        private readonly string _filePath;

        public HtmlPrinter(string filePath)
        {
            _filePath = filePath;
        }

        public void PrintRules(IEnumerable<RuleContainer> rules)
        {
            bool isFirst = true;
            var sb = new StringBuilder();
            sb.Append("<ul class=\"accordion\" data-accordion style=\"width:100%\">");

            foreach (var ruleContainer in rules)
            {
                if(isFirst) sb.AppendLine("<li class=\"accordion-item is-active\" data-accordion-item>");
                else sb.AppendLine("<li class=\"accordion-item\" data-accordion-item>");

                sb.Append("<a href=\"javascript:void(0)\" class=\"accordion-title\">");
                sb.Append(ruleContainer.GetTitle());
                sb.AppendLine("</a>");
                sb.AppendLine("<div class=\"accordion-content\" data-tab-content>");

                sb.Append("<p>");
                sb.Append(ruleContainer.GetDescription());
                sb.AppendLine("</p>");

                PrintRules(sb, 0, ruleContainer.GetRules());

                sb.AppendLine("</div>");
                sb.AppendLine("</li>");
                isFirst = false;
            }
            sb.Append("</ul>");

            if (!File.Exists(_filePath))
                File.Create(_filePath).Close();

            File.WriteAllText(_filePath, sb.ToString(), Encoding.UTF8);
        }

        private void PrintRules(StringBuilder sb, int depth, List<Rule> rules)
        {
            if (rules == null)
                return;

            sb.AppendLine();
            sb.AppendLine(depth > 0 ? "<ol type=\"a\">" : "<ol type=\"1\">");
            for (int r = 0; r < rules.Count; r++)
            {
                var rule = rules[r];
                sb.Append("<li>");
                sb.Append(rule.RuleDescription);

                PrintRules(sb, depth + 1, rule.SubRules);
                sb.AppendLine("</li>");
            }

            sb.AppendLine("</ol>");
        }
    }
}
