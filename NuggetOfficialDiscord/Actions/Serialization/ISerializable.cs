using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NuggetDiscordBot.Actions.Serialization.Interfaces
{
	/// <summary>
	/// Indicates a class which can be serialized
	/// </summary>
	/// <typeparam name="T">Object <see cref="Type"/> which can be asynchrounously serialized</typeparam>
	public interface ISerializable<T>
	{
		SerializationResult Serialize(TextWriter writer, JsonSerializer serializer);
	}
}
