using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NuggetDiscordBot.Data.Converters
{
	public class ServerRegionConverter : IArgumentConverter<VoiceRegion>
	{
		public Task<Optional<VoiceRegion>> ConvertAsync(string value, CommandContext ctx)
		{
			Optional<VoiceRegion> output;

			switch (value.ToLowerInvariant())
			{
				case "":
				case "auto":
				case "automatic":
					output = Optional.FromValue(VoiceRegion.Automatic);
					break;

				case "brazil":
					output = Optional.FromValue(VoiceRegion.Brazil);
					break;

				case "europe":
					output = Optional.FromValue(VoiceRegion.Europe);
					break;

				case "hongkong":
					output = Optional.FromValue(VoiceRegion.HongKong);
					break;

				case "india":
					output = Optional.FromValue(VoiceRegion.India);
					break;

				case "japan":
					output = Optional.FromValue(VoiceRegion.Japan);
					break;

				case "russia":
					output = Optional.FromValue(VoiceRegion.Russia);
					break;

				case "singapore":
					output = Optional.FromValue(VoiceRegion.Singapore);
					break;

				case "southafrica":
					output = Optional.FromValue(VoiceRegion.SouthAfrica);
					break;

				case "sydney":
					output = Optional.FromValue(VoiceRegion.Sydney);
					break;

				case "uscentral":
					output = Optional.FromValue(VoiceRegion.USCentral);
					break;

				case "useast":
					output = Optional.FromValue(VoiceRegion.USEast);
					break;

				case "ussouth":
					output = Optional.FromValue(VoiceRegion.USSouth);
					break;

				case "uswest":
					output = Optional.FromValue(VoiceRegion.USWest);
					break;

				default:
					output = Optional.FromValue(VoiceRegion.Unknown);
					break;
			}

			return Task.FromResult(output);
		}
	}
}
