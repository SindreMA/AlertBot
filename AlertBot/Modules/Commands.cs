using AlertBot;
using Discord.Commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TemplateBot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("Alerthere")]
        public async Task alert()
        {
            if (CommandHandler.List.Exists(x => x.GuildID == Context.Guild.Id ))
            {
                await Context.Channel.SendMessageAsync("There is already a room with alert on!");
            }
            else
            {
                var list = new ChannelAndGuild();
                list.ChannelID = Context.Channel.Id;
                list.GuildID = Context.Guild.Id;
                CommandHandler.List.Add(list);
                await Context.Channel.SendMessageAsync("Alert to channel have been activated");
            }

        }
        [Command("prefix")]
        public async Task prefix([Optional][Remainder]string prefix)
        {
            if (CommandHandler.List.Exists(x => x.GuildID == Context.Guild.Id && x.ChannelID == Context.Channel.Id))
            {
                var item = CommandHandler.List.Find(x => x.GuildID == Context.Guild.Id && x.ChannelID == Context.Channel.Id);
                if (prefix != null)
                {

                    item.prefix = prefix;
                    await Context.Channel.SendMessageAsync("Prefix have been set!");
                }
                else
                {
                    item.prefix = prefix;
                    await Context.Channel.SendMessageAsync("Prefix have been removed!");
                }

            }
            else
            {
                await Context.Channel.SendMessageAsync("Guild have no links");
            }

        }
        [Command("reportallstatus")]
        public async Task status()
        {
            if (CommandHandler.List.Exists(x => x.GuildID == Context.Guild.Id && x.ChannelID == Context.Channel.Id))
            {
                var item = CommandHandler.List.Find(x => x.GuildID == Context.Guild.Id && x.ChannelID == Context.Channel.Id);
                if (item.allstatus)
                {
                    item.allstatus = false;
                    await Context.Channel.SendMessageAsync("Alerting on only online/offline changes!");

                }
                else
                {
                    item.allstatus = true;
                    await Context.Channel.SendMessageAsync("Alerting on all status changes!");
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("Guild have no links");
            }

        }
        [Command("showlinks")]
        public async Task showlinks()
        {
            if (CommandHandler.List.Exists(x => x.GuildID == Context.Guild.Id))
            {

                foreach (var item in   CommandHandler.List.FindAll(x => x.GuildID == Context.Guild.Id))
                {
                    await Context.Channel.SendMessageAsync("```" +JsonConvert.SerializeObject(item, Formatting.Indented)+"```");
                    

                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("Guild have no links");
            }
        }


    }
}
