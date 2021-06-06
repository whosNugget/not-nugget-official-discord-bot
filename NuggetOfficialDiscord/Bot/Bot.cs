using System.Threading.Tasks;

namespace NuggetDiscordBot.Bot
{
	public abstract class DiscordBot
	{
		public abstract Task Run(string botToken);
	}
}
