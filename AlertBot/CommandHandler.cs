using System;
using Discord;
using System.Collections.Generic;
using System.Text;
using Discord.WebSocket;
using Discord.Commands;
using System.Linq;

using System.Reflection;
using System.Threading.Tasks;
using AlertBot;
using System.Threading;
using Newtonsoft.Json;
using System.IO;

namespace TemplateBot
{
    class CommandHandler
    {
        public System.Threading.Timer _timer;
        public static List<GuildAndUsers> guildAndUsers = new List<GuildAndUsers>();

        public static List<ChannelAndGuild> List = new List<ChannelAndGuild>();
        private DiscordSocketClient _client;
        private CommandService _service;
        public CommandHandler(DiscordSocketClient client)
        {
            _timer = new System.Threading.Timer(Callback, true, 1000, System.Threading.Timeout.Infinite);

            _client = client;
            _service = new CommandService();
            _service.AddModulesAsync(Assembly.GetEntryAssembly());
            _client.MessageReceived += _client_MessageReceived;
            try
            {

                string json = File.ReadAllText("Alerts.json");
                List = JsonConvert.DeserializeObject<List<ChannelAndGuild>>(json);
            }
            catch (Exception)
            {
            }
        }
        private void Callback(Object state)
        {
            TimerEvent();
            _timer.Change(1000, Timeout.Infinite);
        }
        private void TimerEvent()
        {
            foreach (var guild in _client.Guilds)
            {
                if (guildAndUsers.Exists(x => x.Guild.Id == guild.Id))
                {
                    var thing = guildAndUsers.Single(x => x.Guild.Id == guild.Id);
                    foreach (var user in guild.Users)
                    {
                        if (!thing.Users.Exists(x => x.UserID == user.Id))
                        {
                            USerDTO u = new USerDTO();
                            u.UserID = user.Id;
                            u.Username = user.Username;
                            u.Status = user.Status;
                            thing.Users.Add(u);

                        }
                        else
                        {
                            var thinguser = thing.Users.Single(x => x.UserID == user.Id);
                            if (thinguser.Status != user.Status)
                            {
                                _client_UserUpdated(thinguser, user,guild.Id);
                                thinguser.Status = user.Status;
                            }
                        }
                    }
                    
                }
                else
                {

                    GuildAndUsers Item = new GuildAndUsers();
                    Item.Guild = guild;
                    List<USerDTO> newitup = new List<USerDTO>();
                    foreach (var user in guild.Users)
                    {
                        USerDTO u = new USerDTO();
                        u.UserID = user.Id;
                        u.Username = user.Username;
                        u.Status = user.Status;
                        newitup.Add(u);
                    }
                    Item.Users = newitup;
                    guildAndUsers.Add(Item);
                }

                

            }
        }
      
        

        private async Task _client_UserUpdated(USerDTO arg1, SocketUser arg2, ulong GuildID)
        {
            
                foreach (var item in List)
                {
                    if (GuildID == item.GuildID)
                    {
                    if (((arg1.Status == UserStatus.Offline || arg1.Status == UserStatus.Invisible) && (arg2.Status == UserStatus.Online || arg2.Status == UserStatus.Idle || arg2.Status == UserStatus.AFK || arg2.Status == UserStatus.DoNotDisturb)) || item.allstatus)
                    {
                        try
                        {
                            var channel = _client.GetChannel(item.ChannelID) as SocketTextChannel;
                            if (item.prefix != null)
                            {
                                await channel.SendMessageAsync(item.prefix.Replace("[USER]",arg1.Username).Replace("[STATUS1]",arg1.Status.ToString()).Replace("[STATUS2]",arg2.Status.ToString()));
                            }
                            else
                            {
                                await channel.SendMessageAsync(arg1.Username + " went from status " + arg1.Status.ToString() + " to " + arg2.Status.ToString());
                            }
                            
                        }
                        catch (Exception)
                        {
                            await Program.Log("Cant find channel!", ConsoleColor.Red);

                        }
                        

                    }

                }
            }
        }

        private async Task _client_MessageReceived(SocketMessage arg)
        {
            var msg = arg as SocketUserMessage;
            if (msg == null) return;

            var context = new SocketCommandContext(_client, msg);
            int argPost = 0;
            if (msg.HasCharPrefix('.',ref argPost))
            {
                var result = _service.ExecuteAsync(context, argPost);
                if (!result.Result.IsSuccess && result.Result.Error != CommandError.UnknownCommand)
                {
                    await context.Channel.SendMessageAsync(result.Result.ErrorReason);
                }
				      await Program.Log("Invoked " + msg + " in " + context.Channel + " with " + result.Result, ConsoleColor.Magenta);
                Save();
            }
            else
            {
                await Program.Log(context.Channel + "-" + context.User.Username + " : " + msg, ConsoleColor.White);
            }
            
        }
        public void Save()
        {
            File.WriteAllText("Alerts.json", JsonConvert.SerializeObject(CommandHandler.List,Formatting.Indented));
        }
    }
}
