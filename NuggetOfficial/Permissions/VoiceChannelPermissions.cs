using NuggetOfficial.Data;

namespace NuggetOfficial.Authority
{
	//TODO move these to their own files?
	/// <summary>
	/// The representation of a member or role's authority to create channels
	/// </summary>
	public enum ChannelCreationAuthority
	{
		/// <summary>
		/// The represented member or role is not authorized to create channels
		/// </summary>
		Unauthorized,

		/// <summary>
		/// The represented member or role is authorized to create channels
		/// </summary>
		Authorized
	}

	/// <summary>
	/// The representation of a member or role's authority to reaname created channels
	/// </summary>
	public enum ChannelRenameAuthority
	{
		/// <summary>
		/// The represented member or role is not authorized to rename created channels
		/// </summary>
		Unauthorized,

		/// <summary>
		/// The represented member or role is not authorized to rename created channels
		/// </summary>
		Authorized
	}

	/// <summary>
	/// The representation of a member or role's authority to create multiple channels
	/// </summary>
	public enum ChannelCreationQuantityAuthority
	{
		/// <summary>
		/// The represented member or role is not authorized to create any voice channels
		/// </summary>
		None,

		/// <summary>
		/// The represented member or role is authorized to create a single voice channel
		/// </summary>
		Single,

		/// <summary>
		/// The represented member or role is authorized to create a specific number of voice channels
		/// </summary>
		Specified,

		/// <summary>
		/// The represented member or role is authorized to create as many voice channels as they like
		/// </summary>
		Unlimited
	}

	/// <summary>
	/// The representation of a member or role's authority to specify a channel's accesibility
	/// </summary>
	public enum ChannelAccesibilityConfigurationAuthority
	{
		/// <summary>
		/// The represented member or role is not authorized to create a channel
		/// </summary>
		Unauthorized = 0,

		/// <summary>
		/// The represented member or role is authorized to create Public channels
		/// </summary>
		Public = 1,

		/// <summary>
		/// The represented member or role is authorized to create Private channels
		/// </summary>
		Private = 2,

		/// <summary>
		/// The represented member or role is authorized to create Supporter-Only channels
		/// </summary>
		Supporter = 4,

		/// <summary>
		/// The represented member or role is authorized to create Hidden channels
		/// </summary>
		Hidden = 8,

		/// <summary>
		/// The represented member or role is authorized to create Limited channels
		/// </summary>
		Limited = 16,

		/// <summary>
		/// The represented member or role is authorized to create any channel
		/// </summary>
		Authorized = Public | Private | Supporter | Hidden | Limited
	}

	/// <summary>
	/// The representation of a member or role's authority to modify a channel's RTC region
	/// </summary>
	public enum ChannelRegionConfigurationAuthority
	{
		/// <summary>
		/// The represented member or role is not authorized to modify a channel's RTC region
		/// </summary>
		Unauthorized,

		/// <summary>
		/// The represented member or role is authorized to modify a channel's RTC region
		/// </summary>
		Authorized
	}

	public class VoiceChannelPermissions
	{
		public static VoiceChannelPermissions Authorized { get => new VoiceChannelPermissions(ChannelCreationAuthority.Authorized, ChannelRenameAuthority.Authorized, ChannelCreationQuantityAuthority.Unlimited, ChannelAccesibilityConfigurationAuthority.Authorized, ChannelRegionConfigurationAuthority.Authorized); }
		public static VoiceChannelPermissions Unauthorized { get => new VoiceChannelPermissions(ChannelCreationAuthority.Unauthorized, ChannelRenameAuthority.Unauthorized, ChannelCreationQuantityAuthority.None, ChannelAccesibilityConfigurationAuthority.Unauthorized, ChannelRegionConfigurationAuthority.Unauthorized); }

		readonly ChannelCreationAuthority channelCreationAuthority;
		readonly ChannelRenameAuthority channelRenameAuthority;
		readonly ChannelCreationQuantityAuthority channelCreationQuantityAuthority;
		readonly ChannelAccesibilityConfigurationAuthority channelAccesibilityConfigurationAuthority;
		readonly ChannelRegionConfigurationAuthority channelRegionConfigurationAuthority;
		readonly int specificChannelCreationQuantity;

		/// <summary>
		/// Create a default authorization scheme where the represented member or role is completely unauthorized
		/// </summary>
		public VoiceChannelPermissions()
		{
			channelCreationAuthority = ChannelCreationAuthority.Unauthorized;
			channelRenameAuthority = ChannelRenameAuthority.Unauthorized;
			channelCreationQuantityAuthority = ChannelCreationQuantityAuthority.None;
			channelAccesibilityConfigurationAuthority = ChannelAccesibilityConfigurationAuthority.Unauthorized;
			channelRegionConfigurationAuthority = ChannelRegionConfigurationAuthority.Unauthorized;
		}
		/// <summary>
		/// Create a custom authorization scheme
		/// </summary>
		/// <param name="channelCreationAuthority">Represented member or role's authorization to create channels</param>
		/// <param name="channelCreationQuantityAuthority">Represented member or role's authorization to create multiple channels</param>
		/// <param name="channelAccesibilityConfigurationAuthority">Represented member or role's authorization to configure a channel's accesibility</param>
		/// <param name="channelRegionConfigurationAuthority">Represented member or role's authorization to configure a channel's voice server location</param>
		/// <param name="specificChannelCreationQuantity">Number of channels the represented member can create. Only applies to members or roles with the <c>ChannelCreationQuantityAuthority.Specified</c> authority</param>
		public VoiceChannelPermissions(ChannelCreationAuthority channelCreationAuthority, ChannelRenameAuthority channelRenameAuthority, ChannelCreationQuantityAuthority channelCreationQuantityAuthority, ChannelAccesibilityConfigurationAuthority channelAccesibilityConfigurationAuthority, ChannelRegionConfigurationAuthority channelRegionConfigurationAuthority, int specificChannelCreationQuantity = 1)
		{
			this.channelCreationAuthority = channelCreationAuthority;
			this.channelRenameAuthority = channelRenameAuthority;
			this.channelCreationQuantityAuthority = channelCreationQuantityAuthority;
			this.channelAccesibilityConfigurationAuthority = channelAccesibilityConfigurationAuthority;
			this.channelRegionConfigurationAuthority = channelRegionConfigurationAuthority;
			this.specificChannelCreationQuantity = specificChannelCreationQuantity;
		}

		/// <summary>
		/// Validate that the provided parameters adhere to this permission scheme
		/// </summary>
		/// <param name="requestedPublicity">The requested publicity of the channel to be created</param>
		/// M<param name="existingChannels">The number of existing channels the represented member has, if any</param>
		/// <param name="requestedRegion">The requested voice region to use for the channel to be created</param>
		/// <param name="error">If a parameter does not adhere to this scheme, an error message will be generated</param>
		/// <returns><c>true</c> if the parameters adhere to this permission scheme, <c>false</c> if otherwise</returns>
		public bool ValidateChannelCreationParameterAuthority(ChannelPublicity requestedPublicity, int existingChannels, VoiceRegion requestedRegion, out string error)
		{
			error = string.Empty;

			if (channelCreationAuthority == ChannelCreationAuthority.Unauthorized)
			{
				error = $"Member does not have the authority to create voice channels";
				goto RequestedParametersInvalid;
			}

			if (!(channelCreationQuantityAuthority == ChannelCreationQuantityAuthority.Unlimited) && existingChannels > 0)
			{
				if (channelCreationQuantityAuthority == ChannelCreationQuantityAuthority.Single)
				{
					error = "Member does not have the authority to create multiple voice channels";
					goto RequestedParametersInvalid;
				}

				if (channelCreationQuantityAuthority == ChannelCreationQuantityAuthority.Specified && ++existingChannels > specificChannelCreationQuantity)
				{
					error = "Member has already created the maximum number of allowed specific channels";
					goto RequestedParametersInvalid;
				}
			}

			if (channelRegionConfigurationAuthority != ChannelRegionConfigurationAuthority.Authorized && requestedRegion != VoiceRegion.Automatic)
			{
				error = "Member does not have the authority to change the voice channel's region";
				goto RequestedParametersInvalid;
			}

			switch (requestedPublicity)
			{
				case ChannelPublicity.Public:
					if (!channelAccesibilityConfigurationAuthority.HasFlag(ChannelAccesibilityConfigurationAuthority.Public))
					{
						error = "Member does not have the authority to create public voice channels";
						goto RequestedParametersInvalid;
					}
					break;

				case ChannelPublicity.Private:
					if (!channelAccesibilityConfigurationAuthority.HasFlag(ChannelAccesibilityConfigurationAuthority.Private))
					{
						error = "Member does not have the authority to create private voice channels";
						goto RequestedParametersInvalid;
					}
					break;

				case ChannelPublicity.Supporter:
					if (!channelAccesibilityConfigurationAuthority.HasFlag(ChannelAccesibilityConfigurationAuthority.Supporter))
					{
						error = "Member does not have the authority to create supporter-only voice channels";
						goto RequestedParametersInvalid;
					}
					break;

				case ChannelPublicity.Hidden:
					if (!channelAccesibilityConfigurationAuthority.HasFlag(ChannelAccesibilityConfigurationAuthority.Hidden))
					{
						error = "Member does not have the authority to create hidden voice channels";
						goto RequestedParametersInvalid;
					}
					break;
			}

		RequestedParametersInvalid:
			return error == string.Empty;
		}
	}
}
