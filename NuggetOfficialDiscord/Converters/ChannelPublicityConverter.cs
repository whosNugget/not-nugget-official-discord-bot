using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using NuggetOfficial.Discord.Commands.VoiceHubModule.Data;
using System.Threading.Tasks;

namespace NuggetOfficial.Discord.Data.Converters
{
	public class ChannelPublicityConverter : IArgumentConverter<ChannelAccessibility>
	{
		public Task<Optional<ChannelAccessibility>> ConvertAsync(string value, CommandContext ctx)
		{
			Optional<ChannelAccessibility> output;

			switch (value.ToLowerInvariant())
			{
				case "":
				case "public":
					output = Optional.FromValue(ChannelAccessibility.Public);
					break;

				case "private":
					output = Optional.FromValue(ChannelAccessibility.Private);
					break;

				case "supporter":
					output = Optional.FromValue(ChannelAccessibility.Supporter);
					break;

				case "hidden":
					output = Optional.FromValue(ChannelAccessibility.Hidden);
					break;

				default:
					output = Optional.FromValue(ChannelAccessibility.Unknown);
					break;
			}

			return Task.FromResult(output);
		}
	}
}
