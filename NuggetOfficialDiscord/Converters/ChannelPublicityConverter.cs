using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using NuggetOfficial.Discord.Commands.VoiceHubModule.Data;
using System.Threading.Tasks;

namespace NuggetOfficial.Discord.Data.Converters
{
	public class ChannelPublicityConverter : IArgumentConverter<ChannelAccessibility>
	{
		public async Task<Optional<ChannelAccessibility>> ConvertAsync(string value, CommandContext ctx)
		{
			ChannelAccessibility output;

			switch (value.ToLowerInvariant())
			{
				case "":
				case "public":
					output = ChannelAccessibility.Public;
					break;

				case "private":
					output = ChannelAccessibility.Private;
					break;

				case "supporter":
					output = ChannelAccessibility.Supporter;
					break;

				case "hidden":
					output = ChannelAccessibility.Hidden;
					break;

				default:
					output = ChannelAccessibility.Unknown;
					break;
			}

			return await Task.FromResult(output);
		}
	}
}
