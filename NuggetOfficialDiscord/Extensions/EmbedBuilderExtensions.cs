using DSharpPlus.Entities;
using System.Collections.Generic;

namespace NuggetOfficialDiscord.Extensions
{
    public static class EmbedBuilderExtensions
    {
        /// <summary>
        /// Adds a field to the builder who's title matches the string representation of the provided <see cref="KeyValuePair{TKey, TValue}"/> instance's key
        /// and who's description matches the string representation of the same provided pair's value
        /// </summary>
        /// <typeparam name="Key">Title type to represent as a string</typeparam>
        /// <typeparam name="Value">Description type to represent as a string</typeparam>
        /// <param name="builder">Instance of <see cref="DiscordEmbedBuilder"/> to add the field to</param>
        /// <param name="kvp">Title and Description pair to write to the builder</param>
        /// <param name="inline">TODO</param>
        /// <returns>The same instance of <see cref="DiscordEmbedBuilder"/> with the provided field added to it</returns>
        public static DiscordEmbedBuilder AddField<Key, Value>(this DiscordEmbedBuilder builder, KeyValuePair<Key, Value> kvp, bool inline = false)
        {
            return builder.AddField(kvp.Key.ToString(), kvp.Value.ToString(), inline);
        }

        /// <summary>
        /// Adds multiple fields to the builder where each field's title matches the string representation of the corresponding <see cref="KeyValuePair{TKey, TValue}"/> instance's key
        /// and who's description matches the string representation of the same corresponding pair's value
        /// </summary>
        /// <typeparam name="Key">Title type to represent as a string</typeparam>
        /// <typeparam name="Value">Description type to represent as a string</typeparam>
        /// <param name="builder">Instance of <see cref="DiscordEmbedBuilder"/> to add the fields to</param>
        /// <param name="pairs">Title and Description pairs to write to the builder</param>
        /// <param name="inline">TODO</param>
        /// <returns>The same instance of <see cref="DiscordEmbedBuilder"/> with the provided fields added to it</returns>
        public static DiscordEmbedBuilder AddFields<Key, Value>(this DiscordEmbedBuilder builder, IEnumerable<KeyValuePair<Key, Value>> pairs, bool inline = false)
        {
            foreach (var kvp in pairs)
            {
                builder.AddField(kvp, inline);
            }

            return builder;
        }
    }
}
