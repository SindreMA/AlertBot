using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace AlertBot
{
    public class USerDTO
    {
        public ulong UserID { get; set; }
        public string Username { get; set; }
        public UserStatus Status { get; set; }
    }
}
