using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Meta;
using GameEngine.Loggers;
using TestHarness.TestHarnesses.Bot.Compilers;

namespace TestHarness.TestHarnesses.Bot
{
    public class BotCompiler
    {
        private readonly BotMeta _botMeta;
        private readonly string _botDir;
        private readonly ILogger _compileLogger;
        private readonly ICompiler _compiler;

        public BotCompiler(BotMeta botMeta, string botDir, ILogger compileLogger)
        {
            _botMeta = botMeta;
            _botDir = botDir;
            this._compileLogger = compileLogger;

            switch (botMeta.BotType)
            {
                    case BotMeta.BotTypes.JavaScript:
                    _compiler = new JavaScriptCompiler(botMeta, botDir, compileLogger);
                    break;
                    case BotMeta.BotTypes.Java:
                    _compiler = new JavaCompiler(botMeta, botDir, compileLogger);
                    break;
                    case BotMeta.BotTypes.Python2:
                    case BotMeta.BotTypes.Python3:
                    _compiler = new PythonCompiler(botMeta, botDir, compileLogger);
                    break;
                default:
                    _compiler = new DotNetCompiler(botMeta, botDir, compileLogger);
                    break;
            }
        }

        public bool Compile()
        {
            try
            {
                return RunPackageManager() && RunCompiler();
            }
            catch (Exception ex)
            {
                _compileLogger.LogException("Failed to run compiler for bot " + _botMeta.NickName + " in directory " + _botDir, ex);
                return false;
            }
        }

        private bool RunPackageManager()
        {
            return !_compiler.HasPackageManager() || _compiler.RunPackageManager();
        }

        private bool RunCompiler()
        {
            return _compiler.RunCompiler();
        }
    }
}
