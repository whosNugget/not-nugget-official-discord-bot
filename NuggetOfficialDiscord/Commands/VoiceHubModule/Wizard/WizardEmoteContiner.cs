using DSharpPlus;
using DSharpPlus.Entities;
using System.Collections.Generic;

namespace NuggetOfficialDiscord.Commands.VoiceHubModule.Wizard
{
	public class WizardEmoteContiner
	{
		public DiscordClient Client { get; }

		public Dictionary<string, DiscordEmoji> RegisteredEmoji { get; }
		public DiscordEmoji Cancel { get; }
		public DiscordEmoji Yes { get; }
		public DiscordEmoji No { get; }

		/// <summary>
		/// Get the registerred <see cref="DiscordEmoji"/> instance by existing instance if one exists.
		/// </summary>
		/// <param name="emoji">Emoji instance to get registered instance from</param>
		/// <returns>Registered <see cref="DiscordEmoji"/> instance if one matches the provided instance, null if otherwise</returns>
		public DiscordEmoji this[DiscordEmoji emoji]
		{
			get
			{
				if (RegisteredEmoji.ContainsKey(emoji.Name)) return RegisteredEmoji[emoji.Name];

				return null;
			}
		}
		/// <summary>
		/// Get a <see cref="DiscordEmoji"/> by name. You should use <see cref="this[DiscordEmoji]"/> to retrieve <see cref="DiscordEmoji"/> when possible
		/// </summary>
		/// <param name="name">Name of the emoji to get</param>
		/// <returns>An instance of the desired <see cref="DiscordEmoji"/> if one exists, null if otherwise</returns>
		public DiscordEmoji this[string name]
		{
			get
			{
				if (RegisteredEmoji.ContainsKey(name)) return RegisteredEmoji[name];

				return null;
			}
		}
		DiscordEmoji this[string name, bool overwrite = false]
		{
			set
			{
				if (value is null) return;

				if (RegisteredEmoji.ContainsKey(name)) RegisteredEmoji[name] = overwrite ? RegisteredEmoji[name] : value;

				RegisteredEmoji.Add(name, value);
			}
		}

		public WizardEmoteContiner(DiscordClient client)
		{
			Client = client;
			RegisteredEmoji = new Dictionary<string, DiscordEmoji>();

			Cancel = DiscordEmoji.FromName(client, "no_entry_sign", false);
			Yes = DiscordEmoji.FromName(client, "white_check_mark", false);
			No = DiscordEmoji.FromName(client, "negative_squared_cross_mark", false);
		}
		public WizardEmoteContiner(DiscordClient client, DiscordEmoji customCancel, DiscordEmoji customYes, DiscordEmoji customNo) : this(client)
		{
			Cancel = DiscordEmoji.FromName(client, customCancel ?? "no_entry_sign");
			Yes = DiscordEmoji.FromName(client, customYes ?? "white_check_mark");
			No = DiscordEmoji.FromName(client, customNo ?? "negative_squared_cross_mark");
		}

		public void RegisterEmoji(DiscordEmoji emoji, bool overwrite)
		{
			this[emoji.Name, overwrite] = emoji;
		}
	}
}
