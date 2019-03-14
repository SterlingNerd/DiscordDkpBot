using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Discord.WebSocket;

using DiscordDkpBot.Auctions;
using DiscordDkpBot.Commands;
using DiscordDkpBot.Configuration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DiscordDkpBot
{
	public static class Program
	{
		public static async Task Main (string[] args)
		{
			IConfigurationRoot configuration = GetConfiguration();

			ServiceProvider services = ConfigureServices(configuration);

			await services.GetRequiredService<DkpBot>().Run();
		}

		private static IServiceCollection AddChatCommands (this IServiceCollection services)
		{
			// Add all implemntations of IChatCommand to DI.
			foreach (TypeInfo type in Assembly.GetCallingAssembly().DefinedTypes
				.Where(x => x.ImplementedInterfaces.Contains(typeof(ICommand)) && x.IsAbstract == false))
			{
				services.AddSingleton(typeof(ICommand), type);
			}
			return services;
		}

		private static IServiceCollection AddDiscordNet (this IServiceCollection services)
		{
			return services.AddTransient<DiscordSocketClient>();
		}

		private static IServiceCollection AddDkpBot (this IServiceCollection services, IConfigurationSection dkpBotConfiguration)
		{
			return
				services
					.AddSingleton(GetBotConfiguration(dkpBotConfiguration))
					.AddChatCommands()
					.AddSingleton<ICommandProcessor, CommandProcessor>()
					.AddSingleton<IAuctionProcessor, AuctionProcessor>()
					.AddSingleton<AuctionState>()
					.AddSingleton<DkpBot>()
					.AddDiscordNet()
				;
		}

		private static IServiceCollection AddLogging (this IServiceCollection services, IConfigurationSection loggingConfiguration)
		{
			return services.AddLogging(c =>
										{
											c.AddConfiguration(loggingConfiguration);
											c.AddConsole();
											c.AddFile();
										});
		}

		private static ServiceProvider ConfigureServices (IConfigurationRoot configuration)
		{
			return new ServiceCollection()
				.AddLogging(configuration.GetSection("Logging"))
				.AddDkpBot(configuration.GetSection("DkpBot"))
				.BuildServiceProvider();
		}

		private static DkpBotConfiguration GetBotConfiguration (IConfigurationSection configuration)
		{
			DkpBotConfiguration botConfig = new DkpBotConfiguration();
			configuration.Bind(botConfig);
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
