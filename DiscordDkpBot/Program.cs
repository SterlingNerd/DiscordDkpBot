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
			IConfigurationRoot configuration = GetConfiguration();

			ServiceProvider services = ConfigureServices(configuration);

			await services.GetRequiredService<DkpBot>().Run();
		}

		private static ServiceProvider ConfigureServices (IConfigurationRoot configuration)
		{
			return new ServiceCollection()
				.AddSingleton(new CommandCollection(
					new PingCommand())
				)
				.AddSingleton<ICommandProcessor, CommandProcessor>()
				.AddTransient<DiscordSocketClient>()
				.AddSingleton<DkpBot>()
				.AddLogging(c =>
							{
								c.AddConfiguration(configuration.GetSection("Logging"));
								c.AddConsole();
							})
				.AddSingleton(GetBotConfiguration(configuration))
				.BuildServiceProvider();
		}

		private static DkpBotConfiguration GetBotConfiguration (IConfigurationRoot configuration)
		{
			DkpBotConfiguration botConfig = new DkpBotConfiguration();
			configuration.GetSection("DkpBot").Bind(botConfig);
			return botConfig;
		}

		private static IConfigurationRoot GetConfiguration ()
		{
			IConfigurationBuilder builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", true, false)
				.AddJsonFile("AuthSettings.json", false, false);
			return builder.Build();
		}
	}
}
