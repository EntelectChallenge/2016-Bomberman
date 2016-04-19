using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Domain.Meta
{
    public class BotMeta
    {
        public string Author { get; set; }
        public string Email { get; set; }
        public string NickName { get; set; }
        public BotTypes BotType { get; set; }
        public string ProjectLocation { get; set; }
        public string RunFile { get; set; }
        public String RunArgs { get; set; }

        public enum BotTypes
        {
            CSharp,
            Java,
            JavaScript,
            CPlusPlus,
            Python2,
            Python3,
            FSharp
        }
    }
}
