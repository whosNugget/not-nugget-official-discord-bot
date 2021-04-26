using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NuggetOfficial.Data.Permissions
{
	/// <summary>
	/// The representation of a member or role's authority to create channels
	/// </summary>
	public enum ChannelCreationAuthority
	{
		/// <summary>
		/// The represented member or role is not authorized create channels
		/// </summary>
		Unauthorized,

		/// <summary>
		/// The represented member or role is authorized to create channels
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
		/// The represented member or role is authorized to create Hidden channels
		/// </summary>
		Hidden = 4,

		/// <summary>
		/// The represented member or role is authorized to create Limited channels
		/// </summary>
		Limited = 8,

		/// <summary>
		/// The represented member or role is authorized to create any channel
		/// </summary>
		Authorized = Public | Private | Hidden | Limited
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
		/// <summary>
		/// Create a default authorization scheme where the represented member or role is not authorized to do anything
		/// </summary>
		public VoiceChannelPermissions()
		{

		}

		/// <summary>
		/// Create a custom authorization scheme
		/// </summary>
		/// <param name="channelCreationAuthority">Represented member or role's authorization to create channels</param>
		/// <param name="channelCreationQuantityAuthority">Represented member or role's authorization to create multiple channels</param>
		/// <param name="channelAccesibilityConfigurationAuthority">Represented member or role's authorization to configure a channel's accesibility</param>
		/// <param name="channelRegionConfigurationAuthority">Represented member or role's authorization to configure a channel's voice server location</param>
		/// <param name="specificChannelCreationQuantity">Number of channels the represented member can create. Only applies to members or roles with the <c>ChannelCreationQuantityAuthority.Specified</c> authority</param>
		public VoiceChannelPermissions(ChannelCreationAuthority channelCreationAuthority, ChannelCreationQuantityAuthority channelCreationQuantityAuthority, ChannelAccesibilityConfigurationAuthority channelAccesibilityConfigurationAuthority, ChannelRegionConfigurationAuthority channelRegionConfigurationAuthority, int specificChannelCreationQuantity = 1)
		{

		}

		/// <summary>
		/// Validate that the provided parameters adhere to this permission scheme
		/// </summary>
		/// <param name="error">If a parameter does not adhere to this scheme, an error message will be generated</param>
		/// <returns><c>true</c> if the parameters adhere to this permission scheme, <c>false</c> if otherwise</returns>
		public bool ValidateChannelCreationParameters(ref string error)
		{
			error = string.Empty;
			return false;
		}
	}
}
