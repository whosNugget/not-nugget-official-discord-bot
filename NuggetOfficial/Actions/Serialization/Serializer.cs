using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Text.Json;

namespace NuggetOfficial.Actions.Serialization
{
	public enum SerializationFormat { JSON, XML, Binary }
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
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="directory"></param>
		/// <param name="filename"></param>
		/// <param name="instance"></param>
		/// <param name="output"></param>
		/// <returns></returns>
		public static async Task<AsyncSerializationResult> SerializeAsync<T>(string directory, string filename, T instance, SerializationFormat output)
		{
			bool result = false;
			string error = string.Empty;

			//TODO validate the directory and filename (or just let the try catch work it's magic)

			try
			{
				Attribute[] attributes = Attribute.GetCustomAttributes(instance.GetType());
				if (!attributes.Contains(new SerializableAttribute()))
				{
					error = $"The provided object {nameof(T)} was not marked as serializable";
					goto Completed;
				}

				using (Stream serializeStream = new FileStream($"{directory}{filename}", FileMode.OpenOrCreate))
				{
					switch (output)
					{
						case SerializationFormat.JSON:
							await JsonSerializer.SerializeAsync(serializeStream, instance);
							break;

						case SerializationFormat.XML:
							await Task.Run(() =>
							{
								XmlSerializer xmlS = new XmlSerializer(typeof(T));
								xmlS.Serialize(serializeStream, instance);
							});
							break;

						case SerializationFormat.Binary:
							await Task.Run(() =>
							{
								BinaryFormatter binS = new BinaryFormatter();
								binS.Serialize(serializeStream, instance);
							});
							break;
					}
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

		public static async Task<AsyncDeserializationResult<T>> DeserializeAsync<T>(string filePath, SerializationFormat format)
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
				using (Stream serializeStream = new FileStream(filePath, FileMode.Open))
				{
					switch (format)
					{
						case SerializationFormat.JSON:
							instance = await JsonSerializer.DeserializeAsync<T>(serializeStream);
							break;

						case SerializationFormat.XML:
							await Task.Run(() =>
							{
								XmlSerializer xmlS = new XmlSerializer(typeof(T));
								instance = (T)xmlS.Deserialize(serializeStream);
							});
							break;

						case SerializationFormat.Binary:
							await Task.Run(() =>
							{
								BinaryFormatter binS = new BinaryFormatter();
								instance = (T)binS.Deserialize(serializeStream);
							});
							break;
					}
				}

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
