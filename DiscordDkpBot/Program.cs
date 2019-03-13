using System;
using System.IO;
using System.Threading.Tasks;

using Discord.WebSocket;

using DiscordDkpBot.Commands;
using DiscordDkpBot.Configuration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DiscordDkpBot
{
	public class Program
	{
		public static async Task Main (string[] args)
		{
			DkpBotConfiguration configuration = GetConfiguration();

			ServiceProvider services = ConfigureServices(configuration);

			await services.GetRequiredService<DkpBot>().Run();
		}

		private static ServiceProvider ConfigureServices (DkpBotConfiguration configuration)
		{
			return new ServiceCollection()
				.AddSingleton(new CommandCollection(
					new PingCommand())
				)
				.AddSingleton<ICommandProcessor, CommandProcessor>()
				.AddTransient<DiscordSocketClient>()
				.AddSingleton<DkpBot>()
				.AddSingleton(configuration)
				.AddLogging(c => { c.AddConsole(); })
				.BuildServiceProvider();
		}

		private static DkpBotConfiguration GetConfiguration ()
		{
			IConfigurationBuilder builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", true, false)
				.AddJsonFile("AuthSettings.json", false, false);

			DkpBotConfiguration configuration = new DkpBotConfiguration();
			builder.Build().Bind(configuration);
			return configuration;
		}
	}
}
