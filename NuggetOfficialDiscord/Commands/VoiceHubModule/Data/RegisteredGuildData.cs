﻿using DSharpPlus;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using NuggetOfficial.Discord.Commands.VoiceHubModule.Data.Permissions;
using NuggetOfficial.Discord.Serialization;
using NuggetOfficial.Discord.Serialization.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NuggetOfficial.Discord.Commands.VoiceHubModule.Data
{
    //TODO abstractify this class so it can be more broad/dynamic maybe?
    //should store channel, user, and permission data on a per-guild basis
    //should be serializable and saved before the bot is terminated
    //should be loaded from a file every time the bot starts if one exists
    /// <summary>
    /// This class holds references to DiscordGuilds and their subsequent data used by the bot's VC features. [NYI/NYT] This class can be directly serialized and deserialized
    /// </summary>
    [JsonDictionary("registered_guilds")]
    public class RegisteredGuildData : ISerializable
    {
        [JsonIgnore] //TODO Create a newtonsoft converter that converts GuildData objects to GuildDataSerializableContainer objects and vice-versa (if possible, i require both a discord client and a discord guild to deserialize this stuff)
        public Dictionary<DiscordGuild, GuildData> RegisteredGuilds { get; private set; } = new Dictionary<DiscordGuild, GuildData>();

        [JsonIgnore]
        public bool Rebuilding { get => rebuilding; }
        [JsonIgnore]
        private bool rebuilding = false;

        [JsonProperty("guild_data")]
        private readonly Dictionary<ulong, GuildDataSerializableContainer> serializableContainer = new();

        public RegisteredGuildData() { }

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
        public bool RegisterGuild(DiscordGuild toRegister, DiscordChannel parentCategory, DiscordChannel waitingRoomVc, DiscordChannel commandListenChannel, DiscordChannel loggingChannel, DiscordRole memberRole, DiscordRole mutedRole, DiscordRole botManagerRole, out string error)
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

            RegisteredGuilds.Add(toRegister, new GuildData(parentCategory, waitingRoomVc, commandListenChannel, loggingChannel, memberRole, mutedRole, botManagerRole));
            UpdateSerializableGuildDataContainer(toRegister);

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
        public bool InitializeGuildPermissions(DiscordGuild guild, ChannelAuthorizations everyonePermission, IEnumerable<KeyValuePair<DiscordRole, ChannelAuthorizations>> rolewisePermissions, IEnumerable<KeyValuePair<DiscordMember, ChannelAuthorizations>> memberwisePermissions, out string error)
        {
            error = string.Empty;

            if (!RegisteredGuilds.ContainsKey(guild))
            {
                error = "Cannot initialize permissions on a guild which is not registered";
                goto Completed;
            }

            RegisteredGuilds[guild].InitializePermissions(everyonePermission, rolewisePermissions, memberwisePermissions);
            UpdateSerializableGuildDataContainer(guild);

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
        public bool UpdateGuildPermissions(DiscordGuild guild, ChannelAuthorizations everyonePermission, IEnumerable<KeyValuePair<DiscordRole, ChannelAuthorizations>> rolewisePermissions, IEnumerable<KeyValuePair<DiscordMember, ChannelAuthorizations>> memberwisePermissions, out string error)
        {
            error = string.Empty;

            if (!RegisteredGuilds.ContainsKey(guild))
            {
                error = "Cannot update permissions on a guild which is not registered";
                goto Completed;
            }

            RegisteredGuilds[guild].UpdatePermissions(everyonePermission, rolewisePermissions, memberwisePermissions);
            UpdateSerializableGuildDataContainer(guild);

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

                serializableContainer.Remove(toRemove.Id);
                goto Completed;
            }

            error = "Cannot remove a guild which was not registered";

        Completed:
            return error == string.Empty;
        }

        private void UpdateSerializableGuildDataContainer(DiscordGuild guild)
        {
            serializableContainer[guild.Id] = RegisteredGuilds[guild].CreateSerializableContainer();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public async Task<string> RebuildDeserializedDataFromClient(DiscordClient client)
        {
            string error = string.Empty;

            if (serializableContainer is null) return "No deserialized data";

            rebuilding = true;
            RegisteredGuilds = new Dictionary<DiscordGuild, GuildData>();

            foreach (var kvp in serializableContainer)
            {
                if (client.Guilds.ContainsKey(kvp.Key))
                {
                    DiscordGuild guild = client.Guilds[kvp.Key];
                    GuildData guildData = await kvp.Value.CreateGuildDataAsync(client.Guilds[kvp.Key]);

                    RegisteredGuilds.Add(guild, guildData);
                }
            }

            rebuilding = false;
            return error;
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
		/// The channel that the bot should log it's actions to in addition to logging to the default audit log. If logging is not desired, this can be set to null
		/// </summary>
		public DiscordChannel LoggingChannel { get; private set; }
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
        public ChannelAuthorizations EveryonePermission { get; private set; }

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
        public Dictionary<DiscordRole, ChannelAuthorizations> RolewisePermissions { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<DiscordMember, ChannelAuthorizations> MemberwisePermissions { get; private set; }

        public GuildData() { }
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="parentCategory"></param>
        /// <param name="waitingRoomVc"></param>
        /// <param name="commandListenChannel"></param>
        /// <param name="memberRole"></param>
        /// <param name="mutedRole"></param>
        public GuildData(DiscordChannel parentCategory, DiscordChannel waitingRoomVc, DiscordChannel commandListenChannel, DiscordChannel loggingChannel, DiscordRole memberRole, DiscordRole mutedRole, DiscordRole botManagerRole)
        {
            ParentCategory = parentCategory;
            WaitingRoomVC = waitingRoomVc;
            CommandListenChannel = commandListenChannel;
            LoggingChannel = loggingChannel;
            MemberRole = memberRole;
            MutedRole = mutedRole;
            BotManagerRole = botManagerRole;

            EveryonePermission = null;
            RolewisePermissions = new Dictionary<DiscordRole, ChannelAuthorizations>();
            MemberwisePermissions = new Dictionary<DiscordMember, ChannelAuthorizations>();
        }
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="parentCategory"></param>
        /// <param name="waitingRoomVc"></param>
        /// <param name="commandListenChannel"></param>
        /// <param name="loggingChannel"></param>
        /// <param name="memberRole"></param>
        /// <param name="mutedRole"></param>
        /// <param name="everyonePermission"></param>
        /// <param name="rolewisePermissions"></param>
        /// <param name="memberwisePermissions"></param>
        public GuildData(DiscordChannel parentCategory, DiscordChannel waitingRoomVc, DiscordChannel commandListenChannel, DiscordChannel loggingChannel, DiscordRole memberRole, DiscordRole mutedRole, DiscordRole botManagerRole, ChannelAuthorizations everyonePermission, IEnumerable<KeyValuePair<DiscordRole, ChannelAuthorizations>> rolewisePermissions, IEnumerable<KeyValuePair<DiscordMember, ChannelAuthorizations>> memberwisePermissions) : this(parentCategory, waitingRoomVc, commandListenChannel, loggingChannel, memberRole, mutedRole, botManagerRole)
        {
            InitializePermissions(everyonePermission, rolewisePermissions, memberwisePermissions);
        }
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="parentCategory"></param>
        /// <param name="waitingRoomVc"></param>
        /// <param name="commandListenChannel"></param>
        /// <param name="loggingChannel"></param>
        /// <param name="memberRole"></param>
        /// <param name="mutedRole"></param>
        /// <param name="botManagerRole"></param>
        /// <param name="everyonePermission"></param>
        /// <param name="rolewisePermissions"></param>
        /// <param name="memberwisePermissions"></param>
        /// <param name="createdChannels"></param>
        internal GuildData(DiscordChannel parentCategory, DiscordChannel waitingRoomVc, DiscordChannel commandListenChannel, DiscordChannel loggingChannel, DiscordRole memberRole, DiscordRole mutedRole, DiscordRole botManagerRole, ChannelAuthorizations everyonePermission, IEnumerable<KeyValuePair<DiscordRole, ChannelAuthorizations>> rolewisePermissions, IEnumerable<KeyValuePair<DiscordMember, ChannelAuthorizations>> memberwisePermissions, IEnumerable<KeyValuePair<DiscordMember, List<DiscordChannel>>> createdChannels) : this(parentCategory, waitingRoomVc, commandListenChannel, loggingChannel, memberRole, mutedRole, botManagerRole, everyonePermission, rolewisePermissions, memberwisePermissions)
        {
            CreatedChannels = new Dictionary<DiscordMember, List<DiscordChannel>>(createdChannels);
        }

        /// <summary>
        /// TODO Documentation comment
        /// </summary>
        /// <param name="everyonePermission"></param>
        /// <param name="rolewisePermissions"></param>
        /// <param name="memberwisePermissions"></param>
        public void InitializePermissions(ChannelAuthorizations everyonePermission, IEnumerable<KeyValuePair<DiscordRole, ChannelAuthorizations>> rolewisePermissions, IEnumerable<KeyValuePair<DiscordMember, ChannelAuthorizations>> memberwisePermissions)
        {
            this.EveryonePermission = everyonePermission ?? new ChannelAuthorizations();
            this.RolewisePermissions = new Dictionary<DiscordRole, ChannelAuthorizations>(rolewisePermissions ?? new KeyValuePair<DiscordRole, ChannelAuthorizations>[] { });
            this.MemberwisePermissions = new Dictionary<DiscordMember, ChannelAuthorizations>(memberwisePermissions ?? new KeyValuePair<DiscordMember, ChannelAuthorizations>[] { });
        }

        /// <summary>
        /// Updates all provided existing permissions, removes keys whos values are null, adds keys whos values are not null and don't exist in the current dictionary
        /// </summary>
        /// <param name="everyonePermission">Updated everyone permission. Set null to ignore</param>
        /// <param name="rolewisePermissions">Updated rolewise permissions. Set null to ignore, set value to null to remove coresponding key from dictionary, set key and value to update existing or add new value repsectively</param>
        /// <param name="memberwisePermissions">Updated memberwise permissions. Set null to ignore, set value to null to remove coresponding key from dictionary, set key and value to update existing or add new value repsectively</param>
        public void UpdatePermissions(ChannelAuthorizations everyonePermission, IEnumerable<KeyValuePair<DiscordRole, ChannelAuthorizations>> rolewisePermissions, IEnumerable<KeyValuePair<DiscordMember, ChannelAuthorizations>> memberwisePermissions)
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

        ///// <summary>
        ///// TODO Documentation comment
        ///// </summary>
        ///// <param name="member"></param>
        ///// <param name="requestedPublicity"></param>
        ///// <param name="requestedRegion"></param>
        ///// <param name="error"></param>
        ///// <returns></returns>
        //public bool CheckPermission(DiscordMember member, ChannelCreationParameters parameters, out string[] errors)
        //{
        //	//int defaultBitrate = 64000;
        //	//int existingChannels = (CreatedChannels.ContainsKey(member) ? CreatedChannels[member]?.Count : 0) ?? 0;
        //	return GetMemberPermissions(member).VerifyAuthority(parameters, out errors);
        //}

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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public GuildDataSerializableContainer CreateSerializableContainer()
        {
            return new GuildDataSerializableContainer(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public ChannelAuthorizations GetMemberPermissions(DiscordMember member)
        {
            ChannelAuthorizations highestPrecedencePermissions = null;

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

            return highestPrecedencePermissions ?? EveryonePermission;
        }
    }

    //TODO own file
    [JsonObject("snowflake_container")]
    public struct SnowflakeSerializationContainer
    {
        [JsonProperty("snowflake_id")]
        public ulong ID { get; private set; }

        public SnowflakeSerializationContainer(SnowflakeObject snowflake)
        {
            ID = snowflake is null ? ulong.MinValue : snowflake.Id;
        }
    }

    //TODO own file
    [JsonObject("guild_data_container")]
    public class GuildDataSerializableContainer
    {
        [JsonProperty("parent_category")]
        public SnowflakeSerializationContainer ParentCategoryContainer { get; private set; }
        [JsonProperty("command_listen_channel")]
        public SnowflakeSerializationContainer CommandListenChannelContainer { get; private set; }
        [JsonProperty("waiting_room_vc")]
        public SnowflakeSerializationContainer WaitingRoomVCContainer { get; private set; }
        [JsonProperty("logging_channel")]
        public SnowflakeSerializationContainer LoggingChannelContainer { get; private set; }
        [JsonProperty("member_role")]
        public SnowflakeSerializationContainer MemberRoleContainer { get; private set; }
        [JsonProperty("muted_role")]
        public SnowflakeSerializationContainer MutedRoleContainer { get; private set; }
        [JsonProperty("bot_manager_role")]
        public SnowflakeSerializationContainer BotManagerRoleContainer { get; private set; }

        [JsonProperty("everyone_permission")]
        public ChannelAuthorizations EveryonePermission { get; private set; }

        [JsonProperty("member_channels")]
        public Dictionary<ulong, List<SnowflakeSerializationContainer>> CreatedChannelsContainer { get; private set; }
        [JsonProperty("rolewise_permissions")]
        public Dictionary<ulong, ChannelAuthorizations> RolewisePermissionContainer { get; private set; }
        [JsonProperty("memberwise_permissions")]
        public Dictionary<ulong, ChannelAuthorizations> MemberwisePermissionContainer { get; private set; }

        public GuildDataSerializableContainer() { }
        public GuildDataSerializableContainer(GuildData data)
        {
            ParentCategoryContainer = new SnowflakeSerializationContainer(data.ParentCategory);
            CommandListenChannelContainer = new SnowflakeSerializationContainer(data.CommandListenChannel);
            WaitingRoomVCContainer = new SnowflakeSerializationContainer(data.WaitingRoomVC);
            LoggingChannelContainer = new SnowflakeSerializationContainer(data.LoggingChannel);
            MemberRoleContainer = new SnowflakeSerializationContainer(data.MemberRole);
            MutedRoleContainer = new SnowflakeSerializationContainer(data.MutedRole);
            BotManagerRoleContainer = new SnowflakeSerializationContainer(data.MutedRole);

            EveryonePermission = data.EveryonePermission;

            CreatedChannelsContainer = new Dictionary<ulong, List<SnowflakeSerializationContainer>>();
            RolewisePermissionContainer = new Dictionary<ulong, ChannelAuthorizations>();
            MemberwisePermissionContainer = new Dictionary<ulong, ChannelAuthorizations>();

            foreach (KeyValuePair<DiscordMember, List<DiscordChannel>> kvp in data.CreatedChannels)
            {
                KeyValuePair<ulong, List<SnowflakeSerializationContainer>> newAdd = new KeyValuePair<ulong, List<SnowflakeSerializationContainer>>(kvp.Key.Id, new List<SnowflakeSerializationContainer>());
                foreach (DiscordChannel channel in kvp.Value)
                {
                    newAdd.Value.Add(new SnowflakeSerializationContainer(channel));
                }
                CreatedChannelsContainer.Add(newAdd.Key, newAdd.Value);
            }
            foreach (KeyValuePair<DiscordRole, ChannelAuthorizations> kvp in data.RolewisePermissions)
            {
                RolewisePermissionContainer.Add(kvp.Key.Id, kvp.Value);
            }
            foreach (KeyValuePair<DiscordMember, ChannelAuthorizations> kvp in data.MemberwisePermissions)
            {
                MemberwisePermissionContainer.Add(kvp.Key.Id, kvp.Value);
            }
        }

        /// <summary>
        /// Create a <see cref="GuildData"/> object based on deserialized data
        /// </summary>
        /// <param name="workingGuild">Guild to be queried when rebuilding the <see cref="SnowflakeObject"/> references</param>
        /// <returns>A Task representing the rebuilt <see cref="GuildData"/> from deserialized data</returns>
        public async Task<GuildData> CreateGuildDataAsync(DiscordGuild workingGuild)
        {
            //TODO need to do a butt ton of error checking and shidma here...just run this through quick to ensure it works

            if (workingGuild is null) return null;

            DiscordChannel parentCategory = workingGuild.GetChannel(ParentCategoryContainer.ID);
            DiscordChannel waitingRoomVC = workingGuild.GetChannel(WaitingRoomVCContainer.ID);
            DiscordChannel commandListenChannel = workingGuild.GetChannel(CommandListenChannelContainer.ID);
            DiscordChannel loggingChannel = workingGuild.GetChannel(LoggingChannelContainer.ID);
            DiscordRole memberRole = workingGuild.GetRole(MemberRoleContainer.ID);
            DiscordRole mutedRole = workingGuild.GetRole(MutedRoleContainer.ID);
            DiscordRole botManagerRole = workingGuild.GetRole(BotManagerRoleContainer.ID);

            Dictionary<DiscordMember, List<DiscordChannel>> createdChannels = new Dictionary<DiscordMember, List<DiscordChannel>>();
            Dictionary<DiscordRole, ChannelAuthorizations> rolewisePermissions = new Dictionary<DiscordRole, ChannelAuthorizations>();
            Dictionary<DiscordMember, ChannelAuthorizations> memberwisePermissions = new Dictionary<DiscordMember, ChannelAuthorizations>();

            foreach (var kvp in CreatedChannelsContainer)
            {
                List<DiscordChannel> memberChannels = new List<DiscordChannel>();
                foreach (SnowflakeSerializationContainer channelContainer in kvp.Value)
                {
                    memberChannels.Add(workingGuild.GetChannel(channelContainer.ID));
                }
                createdChannels.Add(await workingGuild.GetMemberAsync(kvp.Key), memberChannels);
            }
            foreach (var kvp in RolewisePermissionContainer)
            {
                rolewisePermissions.Add(workingGuild.GetRole(kvp.Key), kvp.Value);
            }
            foreach (var kvp in MemberwisePermissionContainer)
            {
                memberwisePermissions.Add(await workingGuild.GetMemberAsync(kvp.Key), kvp.Value);
            }

            return new GuildData(parentCategory, waitingRoomVC, commandListenChannel, loggingChannel, memberRole, mutedRole, botManagerRole, EveryonePermission, rolewisePermissions, memberwisePermissions, createdChannels);
        }
    }
}
