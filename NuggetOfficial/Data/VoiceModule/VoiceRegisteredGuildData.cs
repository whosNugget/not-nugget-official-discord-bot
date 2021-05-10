using DSharpPlus;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using NuggetOfficial.Actions.Serialization;
using NuggetOfficial.Authority;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NuggetOfficial.Data.VoiceModule
{
	//TODO abstractify this class so it can be more broad/dynamic maybe?
	//should store channel, user, and permission data on a per-guild basis
	//should be serializable and saved before the bot is terminated
	//should be loaded from a file every time the bot starts if one exists
	/// <summary>
	/// This class holds references to DiscordGuilds and their subsequent data used by the bot's VC features. [NYI/NYT] This class can be directly serialized and deserialized
	/// </summary>
	public class VoiceRegisteredGuildData //TODO extract the important stuff into an Interface so it will be negligible to support other data storage methods
	{
		public Dictionary<DiscordGuild, GuildData> RegisteredGuilds { get; private set; } = new Dictionary<DiscordGuild, GuildData>();

		/// <summary>
		/// Get the GuildData for the provided guild. Will return null instead of throw an exeption when any error occurs
		/// </summary>
		/// <param name="guild">Guild to get data for</param>
		/// <returns>The valid stored guild data, if one exists. null if otherwise</returns>
		public GuildData this[DiscordGuild guild]
		{
			get
			{
				if (!RegisteredGuilds.ContainsKey(guild)) return null;
				return RegisteredGuilds[guild];
			}
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

			if (RegisteredGuilds.ContainsKey(toRegister))
			{
				error = "Guild is already registered";
				goto Completed;
			}

			RegisteredGuilds.Add(toRegister, new GuildData(parentCategory, waitingRoomVc, commandListenChannel, memberRole, mutedRole, botManagerRole));

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

			if (!RegisteredGuilds.ContainsKey(guild))
			{
				error = "Cannot initialize permissions on a guild which is not registered";
				goto Completed;
			}

			RegisteredGuilds[guild].InitializePermissions(everyonePermission, rolewisePermissions, memberwisePermissions);

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

			if (!RegisteredGuilds.ContainsKey(guild))
			{
				error = "Cannot update permissions on a guild which is not registered";
				goto Completed;
			}

			RegisteredGuilds[guild].UpdatePermissions(everyonePermission, rolewisePermissions, memberwisePermissions);

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

			if (RegisteredGuilds.ContainsKey(toRemove))
			{
				RegisteredGuilds[toRemove] = null;
				RegisteredGuilds.Remove(toRemove);
				goto Completed;
			}

			error = "Cannot remove a guild which was not registered";

		Completed:
			return error == string.Empty;
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <returns></returns>
		public Dictionary<SnowflakeContainer<DiscordGuild>, GuildDataSerializer> GetSerializerObject()
		{
			Dictionary<SnowflakeContainer<DiscordGuild>, GuildDataSerializer> serializerDictionary = new Dictionary<SnowflakeContainer<DiscordGuild>, GuildDataSerializer>();
			foreach (var kvp in RegisteredGuilds)
			{
				serializerDictionary.Add(new SnowflakeContainer<DiscordGuild>(kvp.Key), new GuildDataSerializer(kvp.Value));	
			}
			return serializerDictionary;
		}
	}

	//TODO own file
	/// <summary>
	/// Contains the per-guild data required for the VC system to function
	/// </summary>
	public class GuildData
	{
		/// <summary>
		/// The parent category all created channels will be sorted under
		/// </summary>
		public DiscordChannel ParentCategory { get; private set; }
		/// <summary>
		/// The VC members must be waiting in in order to create channels
		/// </summary>
		public DiscordChannel WaitingRoomVC { get; private set; }
		/// <summary>
		/// The text channel the bot will respond to commands in
		/// </summary>
		public DiscordChannel CommandListenChannel { get; private set; }
		/// <summary>
		/// A role that specifies a general member. The bot will not take action on messages sent if the sender doesn't have this role
		/// </summary>
		public DiscordRole MemberRole { get; private set; }
		/// <summary>
		/// A role that specifies a muted member. The bot will not take actions on messages sent if the sender has this role, and will dissalow members with this role from joining any created channel
		/// </summary>
		public DiscordRole MutedRole { get; private set; }
		/// <summary>
		/// Members with this role are always permitted to use VC commands
		/// </summary>
		public DiscordRole BotManagerRole { get; private set; }
		/// <summary>
		/// The permission default that everyone without a role in the server adopts when creating voice channels
		/// </summary>
		public VoiceChannelCreationPermissions EveryonePermission { get; private set; }

		/// <summary>
		/// The created channel count per member in this server
		/// </summary>
		public Dictionary<DiscordMember, List<DiscordChannel>> CreatedChannels { get; private set; } = new Dictionary<DiscordMember, List<DiscordChannel>>();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="member"></param>
		/// <returns></returns>
		public List<DiscordChannel> this[DiscordMember member]
		{
			get
			{
				if (!CreatedChannels.ContainsKey(member)) return null;
				return CreatedChannels[member];
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public Dictionary<DiscordRole, VoiceChannelCreationPermissions> RolewisePermissions { get; private set; }
		/// <summary>
		/// 
		/// </summary>
		public Dictionary<DiscordMember, VoiceChannelCreationPermissions> MemberwisePermissions { get; private set; }

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
			RolewisePermissions = new Dictionary<DiscordRole, VoiceChannelCreationPermissions>();
			MemberwisePermissions = new Dictionary<DiscordMember, VoiceChannelCreationPermissions>();
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
		/// TODO
		/// </summary>
		/// <param name="parentCategory"></param>
		/// <param name="waitingRoomVc"></param>
		/// <param name="commandListenChannel"></param>
		/// <param name="memberRole"></param>
		/// <param name="mutedRole"></param>
		/// <param name="botManagerRole"></param>
		/// <param name="everyonePermission"></param>
		/// <param name="rolewisePermissions"></param>
		/// <param name="memberwisePermissions"></param>
		/// <param name="createdChannels"></param>
		internal GuildData(DiscordChannel parentCategory, DiscordChannel waitingRoomVc, DiscordChannel commandListenChannel, DiscordRole memberRole, DiscordRole mutedRole, DiscordRole botManagerRole, VoiceChannelCreationPermissions everyonePermission, IEnumerable<KeyValuePair<DiscordRole, VoiceChannelCreationPermissions>> rolewisePermissions, IEnumerable<KeyValuePair<DiscordMember, VoiceChannelCreationPermissions>> memberwisePermissions, IEnumerable<KeyValuePair<DiscordMember, List<DiscordChannel>>> createdChannels) : this(parentCategory, waitingRoomVc, commandListenChannel, memberRole, mutedRole, botManagerRole, everyonePermission, rolewisePermissions, memberwisePermissions)
		{
			CreatedChannels = new Dictionary<DiscordMember, List<DiscordChannel>>(createdChannels);
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
			this.RolewisePermissions = new Dictionary<DiscordRole, VoiceChannelCreationPermissions>(rolewisePermissions ?? new KeyValuePair<DiscordRole, VoiceChannelCreationPermissions>[] { });
			this.MemberwisePermissions = new Dictionary<DiscordMember, VoiceChannelCreationPermissions>(memberwisePermissions ?? new KeyValuePair<DiscordMember, VoiceChannelCreationPermissions>[] { });
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

					if (this.RolewisePermissions.ContainsKey(kvp.Key))
					{
						if (kvp.Value is null)
						{
							this.RolewisePermissions.Remove(kvp.Key);
							continue;
						}

						this.RolewisePermissions[kvp.Key] = kvp.Value;
						continue;
					}

					if (kvp.Value is null) continue;

					this.RolewisePermissions.Add(kvp.Key, kvp.Value);
				}
			}

			if (!(memberwisePermissions is null) && memberwisePermissions.Count() > 0)
			{
				foreach (var kvp in memberwisePermissions)
				{
					if (kvp.Key is null) continue;

					if (this.MemberwisePermissions.ContainsKey(kvp.Key))
					{
						if (kvp.Value is null)
						{
							this.MemberwisePermissions.Remove(kvp.Key);
							continue;
						}

						this.MemberwisePermissions[kvp.Key] = kvp.Value;
					}

					if (kvp.Value is null) continue;

					this.MemberwisePermissions.Add(kvp.Key, kvp.Value);
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
			int existingChannels = (CreatedChannels.ContainsKey(member) ? CreatedChannels[member]?.Count : 0) ?? 0;

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

			if (RolewisePermissions.ContainsKey(highestRole))
			{
				highestPrecedencePermissions = RolewisePermissions[highestRole];
			}

			if (MemberwisePermissions.ContainsKey(member))
			{
				highestPrecedencePermissions = MemberwisePermissions[member];
			}

			return (highestPrecedencePermissions ?? EveryonePermission).ValidateChannelCreationAuthority(!((name is null) || name == string.Empty), requestedPublicity, existingChannels, requestedRegion, out error);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="member"></param>
		/// <returns></returns>
		public List<DiscordChannel> GetChannels(DiscordMember member)
		{
			if (CreatedChannels.ContainsKey(member)) return CreatedChannels[member];
			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="member"></param>
		/// <param name="channel"></param>
		public void AddChannel(DiscordMember member, DiscordChannel channel)
		{
			if (CreatedChannels.ContainsKey(member))
			{
				CreatedChannels[member].Add(channel);
				return;
			}

			CreatedChannels.Add(member, new List<DiscordChannel>(new DiscordChannel[] { channel }));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="member"></param>
		/// <param name="channel"></param>
		/// <returns></returns>
		public bool RemoveChannel(DiscordMember member, DiscordChannel channel)
		{
			if (CreatedChannels.ContainsKey(member) && !(CreatedChannels[member] is null) && CreatedChannels[member].Count > 0)
			{
				return CreatedChannels[member].Remove(channel);
			}

			return false;
		}
	}

	//TODO own file
	public struct SnowflakeContainer<T> where T : SnowflakeObject
	{
		[JsonProperty("snowflake_id")]
		public ulong ID { get; private set; }
		[JsonProperty("snowflake_type")]
		public Type SnowflakeType { get; private set; }

		public SnowflakeContainer(T snowflakeObject)
		{
			if (snowflakeObject is null)
			{
				ID = 0;
				SnowflakeType = null;
				return;
			}

			ID = snowflakeObject.Id;
			SnowflakeType = typeof(T);
		}
	}

	//TODO own file
	public class GuildDataSerializer
	{
		[JsonProperty("parent_category")]
		SnowflakeContainer<DiscordChannel> parentCategoryContainer;
		[JsonProperty("command_listen_channel")]
		SnowflakeContainer<DiscordChannel> commandListenChannelContainer;
		[JsonProperty("waiting_room_vc")]
		SnowflakeContainer<DiscordChannel> waitingRoomVCContainer;
		[JsonProperty("member_role")]
		SnowflakeContainer<DiscordRole> memberRoleContainer;
		[JsonProperty("muted_role")]
		SnowflakeContainer<DiscordRole> mutedRoleContainer;
		[JsonProperty("bot_manager_role")]
		SnowflakeContainer<DiscordRole> botManagerRoleContainer;

		[JsonProperty("everyone_permission")]
		VoiceChannelCreationPermissions everyonePermission;

		[JsonProperty("member_channels")]
		Dictionary<SnowflakeContainer<DiscordMember>, List<SnowflakeContainer<DiscordChannel>>> createdChannelsContainer;
		[JsonProperty("rolewise_permissions")]
		Dictionary<SnowflakeContainer<DiscordRole>, VoiceChannelCreationPermissions> rolewisePermissionContainer;
		[JsonProperty("memberwise_permissions")]
		Dictionary<SnowflakeContainer<DiscordMember>, VoiceChannelCreationPermissions> memberwisePermissionContainer;

		public GuildDataSerializer() { }
		public GuildDataSerializer(GuildData data)
		{
			parentCategoryContainer = new SnowflakeContainer<DiscordChannel>(data.ParentCategory);
			commandListenChannelContainer = new SnowflakeContainer<DiscordChannel>(data.CommandListenChannel);
			waitingRoomVCContainer = new SnowflakeContainer<DiscordChannel>(data.WaitingRoomVC);
			memberRoleContainer = new SnowflakeContainer<DiscordRole>(data.MemberRole);
			mutedRoleContainer = new SnowflakeContainer<DiscordRole>(data.MutedRole);
			botManagerRoleContainer = new SnowflakeContainer<DiscordRole>(data.MutedRole);

			everyonePermission = data.EveryonePermission;

			createdChannelsContainer = new Dictionary<SnowflakeContainer<DiscordMember>, List<SnowflakeContainer<DiscordChannel>>>();
			rolewisePermissionContainer = new Dictionary<SnowflakeContainer<DiscordRole>, VoiceChannelCreationPermissions>();
			memberwisePermissionContainer = new Dictionary<SnowflakeContainer<DiscordMember>, VoiceChannelCreationPermissions>();

			foreach (KeyValuePair<DiscordMember, List<DiscordChannel>> kvp in data.CreatedChannels)
			{
				KeyValuePair<SnowflakeContainer<DiscordMember>, List<SnowflakeContainer<DiscordChannel>>> newAdd = new KeyValuePair<SnowflakeContainer<DiscordMember>, List<SnowflakeContainer<DiscordChannel>>>(new SnowflakeContainer<DiscordMember>(kvp.Key), new List<SnowflakeContainer<DiscordChannel>>());
				foreach (DiscordChannel channel in kvp.Value)
				{
					newAdd.Value.Add(new SnowflakeContainer<DiscordChannel>(channel));
				}
				createdChannelsContainer.Add(newAdd.Key, newAdd.Value);
			}
			foreach (KeyValuePair<DiscordRole, VoiceChannelCreationPermissions> kvp in data.RolewisePermissions)
			{
				rolewisePermissionContainer.Add(new SnowflakeContainer<DiscordRole>(kvp.Key), kvp.Value);
			}
			foreach (KeyValuePair<DiscordMember, VoiceChannelCreationPermissions> kvp in data.MemberwisePermissions)
			{
				memberwisePermissionContainer.Add(new SnowflakeContainer<DiscordMember>(kvp.Key), kvp.Value);
			}
		}

		/// <summary>
		/// Create a <see cref="GuildData"/> object based on deserialized data
		/// </summary>
		/// <param name="workingGuild">Guild to be queried when rebuilding the <see cref="SnowflakeObject"/> references</param>
		/// <returns>A Task representing the rebuilt <see cref="GuildData"/> from deserialized data</returns>
		public async Task<GuildData> CreateGuildDataAsync(DiscordGuild workingGuild)
		{
			//TODO need to do a but ton of error checking and shidma here...just run this through quick to ensure it works

			if (workingGuild is null) return null;

			DiscordChannel parentCategory = workingGuild.GetChannel(parentCategoryContainer.ID);
			DiscordChannel waitingRoomVC = workingGuild.GetChannel(waitingRoomVCContainer.ID);
			DiscordChannel commandListenChannel = workingGuild.GetChannel(commandListenChannelContainer.ID);
			DiscordRole memberRole = workingGuild.GetRole(memberRoleContainer.ID);
			DiscordRole mutedRole = workingGuild.GetRole(mutedRoleContainer.ID);
			DiscordRole botManagerRole = workingGuild.GetRole(botManagerRoleContainer.ID);

			Dictionary<DiscordMember, List<DiscordChannel>> createdChannels = new Dictionary<DiscordMember, List<DiscordChannel>>();
			Dictionary<DiscordRole, VoiceChannelCreationPermissions> rolewisePermissions = new Dictionary<DiscordRole, VoiceChannelCreationPermissions>();
			Dictionary<DiscordMember, VoiceChannelCreationPermissions> memberwisePermissions = new Dictionary<DiscordMember, VoiceChannelCreationPermissions>();

			foreach (var kvp in createdChannelsContainer)
			{
				List<DiscordChannel> memberChannels = new List<DiscordChannel>();
				foreach (SnowflakeContainer<DiscordChannel> channelContainer in kvp.Value)
				{
					memberChannels.Add(workingGuild.GetChannel(channelContainer.ID));
				}
				createdChannels.Add(await workingGuild.GetMemberAsync(kvp.Key.ID), memberChannels);
			}
			foreach (var kvp in rolewisePermissionContainer)
			{
				rolewisePermissions.Add(workingGuild.GetRole(kvp.Key.ID), kvp.Value);
			}
			foreach (var kvp in memberwisePermissionContainer)
			{
				memberwisePermissions.Add(await workingGuild.GetMemberAsync(kvp.Key.ID), kvp.Value);
			}



			return new GuildData(parentCategory, waitingRoomVC, commandListenChannel, memberRole, mutedRole, botManagerRole, everyonePermission, rolewisePermissions, memberwisePermissions, createdChannels);
		}
	}
}
