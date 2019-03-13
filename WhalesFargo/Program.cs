using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using WhalesFargo.UI;

namespace WhalesFargo
{
	public class Program
	{
		// Create a mutex for a single instance.
		private static readonly Mutex INSTANCE_MUTEX = new Mutex(true, "WhalesFargo_DiscordBot");
		private static readonly DiscordBot BOT = new DiscordBot();
		public static Window UI = new Window(BOT);

		private static void Main (string[] args)
		{
			// Check if an instance is already running. Remove this block if you want to run multiple instances.
			if (!INSTANCE_MUTEX.WaitOne(TimeSpan.Zero, false))
			{
				MessageBox.Show("The applicaton is already running.");
				return;
			}
			// Start the UI.
			try
			{
				Application.Run(UI);
			}
			catch
			{
				Console.WriteLine("Failed to run.");
			}
		}

		// Connect to the bot, or cancel before the connection happens.
		public static void Run () => Task.Run(() => BOT.RunAsync());
		public static void Cancel () => Task.Run(() => BOT.CancelAsync());
		public static void Stop () => Task.Run(() => BOT.StopAsync());
	}
}
