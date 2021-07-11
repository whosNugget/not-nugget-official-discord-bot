using System;

namespace NuggetOfficial.Discord.Commands.VoiceHubModule.Data.Permissions
{
    /// <summary>
    /// The representation of a member or role's authorities they cannot violate when creating or editing channels
    /// </summary>
    [Flags]
    public enum ChannelAuthorities
    {
        /// <summary>
        /// The represented member or role has no authorities
        /// </summary>
        CompletelyUnauthorized = 0,

        /// <summary>
        /// The represented member or role is authorized to create channels
        /// </summary>
        CanCreateChannels = 1,

        /// <summary>
        /// The represented member or role is authorized to rename channels
        /// </summary>
        CanRenameChannels = 2,

        /// <summary>
        /// The represented member or role is authorized to create only one channel
        /// </summary>
        CanCreateSingleChannel = 4,

        /// <summary>
        /// The represented member or role is authorized to create a specified number of channels
        /// </summary>
        CanCreateMultipleChannels = 8,

        /// <summary>
        /// The represented member or role is authorized to create as many channels as they want
        /// </summary>
        CanCreateInfiniteChannels = 16,

        /// <summary>
        /// The represented member or role is authorized to create private channels
        /// </summary>
        CanCreatePrivateChannels = 32,

        /// <summary>
        /// The represented member or role is authorized to create public channels
        /// </summary>
        CanCreatePublicChannels = 64,

        /// <summary>
        /// The represented member or role is authorized to create hidden channels
        /// </summary>
        CanCreateHiddenChannels = 128,

        /// <summary>
        /// The represented member or role is authorized to create supporter channels
        /// </summary>
        CanCreateSupporterChannels = 256,

        /// <summary>
        /// The represented member or role is authorized to create channels with user limits
        /// </summary>
        CanCreateLimitedChannels = 512,

        /// <summary>
        /// The represented member or role is authorized to create channels with any restrictions
        /// </summary>
        CanCreateAnyChannel = CanCreateHiddenChannels | CanCreatePrivateChannels | CanCreatePublicChannels | CanCreateSupporterChannels,

        /// <summary>
        /// The represented member or role is authorized to modify the channel's voice region
        /// </summary>
        CanModifyChannelRegion = 1024,

        /// <summary>
        /// The represented member or role is authorized to modify the channel's bitrate
        /// </summary>
        CanModifyChannelBitrate = 2042,

        /// <summary>
        /// The represented member or role has all authorities
        /// </summary>
        CompletelyAuthorized = CanCreateChannels | CanRenameChannels | CanCreateInfiniteChannels | CanCreateAnyChannel | CanModifyChannelBitrate | CanModifyChannelRegion,
    }
}
