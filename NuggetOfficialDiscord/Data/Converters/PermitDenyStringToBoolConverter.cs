using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NuggetDiscordBot.Data.Converters
{
	public class PermitDenyStringToBoolConverter : IArgumentConverter<bool>
	{
		public Task<Optional<bool>> ConvertAsync(string value, CommandContext ctx)
		{
			value ??= string.Empty; //Ensure value exists
			value = value.Trim(); //Trim value if needed
			return Task.FromResult(value.Equals("permit") ? Optional.FromValue(true) : value.Equals("deny") ? Optional.FromValue(false) : Optional.FromNoValue<bool>()); //Return based on value
		}
	}
}
