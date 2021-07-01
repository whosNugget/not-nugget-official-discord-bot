using Newtonsoft.Json;
using System.Collections.Generic;

namespace NuggetOfficial.Discord.Commands.VoiceHubModule.Data.Permissions
{
	///// <summary>
	///// The representation of a member or role's authority to create channels
	///// </summary>
	//public enum ChannelCreationAuthority : byte
	//{
	//    /// <summary>
	//    /// The represented member or role is not authorized to create channels
	//    /// </summary>
	//    Unauthorized,

	//    /// <summary>
	//    /// The represented member or role is authorized to create channels
	//    /// </summary>
	//    Authorized
	//}

	///// <summary>
	///// The representation of a member or role's authority to reaname created channels
	///// </summary>
	//public enum ChannelRenameAuthority : byte
	//{
	//    /// <summary>
	//    /// The represented member or role is not authorized to rename created channels
	//    /// </summary>
	//    Unauthorized,

	//    /// <summary>
	//    /// The represented member or role is not authorized to rename created channels
	//    /// </summary>
	//    Authorized
	//}

	///// <summary>
	///// The representation of a member or role's authority to create multiple channels
	///// </summary>
	//public enum ChannelCreationQuantityAuthority : byte
	//{
	//    /// <summary>
	//    /// The represented member or role is not authorized to create any voice channels
	//    /// </summary>
	//    None,

	//    /// <summary>
	//    /// The represented member or role is authorized to create a single voice channel
	//    /// </summary>
	//    Single,

	//    /// <summary>
	//    /// The represented member or role is authorized to create a specific number of voice channels
	//    /// </summary>
	//    Specified,

	//    /// <summary>
	//    /// The represented member or role is authorized to create as many voice channels as they like
	//    /// </summary>
	//    Unlimited
	//}

	///// <summary>
	///// The representation of a member or role's authority to specify a channel's accesibility
	///// </summary>
	//public enum ChannelAccesibilityConfigurationAuthority : byte
	//{
	//    /// <summary>
	//    /// The represented member or role is not authorized to create a channel
	//    /// </summary>
	//    Unauthorized = 0,

	//    /// <summary>
	//    /// The represented member or role is authorized to create Public channels
	//    /// </summary>
	//    Public = 1,

	//    /// <summary>
	//    /// The represented member or role is authorized to create Private channels
	//    /// </summary>
	//    Private = 2,

	//    /// <summary>
	//    /// The represented member or role is authorized to create Supporter-Only channels
	//    /// </summary>
	//    Supporter = 4,

	//    /// <summary>
	//    /// The represented member or role is authorized to create Hidden channels
	//    /// </summary>
	//    Hidden = 8,

	//    /// <summary>
	//    /// The represented member or role is authorized to create Limited channels
	//    /// </summary>
	//    Limited = 16,

	//    /// <summary>
	//    /// The represented member or role is authorized to create any channel
	//    /// </summary>
	//    Authorized = Public | Private | Supporter | Hidden | Limited
	//}

	///// <summary>
	///// The representation of a member or role's authority to modify a channel's RTC region
	///// </summary>
	//public enum ChannelRegionConfigurationAuthority : byte
	//{
	//    /// <summary>
	//    /// The represented member or role is not authorized to modify a channel's RTC region
	//    /// </summary>
	//    Unauthorized,

	//    /// <summary>
	//    /// The represented member or role is authorized to modify a channel's RTC region
	//    /// </summary>
	//    Authorized
	//}

	///// <summary>
	///// The representation of a member or role's authority to modify a channel's bitrate
	///// </summary>
	//public enum ChannelBitrateConfigurationAuthority : byte
	//{
	//    /// <summary>
	//    /// The represented member or role is not authorized to modify a channel's bitrate
	//    /// </summary>
	//    Unaurhotized,

	//    /// <summary>
	//    /// The represented member or role is not authorized to modify a channel's bitrate
	//    /// </summary>
	//    Authorized
	//}

	//[JsonObject]
	//public class VoiceChannelConfigurationPermissions
	//{
	//	public static VoiceChannelConfigurationPermissions Authorized { get => new VoiceChannelConfigurationPermissions(ChannelCreationAuthority.Authorized, ChannelRenameAuthority.Authorized, ChannelCreationQuantityAuthority.Unlimited, ChannelAccesibilityConfigurationAuthority.Authorized, ChannelRegionConfigurationAuthority.Authorized, ChannelBitrateConfigurationAuthority.Unaurhotized); }
	//	public static VoiceChannelConfigurationPermissions Unauthorized { get => new VoiceChannelConfigurationPermissions(); }

	//	[JsonProperty("channel_creation_authority")]
	//	public ChannelCreationAuthority ChannelCreationAuthority { get; private set; }
	//	[JsonProperty("channel_rename_authority")]
	//	public ChannelRenameAuthority ChannelRenameAuthority {get; private set; }
	//	[JsonProperty("channel_creation_quantity_authority")]
	//	public ChannelCreationQuantityAuthority ChannelCreationQuantityAuthority {get; private set; }
	//	[JsonProperty("channel_accesibility_configuration_authority")]
	//	public ChannelAccesibilityConfigurationAuthority ChannelAccesibilityConfigurationAuthority {get; private set; }
	//	[JsonProperty("channel_region_configuration_authority")]
	//	public ChannelRegionConfigurationAuthority ChannelRegionConfigurationAuthority {get; private set; }
	//	[JsonProperty("channel_bitrate_configuration_authority")]
	//	public ChannelBitrateConfigurationAuthority ChannelBitrateConfigurationAuthority { get; private set; }
	//	[JsonProperty("channel_creation_quantity")]
	//	public int SpecificChannelCreationQuantity { get; private set; }

	//	/// <summary>
	//	/// Create a default authorization scheme where the represented member or role is completely unauthorized
	//	/// </summary>
	//	public VoiceChannelConfigurationPermissions()
	//	{
	//		ChannelCreationAuthority = ChannelCreationAuthority.Unauthorized;
	//		ChannelRenameAuthority = ChannelRenameAuthority.Unauthorized;
	//		ChannelCreationQuantityAuthority = ChannelCreationQuantityAuthority.None;
	//		ChannelAccesibilityConfigurationAuthority = ChannelAccesibilityConfigurationAuthority.Unauthorized;
	//		ChannelRegionConfigurationAuthority = ChannelRegionConfigurationAuthority.Unauthorized;
	//	}
	//	/// <summary>
	//	/// Create a custom authorization scheme
	//	/// </summary>
	//	/// <param name="channelCreationAuthority">Represented member or role's authorization to create channels</param>
	//	/// <param name="channelCreationQuantityAuthority">Represented member or role's authorization to create multiple channels</param>
	//	/// <param name="channelAccesibilityConfigurationAuthority">Represented member or role's authorization to configure a channel's accesibility</param>
	//	/// <param name="channelRegionConfigurationAuthority">Represented member or role's authorization to configure a channel's voice server location</param>
	//	/// <param name="specificChannelCreationQuantity">Number of channels the represented member can create. Only applies to members or roles with the <c>ChannelCreationQuantityAuthority.Specified</c> authority</param>
	//	public VoiceChannelConfigurationPermissions(ChannelCreationAuthority channelCreationAuthority, ChannelRenameAuthority channelRenameAuthority, ChannelCreationQuantityAuthority channelCreationQuantityAuthority, ChannelAccesibilityConfigurationAuthority channelAccesibilityConfigurationAuthority, ChannelRegionConfigurationAuthority channelRegionConfigurationAuthority, ChannelBitrateConfigurationAuthority channelBitrateConfigurationAuthority, int specificChannelCreationQuantity = 1)
	//	{
	//		ChannelCreationAuthority = channelCreationAuthority;
	//		ChannelRenameAuthority = channelRenameAuthority;
	//		ChannelCreationQuantityAuthority = channelCreationQuantityAuthority;
	//		ChannelAccesibilityConfigurationAuthority = channelAccesibilityConfigurationAuthority;
	//		ChannelRegionConfigurationAuthority = channelRegionConfigurationAuthority;
	//		ChannelBitrateConfigurationAuthority = channelBitrateConfigurationAuthority;
	//		SpecificChannelCreationQuantity = specificChannelCreationQuantity;
	//	}

	//	/// <summary>
	//	/// Validate that the provided parameters adhere to this permission scheme
	//	/// </summary>
	//	/// <param name="channelRenamed">Was the channel renamed</param>
	//	/// <param name="requestedPublicity">The requested publicity of the channel to be created</param>
	//	/// M<param name="existingChannels">The number of existing channels the represented member has, if any</param>
	//	/// <param name="requestedRegion">The requested voice region to use for the channel to be created</param>
	//	/// <param name="bitrateChanged">Was the bitrate of the channel changed</param>
	//	/// <param name="error">If a parameter does not adhere to this scheme, an error message will be generated</param>
	//	/// <returns><c>true</c> if the parameters adhere to this permission scheme, <c>false</c> if otherwise</returns>
	//	public bool ValidateChannelCreationAuthority(bool channelRenamed, ChannelPublicity requestedPublicity, int existingChannels, VoiceRegion requestedRegion, bool bitrateChanged, out string error)
	//	{
	//		error = string.Empty;

	//		if (channelRenamed && ChannelRenameAuthority != ChannelRenameAuthority.Authorized)
	//		{
	//			error = $"Member does not have the authority to rename voice channels";
	//			goto RequestedParametersInvalid;
	//		}

	//		if (ChannelCreationAuthority == ChannelCreationAuthority.Unauthorized)
	//		{
	//			error = $"Member does not have the authority to create voice channels";
	//			goto RequestedParametersInvalid;
	//		}

	//		if (!(ChannelCreationQuantityAuthority == ChannelCreationQuantityAuthority.Unlimited) && existingChannels > 0)
	//		{
	//			if (ChannelCreationQuantityAuthority == ChannelCreationQuantityAuthority.Single)
	//			{
	//				error = "Member does not have the authority to create multiple voice channels";
	//				goto RequestedParametersInvalid;
	//			}

	//			if (ChannelCreationQuantityAuthority == ChannelCreationQuantityAuthority.Specified && ++existingChannels > SpecificChannelCreationQuantity)
	//			{
	//				error = "Member has already created the maximum number of allowed specific channels";
	//				goto RequestedParametersInvalid;
	//			}
	//		}

	//		if (ChannelRegionConfigurationAuthority != ChannelRegionConfigurationAuthority.Authorized && requestedRegion != VoiceRegion.Automatic)
	//		{
	//			error = "Member does not have the authority to change the voice channel's region";
	//			goto RequestedParametersInvalid;
	//		}

	//		if (ChannelBitrateConfigurationAuthority != ChannelBitrateConfigurationAuthority.Authorized && bitrateChanged)
	//		{
	//			error = "Member does not have the authority to change the voice channel's bitrate";
	//			goto RequestedParametersInvalid;
	//		}

	//		switch (requestedPublicity)
	//		{
	//			case ChannelPublicity.Public:
	//				if (!ChannelAccesibilityConfigurationAuthority.HasFlag(ChannelAccesibilityConfigurationAuthority.Public))
	//				{
	//					error = "Member does not have the authority to create public voice channels";
	//					goto RequestedParametersInvalid;
	//				}
	//				break;

	//			case ChannelPublicity.Private:
	//				if (!ChannelAccesibilityConfigurationAuthority.HasFlag(ChannelAccesibilityConfigurationAuthority.Private))
	//				{
	//					error = "Member does not have the authority to create private voice channels";
	//					goto RequestedParametersInvalid;
	//				}
	//				break;

	//			case ChannelPublicity.Supporter:
	//				if (!ChannelAccesibilityConfigurationAuthority.HasFlag(ChannelAccesibilityConfigurationAuthority.Supporter))
	//				{
	//					error = "Member does not have the authority to create supporter-only voice channels";
	//					goto RequestedParametersInvalid;
	//				}
	//				break;

	//			case ChannelPublicity.Hidden:
	//				if (!ChannelAccesibilityConfigurationAuthority.HasFlag(ChannelAccesibilityConfigurationAuthority.Hidden))
	//				{
	//					error = "Member does not have the authority to create hidden voice channels";
	//					goto RequestedParametersInvalid;
	//				}
	//				break;
	//		}

	//	RequestedParametersInvalid:
	//		return error == string.Empty;
	//	}
	//}

	/// <summary>
	/// Representation of authorities which allow or deny certain configurations when creating or editing channels
	/// </summary>
	[JsonObject]
    public class ChannelAuthorizations
    {
        public static ChannelAuthorities Authorized => ChannelAuthorities.CompletelyAuthorized;
        public static ChannelAuthorities Unauthorized => ChannelAuthorities.CompletelyUnauthorized;

        [JsonProperty("authorities")]
        public ChannelAuthorities Authorities { get; private set; }

        public ChannelAuthorizations()
        {
            Authorities = ChannelAuthorities.CompletelyUnauthorized;
        }
        public ChannelAuthorizations(ChannelAuthorities authorities)
        {
            Authorities = authorities;
        }
        public ChannelAuthorizations(int authoritiesInt)
        {
            Authorities = (ChannelAuthorities)authoritiesInt;
        }

        public bool VerifyAuthority(ChannelCreationParameters parameters, out string[] errors)
        {
            List<string> errorsList = new List<string>();

            if (!Authorities.HasFlag(ChannelAuthorities.CanCreateChannels))
            {
                errorsList.Add("member does not have the authority to create channels");
                goto MajorAuthorityViolationOccurred;
            }

            if (!string.IsNullOrEmpty(parameters.ChannelName) && !Authorities.HasFlag(ChannelAuthorities.CanRenameChannels))
            {
                errorsList.Add("member does not have the authority to rename channels");
            }

            if (!Authorities.HasFlag(ChannelAuthorities.CanCreateInfiniteChannels))
            {
                if (Authorities.HasFlag(ChannelAuthorities.CanCreateSingleChannel) && parameters.ExistingChannels != 0)
                {
                    errorsList.Add("member does not have the authority to create more than one channel");
                    goto ChannelQuantityCheckComplete;
                }

                if (Authorities.HasFlag(ChannelAuthorities.CanCreateMultipleChannels) && (parameters.ExistingChannels + 1) > parameters.MaximumAllowedChannels)
                {
                    errorsList.Add("member has reached their maximum channel limit and needs to delete some existing ones before they can create new ones");
                }
            }
        ChannelQuantityCheckComplete:

            if (!Authorities.HasFlag(ChannelAuthorities.CanModifyChannelRegion) && !(parameters.RequestedRegion is null))
            {
                errorsList.Add("member does not have the authority to modify the channel's voice region");
            }

            if (!Authorities.HasFlag(ChannelAuthorities.CanModifyChannelBitrate) && !(parameters.Bitrate is null))
            {
                errorsList.Add("member does not have the authority to modify the channel's bitrate");
                goto BitrateCheckComplete;
            }

        BitrateCheckComplete:

            switch (parameters.Publicity)
            {
                case ChannelPublicity.Public:
                    if (!Authorities.HasFlag(ChannelAuthorities.CanCreatePublicChannels))
                    {
                        errorsList.Add("member does not have the authority to create public channels");
                    }
                    break;
                case ChannelPublicity.Private:
                    if (!Authorities.HasFlag(ChannelAuthorities.CanCreatePrivateChannels))
                    {
                        errorsList.Add("member does not have the authority to create private channels");
                    }
                    break;
                case ChannelPublicity.Supporter:
                    if (!Authorities.HasFlag(ChannelAuthorities.CanCreateSupporterChannels))
                    {
                        errorsList.Add("member does not have the authority to create supporter channels");
                    }
                    break;
                case ChannelPublicity.Hidden:
                    if (!Authorities.HasFlag(ChannelAuthorities.CanCreateHiddenChannels))
                    {
                        errorsList.Add("member does not have the authority to create hidden channels");
                    }
                    break;
            }

        MajorAuthorityViolationOccurred:
            return (errors = errorsList.ToArray()).Length == 0;
        }
    }
}
