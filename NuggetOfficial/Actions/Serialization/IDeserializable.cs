using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NuggetOfficial.Actions.Serialization.Interfaces
{
	/// <summary>
	/// Indicates a class which can be serialized
	/// </summary>
	/// <typeparam name="T">Object <see cref="Type"/> which can be asynchrounously serialized</typeparam>
	public interface IDeserializable<T> where T : new()
	{
		DeserializationResult<T> Deserialize(TextReader reader, JsonSerializer serializer);
	}
}
