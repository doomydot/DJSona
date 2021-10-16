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
	public class Memes : ModuleBase<SocketCommandContext>
	{
		[Command("meme")]
		public async Task Meme(string subreddit = null)
		{
			var client = new HttpClient();
			var result = await client.GetStringAsync($"https://reddit.com/r/{subreddit}" + "memes/random.json?limit=1&&obey_over18=true");
			if (!result.StartsWith("["))
			{
				await Context.Channel.SendMessageAsync("Your memes are in another castle!");
				return;
			}
			await Context.Channel.TriggerTypingAsync();
			JArray arr = JArray.Parse(result);
			JObject post = JObject.Parse(arr[0]["data"]["children"][0]["data"].ToString());

			var builder = new EmbedBuilder()
				.WithImageUrl(post["url"].ToString())
				.WithColor(new Color(218, 139, 240))
				.WithTitle(post["title"].ToString())
				.WithUrl("https://reddit.com" + post["permalink"].ToString())
				.WithFooter($"💬 {post["num_comments"]} ⬆️ {post["ups"]}");

			var embed = builder.Build();
			await Context.Channel.SendMessageAsync(null, false, embed);

		}
	}
}
