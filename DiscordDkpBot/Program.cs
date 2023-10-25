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
using DiscordDkpBot.Dkp.NullDkp;
using DiscordDkpBot.Items;
using DiscordDkpBot.Items.Allakhazam;
using DiscordDkpBot.Items.Wowhead;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NLog;
using NLog.Extensions.Logging;

using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace DiscordDkpBot
{
	public static class Program
	{
		public static async Task Main (string[] args)
		{
			var logger = LogManager.GetCurrentClassLogger();

			try
			{
				logger.Info("Configuring...");
				IConfigurationRoot configuration = GetConfiguration();
				IServiceProvider services = ConfigureServices(configuration, args);

				logger.Info("Starting Up...");

				await services.GetRequiredService<DkpBot>().Run();
			} catch (Exception ex)
			{
				logger.Error(ex, "Unexpected Exception");
				throw;
			} finally
			{
				LogManager.Shutdown();
			}
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

		private static IServiceCollection AddDiscordNet (this IServiceCollection services)
		{
			return services.AddTransient<DiscordSocketClient>();
		}

		private static IServiceCollection AddDkpBot (this IServiceCollection services, IConfigurationSection dkpBotConfiguration, string[] args)
		{
			DkpBotConfiguration config = GetBotConfiguration(dkpBotConfiguration);

			config.Discord.Token = string.IsNullOrWhiteSpace(config.Discord.Token) ? args?.FirstOrDefault() : config.Discord.Token;
			if (string.IsNullOrWhiteSpace(config.Discord.Token))
			{
				throw new ApplicationException("Discord token required.");
			}
			
			config.EqDkpPlus.Token = string.IsNullOrWhiteSpace(config.EqDkpPlus.Token) ? args?.Skip(1).FirstOrDefault() : config.EqDkpPlus.Token;

			services.AddSingleton(config);
			services.AddSingleton(config.EqDkpPlus);
			services.AddSingleton(config.Discord);
			services.AddSingleton(config.Ranks);
			services.AddHttpClient();
			services.AddChatCommands();

			if (!string.IsNullOrWhiteSpace(config.EqDkpPlus.Token))
			{
				services.AddSingleton<EqDkpPlusClient>();
				services.AddSingleton<IDkpProcessor, EqDkpPlusProcessor>();
			} else
			{
				services.AddSingleton<IDkpProcessor, NullDkpProcessor>();
			}

			services.AddSingleton<IAttendanceParser, RaidWindowParser>();
			services.AddItemSource(config);
			services.AddDefaultImplementations();
			services.AddSingleton<AuctionState>();
			services.AddSingleton<EqDkpState>();
			services.AddSingleton<DkpBot>();
			services.AddDiscordNet();

			return services;
		}

		private static IServiceCollection AddItemSource (this IServiceCollection services, DkpBotConfiguration config)
		{
			if (config.ItemSource.Equals("Wowhead", StringComparison.OrdinalIgnoreCase))
			{
				services.AddSingleton<IItemSource, WowheadItemSource>();
			} else if (config.ItemSource.Equals("WowheadClassic", StringComparison.OrdinalIgnoreCase))
			{
				services.AddSingleton<IItemSource, WowheadClassicItemSource>();
			} else
			{
				services.AddSingleton<IItemSource, AllakhazamItemSource>();
			}
			return services;
		}

		private static ServiceProvider ConfigureServices (IConfigurationRoot configuration, string[] args)
		{
			return new ServiceCollection()
				.AddLogging(ConfigureLogging)
				.AddDkpBot(configuration.GetSection("DkpBot"), args)
				.BuildServiceProvider();
		}

		private static void ConfigureLogging (ILoggingBuilder loggingBuilder)
		{
			loggingBuilder.ClearProviders();
			loggingBuilder.SetMinimumLevel(LogLevel.Trace);
			loggingBuilder.AddNLog();
			loggingBuilder.AddConsole();
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
				.AddJsonFile("authsettings.json", true, false)
				.AddJsonFile("AuthSettings.json", true, false);

			return builder.Build();
		}
	}
}
