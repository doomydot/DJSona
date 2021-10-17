using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Victoria;

namespace DJSona.Services
{
	public class CommandHandler : DiscordClientService
	{
		private readonly IServiceProvider provider;
		
		private readonly CommandService service;
		private readonly IConfiguration configuration;
		private readonly LavaNode lavaNode;

		public CommandHandler(IServiceProvider provider, DiscordSocketClient client, ILogger<CommandHandler> logger, CommandService service, IConfiguration configuration, LavaNode lavaNode) : base(client, logger)
		{
			this.provider = provider;
			this.service = service;
			this.configuration = configuration;
			this.lavaNode = lavaNode;

		}

		protected override async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			Client.MessageReceived += OnMessageReceived;
			Client.Ready += OnReadyAsync;
			service.CommandExecuted += OnCommandExecuted;

			await service.AddModulesAsync(Assembly.GetEntryAssembly(), provider);
		}

		private async Task OnReadyAsync()
		{
			// Avoid calling ConnectAsync again if it's already connected 
			// (It throws InvalidOperationException if it's already connected).
			if (!lavaNode.IsConnected)
			{
				await lavaNode.ConnectAsync();
			}

			// Other ready related stuff
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
			if (!message.HasStringPrefix(this.configuration["Prefix"], ref argPos) && !message.HasMentionPrefix(Client.CurrentUser, ref argPos)) return;

			var context = new SocketCommandContext(Client, message);
			await service.ExecuteAsync(context, argPos, provider);

		}
	}
}
