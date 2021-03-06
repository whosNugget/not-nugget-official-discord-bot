﻿using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace NuggetOfficial.Discord.Data.Converters
{
	public class PermitDenyStringConverter : IArgumentConverter<bool>
	{
		public async Task<Optional<bool>> ConvertAsync(string value, CommandContext ctx)
		{
			value ??= string.Empty; //Ensure value exists
			value = value.Trim(); //Trim value if needed
			return await Task.FromResult(value.Equals("permit") ? Optional.FromValue(true) : value.Equals("deny") ? Optional.FromValue(false) : Optional.FromNoValue<bool>()); //Return based on value
		}
	}
}
