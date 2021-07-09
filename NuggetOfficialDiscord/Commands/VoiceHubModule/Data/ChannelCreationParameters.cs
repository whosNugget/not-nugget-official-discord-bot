using DSharpPlus;
using DSharpPlus.Entities;
using System.Collections.Generic;

namespace NuggetOfficial.Discord.Commands.VoiceHubModule.Data
{
	/// <summary>
	/// Structure containing all parameters used for creating or editing channels, as well as some commonly useful metadata. Create instances of this structure using the initialization syntax
	/// </summary>
	public struct ChannelCreationParameters
	{
		public DiscordUser CreatingUser { get; set; }
		public DiscordChannel InvocationChannel { get; set; }
		public int ExistingChannels { get; set; }
		public int MaximumAllowedChannels { get; set; }

		public string ChannelName { get; set; }
		public DiscordChannel ParentChannel { get; set; }
		public int? Bitrate { get; set; }
		public int? UserLimit { get; set; }
		public IEnumerable<DiscordOverwrite> PermissionOverwrites { get; set; }
		public ChannelAccessibility Publicity { get; set; }
		public VoiceRegion? RequestedRegion { get; set; }
		public VideoQualityMode? VideoQuality { get; set; }
		public string AuditLogReason { get; set; }
	}
}
