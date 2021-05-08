using DSharpPlus.Entities;
using NuggetOfficial.Authority;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace NuggetOfficial.Data.VoiceModule
{
	//TODO abstractify this class so it can be more broad/dynamic maybe?
	/// <summary>
	/// This class holds references to DiscordGuilds and their subsequent data used by the bot's VC features. [NYI/NYT] This class can be directly serialized and deserialized
	/// </summary>
	public class VoiceRegisteredGuildData //TODO extract the important stuff into an Interface so it will be negligible to support other data storage methods
	{
		//should store channel, user, and permission data on a per-guild basis
		//should be serializable and saved before the bot is terminated
		//should be loaded from a file every time the bot starts if one exists
		[JsonProperty("registered_guilds")]
		readonly Dictionary<DiscordGuild, GuildData> registeredGuilds = new Dictionary<DiscordGuild, GuildData>();

		/// <summary>
		/// Get the GuildData for the provided guild. Will return null instead of throw an exeption when any error occurs
		/// </summary>
		/// <param name="guild">Guild to get data for</param>
		/// <returns>The valid stored guild data, if one exists. null if otherwise</returns>
		[JsonIgnore]
		public GuildData this[DiscordGuild guild]
		{
			get
			{
				if (!registeredGuilds.ContainsKey(guild)) return null;
				return registeredGuilds[guild];
			}
		}

		public VoiceRegisteredGuildData() { }
		[JsonConstructor]
		VoiceRegisteredGuildData(DiscordGuild toRegister, DiscordChannel parentCategory, DiscordChannel waitingRoomVc, DiscordChannel commandListenChannel, DiscordRole memberRole, DiscordRole mutedRole, DiscordRole botManagerRole)
		{
			RegisterGuild(toRegister, parentCategory, waitingRoomVc, commandListenChannel, memberRole, mutedRole, botManagerRole, out _);
		}

		/// <summary>
		/// Register a guild, allowing it to use the VC features
		/// </summary>
		/// <param name="toRegister">Guild to register</param>
		/// <param name="parentCategory">The parent category channels will be created under</param>
		/// <param name="waitingRoomVc">The VC members are required to be in to create other channels</param>
		/// <param name="commandListenChannel">The chat channel that the bot will respond to commands in</param>
		/// <param name="error">If an error occurs while registering the guild, this field will be populated</param>
		/// <returns>True when no error occurs during registration</returns>
		public bool RegisterGuild(DiscordGuild toRegister, DiscordChannel parentCategory, DiscordChannel waitingRoomVc, DiscordChannel commandListenChannel, DiscordRole memberRole, DiscordRole mutedRole, DiscordRole botManagerRole, out string error)
		{
			error = string.Empty;

			if (toRegister is null)
			{
				error = "Cannot register a null guild";
				goto Completed;
			}

			if (registeredGuilds.ContainsKey(toRegister))
			{
				error = "Guild is already registered";
				goto Completed;
			}

			registeredGuilds.Add(toRegister, new GuildData(parentCategory, waitingRoomVc, commandListenChannel, memberRole, mutedRole, botManagerRole));

		Completed:
			return error == string.Empty;
		}

		/// <summary>
		/// Initialize a guild's permissions
		/// </summary>
		/// <param name="everyonePermission">Default permission for every member on the server</param>
		/// <param name="rolewisePermissions">Permissions which apply to members based on roles</param>
		/// <param name="memberwisePermissions">Permissions which apply to members directly</param>
		/// <param name="error">If an error occurrs durring initialization, this field will be populated</param>
		/// <returns>True if no error occurs during initialization, false if otherwise</returns>
		public bool InitializeGuildPermissions(DiscordGuild guild, VoiceChannelCreationPermissions everyonePermission, IEnumerable<KeyValuePair<DiscordRole, VoiceChannelCreationPermissions>> rolewisePermissions, IEnumerable<KeyValuePair<DiscordMember, VoiceChannelCreationPermissions>> memberwisePermissions, out string error)
		{
			error = string.Empty;

			if (!registeredGuilds.ContainsKey(guild))
			{
				error = "Cannot initialize permissions on a guild which is not registered";
				goto Completed;
			}

			registeredGuilds[guild].InitializePermissions(everyonePermission, rolewisePermissions, memberwisePermissions);

		Completed:
			return error == string.Empty;
		}

		/// <summary>
		/// Update a guild's permissions
		/// </summary>
		/// <param name="guild">Guild who's permissions will be updated</param>
		/// <param name="everyonePermission">New update permission. Should be null if you don't want to update this permission</param>
		/// <param name="rolewisePermissions">New rolewose permissions. Will only update existing roles if they already exist, and will add new roles to the list if they do not</param>
		/// <param name="memberwisePermissions">New memberwise permissions. Will only update existing members if they already exist, and will add new members to the list if they do not</param>
		/// <param name="error">If an error occurrs durring the update, this field will be populated</param>
		/// <returns>True of no error occurs during the update, false if otherwise</returns>
		public bool UpdateGuildPermissions(DiscordGuild guild, VoiceChannelCreationPermissions everyonePermission, IEnumerable<KeyValuePair<DiscordRole, VoiceChannelCreationPermissions>> rolewisePermissions, IEnumerable<KeyValuePair<DiscordMember, VoiceChannelCreationPermissions>> memberwisePermissions, out string error)
		{
			error = string.Empty;

			if (!registeredGuilds.ContainsKey(guild))
			{
				error = "Cannot update permissions on a guild which is not registered";
				goto Completed;
			}

			registeredGuilds[guild].UpdatePermissions(everyonePermission, rolewisePermissions, memberwisePermissions);

		Completed:
			return error == string.Empty;
		}

		/// <summary>
		/// Attempts to remove a guild from the registered guild container
		/// </summary>
		/// <param name="toRemove">The guild to remove</param>
		/// <param name="error">If an error occurs, this field will be populated</param>
		/// <returns>True if the guild was successfully removed, false if otherwise</returns>
		public bool RemoveGuild(DiscordGuild toRemove, out string error)
		{
			error = string.Empty;

			if (toRemove is null)
			{
				error = "Cannot remove a null guild";
				goto Completed;
			}

			if (registeredGuilds.ContainsKey(toRemove))
			{
				registeredGuilds[toRemove] = null;
				registeredGuilds.Remove(toRemove);
				goto Completed;
			}

			error = "Cannot remove a guild which was not registered";

		Completed:
			return error == string.Empty;
		}

		/// <summary>
		/// Contains the per-guild data required for the VC system to function
		/// </summary>
		public class GuildData
		{
			/// <summary>
			/// The parent category all created channels will be sorted under
			/// </summary>
			[JsonProperty("parent_category", Required = Required.Always)]
			public DiscordChannel ParentCategory { get; private set; }
			/// <summary>
			/// The VC members must be waiting in in order to create channels
			/// </summary>
			[JsonProperty("waiting_room_vc")]
			public DiscordChannel WaitingRoomVC { get; private set; }
			/// <summary>
			/// The text channel the bot will respond to commands in
			/// </summary>
			[JsonProperty("command_listen_channel")]
			public DiscordChannel CommandListenChannel { get; private set; }
			/// <summary>
			/// A role that specifies a general member. The bot will not take action on messages sent if the sender doesn't have this role
			/// </summary>
			[JsonProperty("member_role")]
			public DiscordRole MemberRole { get; private set; }
			/// <summary>
			/// A role that specifies a muted member. The bot will not take actions on messages sent if the sender has this role, and will dissalow members with this role from joining any created channel
			/// </summary>
			[JsonProperty("muted_role")]
			public DiscordRole MutedRole { get; private set; }
			/// <summary>
			/// Members with this role are always permitted to use VC commands
			/// </summary>
			public DiscordRole BotManagerRole { get; private set; }
			/// <summary>
			/// The permission default that everyone without a role in the server adopts when creating voice channels
			/// </summary>
			[JsonProperty("everyone_permission")]
			public VoiceChannelCreationPermissions EveryonePermission { get; private set; }

			/// <summary>
			/// The created channel count per member in this server
			/// </summary>
			[JsonProperty("created_channels")]
			readonly Dictionary<DiscordMember, List<DiscordChannel>> createdChannels = new Dictionary<DiscordMember, List<DiscordChannel>>();

			/// <summary>
			/// 
			/// </summary>
			/// <param name="member"></param>
			/// <returns></returns>
			[JsonIgnore]
			public List<DiscordChannel> this[DiscordMember member]
			{
				get
				{
					if (!createdChannels.ContainsKey(member)) return null;
					return createdChannels[member];
				}
			}
			[JsonProperty("rolewise_permissions")]
			Dictionary<DiscordRole, VoiceChannelCreationPermissions> rolewisePermissions;
			[JsonProperty("memberwise_permissions")]
			Dictionary<DiscordMember, VoiceChannelCreationPermissions> memberwisePermissions;

			/// <summary>
			/// TODO
			/// </summary>
			/// <param name="parentCategory"></param>
			/// <param name="waitingRoomVc"></param>
			/// <param name="commandListenChannel"></param>
			/// <param name="memberRole"></param>
			/// <param name="mutedRole"></param>
			public GuildData(DiscordChannel parentCategory, DiscordChannel waitingRoomVc, DiscordChannel commandListenChannel, DiscordRole memberRole, DiscordRole mutedRole, DiscordRole botManagerRole)
			{
				ParentCategory = parentCategory;
				WaitingRoomVC = waitingRoomVc;
				CommandListenChannel = commandListenChannel;
				MemberRole = memberRole;
				MutedRole = mutedRole;
				BotManagerRole = botManagerRole;

				EveryonePermission = null;
				rolewisePermissions = new Dictionary<DiscordRole, VoiceChannelCreationPermissions>();
				memberwisePermissions = new Dictionary<DiscordMember, VoiceChannelCreationPermissions>();
			}
			/// <summary>
			/// TODO
			/// </summary>
			/// <param name="parentCategory"></param>
			/// <param name="waitingRoomVc"></param>
			/// <param name="commandListenChannel"></param>
			/// <param name="memberRole"></param>
			/// <param name="mutedRole"></param>
			/// <param name="everyonePermission"></param>
			/// <param name="rolewisePermissions"></param>
			/// <param name="memberwisePermissions"></param>
			public GuildData(DiscordChannel parentCategory, DiscordChannel waitingRoomVc, DiscordChannel commandListenChannel, DiscordRole memberRole, DiscordRole mutedRole, DiscordRole botManagerRole, VoiceChannelCreationPermissions everyonePermission, IEnumerable<KeyValuePair<DiscordRole, VoiceChannelCreationPermissions>> rolewisePermissions, IEnumerable<KeyValuePair<DiscordMember, VoiceChannelCreationPermissions>> memberwisePermissions) : this(parentCategory, waitingRoomVc, commandListenChannel, memberRole, mutedRole, botManagerRole)
			{
				InitializePermissions(everyonePermission, rolewisePermissions, memberwisePermissions);
			}

			/// <summary>
			/// TODO Documentation comment
			/// </summary>
			/// <param name="everyonePermission"></param>
			/// <param name="rolewisePermissions"></param>
			/// <param name="memberwisePermissions"></param>
			public void InitializePermissions(VoiceChannelCreationPermissions everyonePermission, IEnumerable<KeyValuePair<DiscordRole, VoiceChannelCreationPermissions>> rolewisePermissions, IEnumerable<KeyValuePair<DiscordMember, VoiceChannelCreationPermissions>> memberwisePermissions)
			{
				this.EveryonePermission = everyonePermission ?? new VoiceChannelCreationPermissions();
				this.rolewisePermissions = new Dictionary<DiscordRole, VoiceChannelCreationPermissions>(rolewisePermissions ?? new KeyValuePair<DiscordRole, VoiceChannelCreationPermissions>[] { });
				this.memberwisePermissions = new Dictionary<DiscordMember, VoiceChannelCreationPermissions>(memberwisePermissions ?? new KeyValuePair<DiscordMember, VoiceChannelCreationPermissions>[] { });
			}

			/// <summary>
			/// Updates all provided existing permissions, removes keys whos values are null, adds keys whos values are not null and don't exist in the current dictionary
			/// </summary>
			/// <param name="everyonePermission">Updated everyone permission. Set null to ignore</param>
			/// <param name="rolewisePermissions">Updated rolewise permissions. Set null to ignore, set value to null to remove coresponding key from dictionary, set key and value to update existing or add new value repsectively</param>
			/// <param name="memberwisePermissions">Updated memberwise permissions. Set null to ignore, set value to null to remove coresponding key from dictionary, set key and value to update existing or add new value repsectively</param>
			public void UpdatePermissions(VoiceChannelCreationPermissions everyonePermission, IEnumerable<KeyValuePair<DiscordRole, VoiceChannelCreationPermissions>> rolewisePermissions, IEnumerable<KeyValuePair<DiscordMember, VoiceChannelCreationPermissions>> memberwisePermissions)
			{
				if (!(everyonePermission is null))
				{
					EveryonePermission = everyonePermission;
				}

				if (!(rolewisePermissions is null) && rolewisePermissions.Count() > 0)
				{
					foreach (var kvp in rolewisePermissions)
					{
						if (kvp.Key is null) continue;

						if (this.rolewisePermissions.ContainsKey(kvp.Key))
						{
							if (kvp.Value is null)
							{
								this.rolewisePermissions.Remove(kvp.Key);
								continue;
							}

							this.rolewisePermissions[kvp.Key] = kvp.Value;
							continue;
						}

						if (kvp.Value is null) continue;

						this.rolewisePermissions.Add(kvp.Key, kvp.Value);
					}
				}

				if (!(memberwisePermissions is null) && memberwisePermissions.Count() > 0)
				{
					foreach (var kvp in memberwisePermissions)
					{
						if (kvp.Key is null) continue;

						if (this.memberwisePermissions.ContainsKey(kvp.Key))
						{
							if (kvp.Value is null)
							{
								this.memberwisePermissions.Remove(kvp.Key);
								continue;
							}

							this.memberwisePermissions[kvp.Key] = kvp.Value;
						}

						if (kvp.Value is null) continue;

						this.memberwisePermissions.Add(kvp.Key, kvp.Value);
					}
				}
			}

			/// <summary>
			/// TODO Documentation comment
			/// </summary>
			/// <param name="member"></param>
			/// <param name="requestedPublicity"></param>
			/// <param name="requestedRegion"></param>
			/// <param name="error"></param>
			/// <returns></returns>
			public bool CheckPermission(DiscordMember member, string name, ChannelPublicity requestedPublicity, VoiceRegion requestedRegion, out string error)
			{
				VoiceChannelCreationPermissions highestPrecedencePermissions = null;
				int existingChannels = (createdChannels.ContainsKey(member) ? createdChannels[member]?.Count : 0) ?? 0;

				int highestRolePosition = -1;
				DiscordRole highestRole = null;
				foreach (DiscordRole role in member.Roles)
				{
					if (highestRolePosition < role.Position)
					{
						highestRole = role;
						highestRolePosition = role.Position;
					}
				}

				if (rolewisePermissions.ContainsKey(highestRole))
				{
					highestPrecedencePermissions = rolewisePermissions[highestRole];
				}

				if (memberwisePermissions.ContainsKey(member))
				{
					highestPrecedencePermissions = memberwisePermissions[member];
				}

				return (highestPrecedencePermissions ?? EveryonePermission).ValidateChannelCreationAuthority(!((name is null) || name == string.Empty), requestedPublicity, existingChannels, requestedRegion, out error);
			}

			public List<DiscordChannel> GetChannels(DiscordMember member)
			{
				if (createdChannels.ContainsKey(member)) return createdChannels[member];
				return null;
			}

			public void AddChannel(DiscordMember member, DiscordChannel channel)
			{
				if (createdChannels.ContainsKey(member))
				{
					createdChannels[member].Add(channel);
					return;
				}

				createdChannels.Add(member, new List<DiscordChannel>(new DiscordChannel[] { channel }));
			}

			public bool RemoveChannel(DiscordMember member, DiscordChannel channel)
			{
				if (createdChannels.ContainsKey(member) && !(createdChannels[member] is null) && createdChannels[member].Count > 0)
				{
					return createdChannels[member].Remove(channel);
				}

				return false;
			}
		}
	}
}
