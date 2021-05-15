using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NuggetOfficial.Data.Converters
{
	public class ChannelPublicityConverter : IArgumentConverter<ChannelPublicity>
	{
		public Task<Optional<ChannelPublicity>> ConvertAsync(string value, CommandContext ctx)
		{
			Optional<ChannelPublicity> output;

			switch (value.ToLowerInvariant())
			{
				case "":
				case "public":
					output = Optional.FromValue(ChannelPublicity.Public);
					break;

				case "private":
					output = Optional.FromValue(ChannelPublicity.Private);
					break;

				case "supporter":
					output = Optional.FromValue(ChannelPublicity.Supporter);
					break;

				case "hidden":
					output = Optional.FromValue(ChannelPublicity.Hidden);
					break;

				default:
					output = Optional.FromValue(ChannelPublicity.Unknown);
					break;
			}

			return Task.FromResult(output);
		}
	}
}
