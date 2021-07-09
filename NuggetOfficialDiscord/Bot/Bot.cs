using System.Threading.Tasks;

namespace NuggetOfficial.Discord.Bot
{
	//TODO research the whole "host" thing and see if this can be run in its own service. Additionally, allow the calling
	//application terminate that service across executions. Finally, should it ever be necessary, research how to communicate
	//with another host and see if its possible to request those hosts to start the service instead, if it is installed

	public abstract class DiscordBot
	{
		public abstract Task Run(string botToken);
	}
}
