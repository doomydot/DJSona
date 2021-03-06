using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DJSona.Modules
{
	public class General : ModuleBase<SocketCommandContext>
	{
        [Command("ping")]
        public async Task Ping()
		{
            await ReplyAsync("Pong!");
		}

        [Command("info")]
        public async Task InfoAsync(SocketGuildUser user = null)
        {
            if (user == null) user = (Context.User as SocketGuildUser);
            await Context.Channel.TriggerTypingAsync();
            var builder = new EmbedBuilder()
                .WithThumbnailUrl(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
                .WithTitle($"{user.Username} Information")
                .WithColor(new Color(218, 139, 240))
                .AddField("Tag", user.Username + "#" + user.Discriminator)
                .AddField("User ID", user.Id, true)
                .AddField("Created at", user.CreatedAt.ToString("dd/MM/yyyy"), true)
                .AddField("Joined at", user.JoinedAt.Value.ToString("dd/MM/yyyy"), true)
                .AddField("Roles", string.Join(" ", user.Roles.Select(x => x.Mention)))
                .WithCurrentTimestamp();
            var embed = builder.Build();
            await Context.Channel.SendMessageAsync(null, false, embed);
        }

        [Command("server")]
        public async Task ServerAsync()
        {
            await Context.Channel.TriggerTypingAsync();
            var builder = new EmbedBuilder()
                .WithThumbnailUrl(Context.Guild.IconUrl)
                .WithTitle($"{Context.Guild.Name} Information")
                .WithColor(new Color(218, 139, 240))
                .AddField("Created at", Context.Guild.CreatedAt.ToString("dd/MM/yyyy"), true)
                .AddField("Users: ", (Context.Guild as SocketGuild).MemberCount, true)
                .AddField("Online: ", (Context.Guild as SocketGuild).Users.Where(x => x.Status != UserStatus.Offline).Count(), true);

            var embed = builder.Build();
            await Context.Channel.SendMessageAsync(null, false, embed);
        }
    }
}
