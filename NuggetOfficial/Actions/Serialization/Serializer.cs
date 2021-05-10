using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using Newtonsoft.Json;
using DSharpPlus.Entities;
using DSharpPlus.Net.Serialization;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Reflection;

namespace NuggetOfficial.Actions.Serialization
{
	/// <summary>
	/// Result container that specifies the success of the serialization operation and any error messages if one occurred
	/// </summary>
	public struct AsyncSerializationResult
	{
		public bool Success { get; private set; }
		public string Error { get; private set; }

		public AsyncSerializationResult(bool success, string error)
		{
			Success = success;
			Error = error;
		}
	}
	/// <summary>
	/// /// Result container that specifies the success of the deserialization operation, any error messages if one occurred, and the instance of the deserialized object
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public struct AsyncDeserializationResult<T>
	{
		public bool Success { get; private set; }
		public string Error { get; private set; }
		public T Instance { get; private set; }

		public AsyncDeserializationResult(bool success, string error, T instance)
		{
			Success = success;
			Error = error;
			Instance = instance;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public static class Serializer
	{
		public class StringToGenericSnowflake<T> : JsonConverter where T : SnowflakeObject
		{
			//public override bool CanConvert(Type objectType)
			//{
			//	return objectType == typeof(T);
			//}

			//public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
			//{
			//	DiscordJson.PopulateObject(JToken.ReadFrom(reader), existingValue);
			//	return existingValue;
			//}

			//public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
			//{
			//	if (value is null) writer.WriteNull();
			//	else
			//	{
			//		serializer.Serialize(writer, value);
			//	}
			//}
			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
			{
				if (value == null)
				{
					writer.WriteNull();
				}
				else
				{
					var type = value.GetType().GetTypeInfo();
					JToken.FromObject(type.GetDeclaredProperty("Values").GetValue(value)).WriteTo(writer);
				}
			}

			public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
			{
				var constructor = objectType.GetTypeInfo().DeclaredConstructors
					.FirstOrDefault(e => !e.IsStatic && e.GetParameters().Length == 0);

				var dict = constructor.Invoke(new object[] { });

				// the default name of an indexer is "Item"
				var properties = objectType.GetTypeInfo().GetDeclaredProperty("Item");

				var entries = (System.Collections.IEnumerable)serializer.Deserialize(reader, objectType.GenericTypeArguments[1].MakeArrayType());
				foreach (var entry in entries)
				{
					properties.SetValue(dict, entry, new object[]
					{
					(entry as SnowflakeObject)?.Id
					?? (entry as DiscordVoiceState)?.User?.Id
					?? throw new InvalidOperationException($"Type {entry?.GetType()} is not deserializable")
					});
				}

				return dict;
			}

			public override bool CanConvert(Type objectType)
			{
				if (!objectType.IsGenericType) return false;
				var genericTypedef = objectType.GetGenericTypeDefinition();
				if (genericTypedef != typeof(Dictionary<,>) && genericTypedef != typeof(ConcurrentDictionary<,>)) return false;
				if (objectType.GenericTypeArguments[0] != typeof(ulong)) return false;

				var valueParam = objectType.GenericTypeArguments[1];
				return typeof(SnowflakeObject).GetTypeInfo().IsAssignableFrom(valueParam.GetTypeInfo()) ||
					   valueParam == typeof(DiscordVoiceState);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="directory"></param>
		/// <param name="filename"></param>
		/// <param name="instance"></param>
		/// <param name="output"></param>
		/// <returns></returns>
		public static async Task<AsyncSerializationResult> SerializeAsync<T>(string directory, string filename, T instance)
		{
			bool result = false;
			string error = string.Empty;

			//TODO validate the directory and filename (or just let the try catch work it's magic)

			try
			{
				await using (TextWriter serializeStream = new StreamWriter($"{directory}{filename}", false))
				{
					await Task.Run(() =>
					{
						JsonSerializer jsonS = JsonSerializer.Create(new JsonSerializerSettings { Formatting = Formatting.Indented });
						jsonS.Serialize(serializeStream, instance, typeof(T));
					});
				}

				result = true;
			}
			catch (Exception e)
			{
				error = e.Message;
				goto Completed;
			}

		Completed:
			return new AsyncSerializationResult(result, error);
		}

		public static async Task<AsyncDeserializationResult<T>> DeserializeAsync<T>(string filePath)
		{
			bool result = false;
			string error = string.Empty;
			T instance = default;

			if (!File.Exists(filePath))
			{
				error = "No file exists at the provided file path or the program does not have permission to access the file at the provided location";
				goto Completed;
			}

			try
			{
				await Task.Run(() =>
				{
					using TextReader deserializeStream = new StreamReader(filePath);
					JsonSerializer jsonS = JsonSerializer.Create(new JsonSerializerSettings { Converters = new List<JsonConverter>(new JsonConverter[] { new StringToGenericSnowflake<DiscordGuild>(), new StringToGenericSnowflake<DiscordRole>(), new StringToGenericSnowflake<DiscordMember>(), new StringToGenericSnowflake<DiscordChannel>() }) });
					instance = (T)jsonS.Deserialize(deserializeStream, typeof(T));
				});

				result = true;
			}
			catch (Exception e)
			{
				error = e.Message;
				goto Completed;
			}

		Completed:
			return new AsyncDeserializationResult<T>(result, error, instance);
		}
	}
}
