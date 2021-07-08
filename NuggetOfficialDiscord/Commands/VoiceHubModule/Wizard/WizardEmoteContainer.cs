using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;

namespace NuggetOfficialDiscord.Commands.VoiceHubModule.Wizard
{
	public class WizardEmoteContainer
	{
		private struct EmoteValueContainer
		{
			public DiscordEmoji Emoji { get; set; }
			public Type ValueType { get; set; }
			public object Value { get; set; }
		}

		public DiscordClient Client { get; }

		private readonly Dictionary<string, EmoteValueContainer> emojiValues;

		public DiscordEmoji Cancel { get; }
		public DiscordEmoji Yes { get; }
		public DiscordEmoji No { get; }

		public DiscordEmoji this[string name]
		{
			get
			{
				if (emojiValues.ContainsKey(name))
				{
					return emojiValues[name].Emoji;
				}

				return null;
			}
		}

		public WizardEmoteContainer(DiscordClient client)
		{
			Client = client;
			emojiValues = new Dictionary<string, EmoteValueContainer>();

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

		public void SetEmojiValue<T>(DiscordEmoji emoji, T representedValue)
		{
			if (emojiValues.ContainsKey(emoji?.Name))
			{
				EmoteValueContainer v = emojiValues[emoji.Name];

				if (typeof(T) != emojiValues[emoji.Name].ValueType)
				{
					v.ValueType = typeof(T);
				}
				v.Value = representedValue;

				emojiValues[emoji.Name] = v;
				return;
			}

			emojiValues.Add(emoji.Name, new EmoteValueContainer { Emoji = emoji, ValueType = typeof(T), Value = representedValue });
		}

		public bool GetEmojiValue<T>(DiscordEmoji emoji, out T value)
		{
			value = default;

			if (emojiValues.ContainsKey(emoji?.Name) && emojiValues[emoji.Name].ValueType == typeof(T))
			{
				value = (T)emojiValues[emoji.Name].Value;
				return true;
			}

			return false;
		}
	}
}
