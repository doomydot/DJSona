using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DJSona
{
	public class Startup
	{
		public IConfigurationRoot Configuration { get; }

		public Startup(string[] args)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(AppContext.BaseDirectory)
				.AddYamlFile("_config.yml"));
			Configuration = builder.Build();
		}

		public static async Task RunAsync(string[] args)
		{
			var startup = new Startup(args);
			await startup.StartAsync();
		}

		public async Task RunAsync()
		{
			var services = new ServiceCollection();
			ConfigureServices(services);

			var provider = services.BuildServiceProvider();
			provider.GetRequiredService<CommandHandler>();

			await provider.GetRequiredService<StartupService>().StartAsync();
			await Task.Delay(-1);
		}
	}
}
