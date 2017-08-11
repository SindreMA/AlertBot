using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace AlertBot
{
    public class GuildAndUsers
    {
        public SocketGuild Guild { get; set; }
        public List<USerDTO> Users { get; set; }
    }
}
