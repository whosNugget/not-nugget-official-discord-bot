using DSharpPlus;
using DSharpPlus.Entities;
using NuggetOfficial.Data;
using System.Collections.Generic;

namespace NuggetOfficial.Data.VoiceModule
{
    /// <summary>
    /// Structure containing all parameters used for creating or editing channels, as well as some commonly useful metadata. Create instances of this structure using the initialization syntax
    /// </summary>
    public struct ChannelCreationParameters
    {
        public DiscordUser CreatingUser { get; }
        public DiscordChannel InvocationChannel { get; }
        public int ExistingChannels { get; }
        public int MaximumAllowedChannels { get; }

        public string ChannelName { get; }
        public DiscordChannel ParentChannel { get; }
        public int? Bitrate { get; }
        public int? UserLimit { get; }
        public IEnumerable<DiscordOverwrite> PermissionOverwrites { get; }
        public ChannelPublicity Publicity { get; }
        public VoiceRegion? RequestedRegion { get; }
        public VideoQualityMode? VideoQuality { get; }
        public string AuditLogReason { get; }
    }
}
