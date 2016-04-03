using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bomberman.Rules
{
    public interface IRulePrinter
    {
        void PrintRules(IEnumerable<RuleContainer> rules);
    }
}
