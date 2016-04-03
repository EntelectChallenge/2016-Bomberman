using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Domain.Meta;
using Newtonsoft.Json;
using TestHarness.Properties;

namespace TestHarness.Util
{
    public class BotMetaReader
    {
        public static BotMeta ReadBotMeta(String botLocation)
        {
            var metaLocation = Path.Combine(botLocation, Settings.Default.BotMetaFileName);

            if(!File.Exists(metaLocation))
                throw new FileNotFoundException("No bot meta file found at location " + metaLocation);

            var fileContent = File.ReadAllText(metaLocation);

            var meta = JsonConvert.DeserializeObject<BotMeta>(fileContent);
            meta.ProjectLocation = meta.ProjectLocation.Replace(@"\\", Path.DirectorySeparatorChar.ToString()).Replace(@"\", Path.DirectorySeparatorChar.ToString());
            meta.RunFile = meta.RunFile.Replace(@"\\", Path.DirectorySeparatorChar.ToString()).Replace(@"\", Path.DirectorySeparatorChar.ToString());
            return meta;
        }
    }
}
