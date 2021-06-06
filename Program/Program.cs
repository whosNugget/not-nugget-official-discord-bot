using NuggetDiscordBot;
using NuggetTwitchIntegration;
using System.Configuration;
using System.Threading.Tasks;

class Program
{
	static NuggetBot bot;

	static async Task Main()
	{
		new NuggetOfficial(ConfigurationManager.AppSettings.Get("twitchClientId")); //Singleton instance set in constructor

		bot = new NuggetBot(ConfigurationManager.AppSettings.Get("voiceDataPath"));
		await bot.Run(ConfigurationManager.AppSettings.Get("botToken"));
	}
}
