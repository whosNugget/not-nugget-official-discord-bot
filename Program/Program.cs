using NuggetOfficial.Discord;
using System.Configuration;
using System.Threading.Tasks;

namespace NuggetOfficial
{
	class Program
	{
		static NuggetBot bot;

		static async Task Main()
		{
			bot = new NuggetBot(ConfigurationManager.AppSettings.Get("voiceDataPath"));
			await bot.Run(ConfigurationManager.AppSettings.Get("botToken"));
		}
	}
}
