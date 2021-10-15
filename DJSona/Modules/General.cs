using Discord.Commands;
using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using System.Linq;


namespace DJSona.Modules
{
    public class General : ModuleBase
    {
        [Command("ping")]
        public async Task Ping()
        {
            await Context.Channel.SendMessageAsync("pong");
        }

        [Command("info")]
        public async Task Info(SocketGuildUser user = null)
        {
            if(user == null) user = (Context.User as SocketGuildUser);
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

        [Command("purge")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task Purge(int amount)
        {
            var messages = await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync();
            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);

            var message = await Context.Channel.SendMessageAsync($"{messages.Count() - 1} messages deleted successfully!");
            await Task.Delay(2500);
            await message.DeleteAsync();
        
        }

        [Command("server")]
        public async Task Server()
        {
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