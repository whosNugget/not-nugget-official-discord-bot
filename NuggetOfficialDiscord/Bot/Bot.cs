﻿using System.Threading.Tasks;

namespace NuggetOfficial.Discord.Bot
{
	public abstract class DiscordBot
	{
		public abstract Task Run(string botToken);
	}
}
