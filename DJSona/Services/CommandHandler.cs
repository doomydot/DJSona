using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DJSona.Services
{
	public class CommandHandler : InitializedService
	{
		private readonly IServiceProvider provider;
		private readonly DiscordSocketClient client;
		private readonly CommandService service;
		private readonly IConfiguration configuration;

		public CommandHandler(IServiceProvider provider, DiscordSocketClient client, CommandService service, IConfiguration configuration)
		{
			this.provider = provider;
			this.client = client;
			this.service = service;
			this.configuration = configuration;
		}

		public override async Task InitializeAsync(CancellationToken cancellationToken)
		{
			client.MessageReceived += OnMessageReceived;
			service.CommandExecuted += OnCommandExecuted;
			await service.AddModulesAsync(Assembly.GetEntryAssembly(), provider);
		}

		private async Task OnCommandExecuted(Optional<CommandInfo> commandInfo, ICommandContext commandContext, IResult result)
		{
			if (result.IsSuccess) return;

			await commandContext.Channel.SendMessageAsync(result.ErrorReason);
		}

		private async Task OnMessageReceived(SocketMessage socketMessage)
		{
			if (!(socketMessage is SocketUserMessage message)) return;
			if (message.Source != MessageSource.User) return;

			// Returns if user is not using the bot prefix nor is querying the bot for it's prefix
			var argPos = 0;
			if (!message.HasStringPrefix(this.configuration["Prefix"], ref argPos) && !message.HasMentionPrefix(this.client.CurrentUser, ref argPos)) return;

			var context = new SocketCommandContext(client, message);
			await service.ExecuteAsync(context, argPos, provider);

		}
	}
}
