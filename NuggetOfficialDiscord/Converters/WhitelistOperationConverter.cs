using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using NuggetOfficialDiscord.Commands.VoiceHubModule.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NuggetOfficialDiscord.Converters
{
    class WhitelistOperationConverter : IArgumentConverter<WhitelistOperation>
    {
        public async Task<Optional<WhitelistOperation>> ConvertAsync(string value, CommandContext ctx)
        {
            WhitelistOperation operation = WhitelistOperation.Unknown;


            return await Task.FromResult(operation);
        }
    }
}
