using System;
using System.IO;

using Microsoft.Extensions.Configuration;

namespace DiscordDkpBot
{
	public class Program
	{
		public static void Main (string[] args)
		{
			IConfigurationBuilder builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddJsonFile("DiscordSettings.json", optional: true, reloadOnChange: true)
				.AddJsonFile("EqDkpSettings.json", optional: true, reloadOnChange: true);

			IConfigurationRoot configuration = builder.Build();

			Console.WriteLine("Hello World!");
		}
	}
}
