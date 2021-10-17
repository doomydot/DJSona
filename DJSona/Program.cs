using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Discord.Addons.Hosting;
using Discord.WebSocket;
using Discord;
using Discord.Commands;
using DJSona.Services;
using Microsoft.Extensions.DependencyInjection;
using Victoria;

namespace DJSona
{
	class Program
	{
		public static async Task Main()
		{
			Console.ForegroundColor = ConsoleColor.Magenta;
			Console.Write(@"
			_____     ___   _____
			| _  \    |_ | /  ___|
			| | | |    | | \  `--.    ___   _ __    __ _
			| | | |    | |  `--.  \  / _ \ | '_ \  / _` |
			| |/ / /\__/ / /\__/  / | (_) || | | || (_| |
			|___/  \____/  \____ /  \___ / |_| |_| \__,_|


				");

			Console.ForegroundColor = ConsoleColor.Blue;
			Console.Write("Github: ");
			Console.ResetColor();
			Console.Write("doomydot/DJSona");
			Console.Write(" | ");
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write("Version: ");
			Console.ResetColor();
			Console.Write("0.7b");

			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine();

			var builder = new HostBuilder()
				.ConfigureAppConfiguration(x =>
				{
					var configuration = new ConfigurationBuilder()
						.SetBasePath(Directory.GetCurrentDirectory())
						.AddJsonFile("appsettings.json", false, true)
						.Build();

					x.AddConfiguration(configuration);
				})
				.ConfigureLogging(x =>
				{
					x.AddConsole();
					x.SetMinimumLevel(LogLevel.Debug);
				})
				.ConfigureDiscordHost((context, config) =>
				{
					config.SocketConfig = new DiscordSocketConfig
					{
						LogLevel = LogSeverity.Debug,
						AlwaysDownloadUsers = false,
						MessageCacheSize = 200,
					};

					config.Token = context.Configuration["Token"];
				})
				.UseCommandService((context, config) =>
				{
					config.CaseSensitiveCommands = false;
					config.LogLevel = LogSeverity.Debug;
					config.DefaultRunMode = RunMode.Sync;
				})
				.ConfigureServices((context, services) =>
				{
					services
						.AddHostedService<CommandHandler>()
						.AddLavaNode(x =>
						{
							x.SelfDeaf = true;
						});
				})
				.UseConsoleLifetime();



			var host = builder.Build();
			using (host)
			{
				await host.RunAsync();
			}

			


		}

		
	}
}
