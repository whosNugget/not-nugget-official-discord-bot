using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;

namespace NuggetOfficialDiscord.Commands.VoiceHubModule.Wizard
{
	public class WizardEmoteContainer
	{
		public DiscordClient Client { get; }

		public Dictionary<Type, Dictionary<string, object>> EmojiValueRepresentation { get; }

		public DiscordEmoji Cancel { get; }
		public DiscordEmoji Yes { get; }
		public DiscordEmoji No { get; }

		public WizardEmoteContainer(DiscordClient client)
		{
			Client = client;
			EmojiValueRepresentation = new Dictionary<Type, Dictionary<string, object>>();

			Cancel = DiscordEmoji.FromName(client, "no_entry_sign", false);
			Yes = DiscordEmoji.FromName(client, "white_check_mark", false);
			No = DiscordEmoji.FromName(client, "negative_squared_cross_mark", false);
		}
		public WizardEmoteContainer(DiscordClient client, DiscordEmoji customCancel, DiscordEmoji customYes, DiscordEmoji customNo) : this(client)
		{
			Cancel = DiscordEmoji.FromName(client, customCancel ?? "no_entry_sign");
			Yes = DiscordEmoji.FromName(client, customYes ?? "white_check_mark");
			No = DiscordEmoji.FromName(client, customNo ?? "negative_squared_cross_mark");
		}

		public void RegisterEmojiValueRepresentation<T>(DiscordEmoji emoji, T representedValue, bool overwrite)
		{
			if (EmojiValueRepresentation.ContainsKey(typeof(T)) && EmojiValueRepresentation[typeof(T)].ContainsKey(emoji.Name))
			{
				EmojiValueRepresentation[typeof(T)][emoji.Name] = overwrite ? representedValue : EmojiValueRepresentation[typeof(T)][emoji.Name];
			}
		}

		public bool RetrieveValueRepresentation<T>(DiscordEmoji emoji, out T value)
		{
			value = default;

			if (EmojiValueRepresentation.ContainsKey(typeof(T)) && EmojiValueRepresentation[typeof(T)].ContainsKey(emoji.Name))
			{
				value = (T)EmojiValueRepresentation[typeof(T)][emoji.Name];
				return true;
			}

			return false;
		}
	}
}
