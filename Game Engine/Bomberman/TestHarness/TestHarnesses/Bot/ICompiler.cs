using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestHarness.TestHarnesses.Bot
{
    public interface ICompiler
    {
        bool HasPackageManager();
        bool RunPackageManager();
        bool RunCompiler();
    }
}
