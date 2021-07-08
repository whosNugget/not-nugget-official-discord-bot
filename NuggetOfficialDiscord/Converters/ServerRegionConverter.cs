using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using NuggetOfficial.Discord.Commands.VoiceHubModule.Data;
using System.Threading.Tasks;

namespace NuggetOfficial.Discord.Data.Converters
{
	public class ServerRegionConverter : IArgumentConverter<VoiceRegion>
	{
		public async Task<Optional<VoiceRegion>> ConvertAsync(string value, CommandContext ctx)
		{
			VoiceRegion output;

			switch (value.ToLowerInvariant())
			{
				case "":
				case "auto":
				case "automatic":
					output = VoiceRegion.Automatic;
					break;

				case "brazil":
					output = VoiceRegion.Brazil;
					break;

				case "europe":
					output = VoiceRegion.Europe;
					break;

				case "hongkong":
					output = VoiceRegion.HongKong;
					break;

				case "india":
					output = VoiceRegion.India;
					break;

				case "japan":
					output = VoiceRegion.Japan;
					break;

				case "russia":
					output = VoiceRegion.Russia;
					break;

				case "singapore":
					output = VoiceRegion.Singapore;
					break;

				case "southafrica":
					output = VoiceRegion.SouthAfrica;
					break;

				case "sydney":
					output = VoiceRegion.Sydney;
					break;

				case "uscentral":
					output = VoiceRegion.USCentral;
					break;

				case "useast":
					output = VoiceRegion.USEast;
					break;

				case "ussouth":
					output = VoiceRegion.USSouth;
					break;

				case "uswest":
					output = VoiceRegion.USWest;
					break;

				default:
					output = VoiceRegion.Unknown;
					break;
			}

			return await Task.FromResult(output);
		}
	}
}
