using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Discord.WebSocket;

using DiscordDkpBot.Auctions;
using DiscordDkpBot.Commands;
using DiscordDkpBot.Configuration;
using DiscordDkpBot.Dkp;
using DiscordDkpBot.Dkp.EqDkpPlus;
using DiscordDkpBot.Items;
using DiscordDkpBot.Items.Allakhazam;

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
			// Add all implementations of IChatCommand to DI.
			foreach (TypeInfo type in Assembly.GetCallingAssembly().DefinedTypes
				.Where(x => x.ImplementedInterfaces.Contains(typeof(IDmCommand)) && x.IsAbstract == false))
			{
				services.AddSingleton(typeof(IDmCommand), type);
			}

			foreach (TypeInfo type in Assembly.GetCallingAssembly().DefinedTypes
				.Where(x => x.ImplementedInterfaces.Contains(typeof(IChannelCommand)) && x.IsAbstract == false))
			{
				services.AddSingleton(typeof(IChannelCommand), type);
			}

			return services;
		}

		private static IServiceCollection AddDiscordNet (this IServiceCollection services)
		{
			return services.AddTransient<DiscordSocketClient>();
		}

		private static IServiceCollection AddDkpBot (this IServiceCollection services, IConfigurationSection dkpBotConfiguration)
		{
			DkpBotConfiguration config = GetBotConfiguration(dkpBotConfiguration);

			return services
				.AddSingleton(config)
				.AddSingleton(config.EqDkpPlus)
				.AddSingleton(config.Discord)
				.AddSingleton(config.Ranks)
				.AddHttpClient()
				.AddChatCommands()
				.AddSingleton<EqDkpPlusClient>()
				.AddSingleton<IDkpProcessor, EqDkpPlusProcessor>()
				.AddSingleton<IItemSource, AllakhazamItemSource>()
				.AddDefaultImplementations()
				.AddSingleton<AuctionState>()
				.AddSingleton<DkpBot>()
				.AddDiscordNet()
				;
		}

		private static IServiceCollection AddDefaultImplementations (this IServiceCollection services)
		{
			Assembly coreAssembly = Assembly.GetAssembly(typeof(Program));

			// If there's an interface
			foreach (Type interfaceType in coreAssembly.GetTypes().Where(x => x.IsInterface))
			{
				// strip the "I" off after the last . in it's full name
				string defaultImplementationName = interfaceType.FullName.Remove(interfaceType.FullName.LastIndexOf('.') + 1, 1);
				Type implementationType = coreAssembly.GetType(defaultImplementationName);
				if (implementationType != null)
				{
					// If we found one, inject it!
					services.AddSingleton(interfaceType, implementationType);
				}
			}

			return services;
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
