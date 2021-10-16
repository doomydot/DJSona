using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DJSona.Modules
{
	public class Moderation : ModuleBase<SocketCommandContext>
	{
        [Command("purge")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task Purge(int amount)
        {
            await Context.Channel.TriggerTypingAsync();
            var messages = await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync();
            
            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);

            var message = await Context.Channel.SendMessageAsync($"{messages.Count() - 1} messages deleted successfully!");
            await Task.Delay(2500);
            await message.DeleteAsync();

        }
    }
}
