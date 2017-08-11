using System;
using System.Collections.Generic;
using System.Text;

namespace AlertBot
{
    class ChannelAndGuild
    {
        public ulong ChannelID { get; set; }
        public ulong GuildID { get; set; }
        public bool allstatus { get; set; }
        public string prefix { get; set; }
    }
}
