using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domain.Meta
{
    public class RoundInfo
    {
        public int MapSeed { get; set; }
        public int Round { get; set; }
        public PlayerInfo Winner { get; set; }
        public IEnumerable<PlayerInfo> LeaderBoard { get; set; }

        public IEnumerable<PlayerInfo> Players { get; set; }
    }

    public class PlayerInfo
    {
        public string Author { get; set; }
        public string NickName { get; set; }
        public string Email { get; set; }
        public char PlayerKey { get; set; }
        public string BotType { get; set; }
    }
}
