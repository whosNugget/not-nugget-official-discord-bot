using System.Threading.Tasks;

namespace NuggetOfficial.Bot
{
	public abstract class DiscordBot
	{
		public abstract Task Run(string botToken);
	}
}
