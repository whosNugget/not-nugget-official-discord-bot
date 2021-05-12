using DSharpPlus.Entities;
using Newtonsoft.Json;
using NuggetOfficial.Actions.Serialization.Interfaces;
using NuggetOfficial.Data.VoiceModule;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NuggetOfficial.Actions.Serialization
{
	///// <summary>
	///// Result container that specifies the success of the serialization operation and any error messages if one occurred
	///// </summary>
	//public struct AsyncSerializationResult
	//{
	//	public bool Success { get; private set; }
	//	public string ErrorMessage { get; private set; }

	//	public AsyncSerializationResult(bool success, string error)
	//	{
	//		Success = success;
	//		ErrorMessage = error;
	//	}
	//}
	///// <summary>
	///// /// Result container that specifies the success of the deserialization operation, any error messages if one occurred, and the instance of the deserialized object
	///// </summary>
	///// <typeparam name="T"></typeparam>
	//public struct AsyncDeserializationResult<T>
	//{
	//	public bool Success { get; private set; }
	//	public string ErrorMessage { get; private set; }
	//	public T DeserializedInstance { get; private set; }

	//	public AsyncDeserializationResult(bool success, string error, T instance)
	//	{
	//		Success = success;
	//		ErrorMessage = error;
	//		DeserializedInstance = instance;
	//	}
	//}

	/// <summary>
	/// Result container that specifies the success of the serialization operation and any error messages if one occurred
	/// </summary>
	public struct SerializationResult
	{
		public bool Success { get; private set; }
		public string ErrorMessage { get; private set; }

		public SerializationResult(bool success, string error)
		{
			Success = success;
			ErrorMessage = error;
		}

		public static implicit operator bool(SerializationResult result) => result.Success;
		public static implicit operator string(SerializationResult result) => result.ErrorMessage;
	}
	/// <summary>
	/// /// Result container that specifies the success of the deserialization operation, any error messages if one occurred, and the instance of the deserialized object
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public struct DeserializationResult<T>
	{
		public bool Success { get; private set; }
		public string ErrorMessage { get; private set; }
		public T DeserializedInstance { get; private set; }

		public DeserializationResult(bool success, string error, T instance)
		{
			Success = success;
			ErrorMessage = error;
			DeserializedInstance = instance;
		}

		public static implicit operator bool(DeserializationResult<T> result) => result.Success;
		public static implicit operator string(DeserializationResult<T> result) => result.ErrorMessage;
		public static implicit operator T(DeserializationResult<T> result) => result.DeserializedInstance;
	}

	/// <summary>
	/// TODO
	/// </summary>
	public static class Serializer
	{
		public static SerializationResult Serialize<T>(string filePath, T instance) where T : ISerializable<T>
		{
			SerializationResult result = default;

			using (TextWriter writer = new StreamWriter(filePath, false))
			{
				JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings { Formatting = Formatting.Indented });
				result = instance.Serialize(writer, serializer);
			}

			return result;
		}

		public static DeserializationResult<T> Deserialize<T>(string filePath) where T : IDeserializable<T>, new()
		{
			DeserializationResult<T> result = default;

			using (TextReader reader = new StreamReader(filePath))
			{
				JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings { Formatting = Formatting.Indented });
				result = new T().Deserialize(reader, serializer);
			}

			return result;
		}

		///// <summary>
		///// TODO
		///// </summary>
		///// <typeparam name="T"></typeparam>
		///// <param name="directory"></param>
		///// <param name="filename"></param>
		///// <param name="instance"></param>
		///// <param name="output"></param>
		///// <returns></returns>
		//public static async Task<AsyncSerializationResult> SerializeAsync<T>(string filePath, T instance) where T : IAsyncSerializable<T>
		//{
		//	bool success = false;
		//	string errorMessage = string.Empty;

		//	try
		//	{
		//		await using (TextWriter serializeStream = new StreamWriter(filePath, false))
		//		{
		//			JsonSerializer jsonS = JsonSerializer.Create(new JsonSerializerSettings { Formatting = Formatting.Indented });
		//			await instance.SerializeAsync(serializeStream, jsonS);
		//		}

		//		success = true;
		//	}
		//	catch (Exception e)
		//	{
		//		errorMessage = e.Message;
		//		goto Completed;
		//	}

		//Completed:
		//	return new AsyncSerializationResult(success, errorMessage);
		//}

		///// <summary>
		///// TODO
		///// </summary>
		///// <typeparam name="T"></typeparam>
		///// <param name="filePath"></param>
		///// <returns></returns>
		//public static async Task<AsyncDeserializationResult<T>> DeserializeAsync<T>(string filePath)
		//{
		//	bool result = false;
		//	string error = string.Empty;
		//	T instance = default;

		//	if (!File.Exists(filePath))
		//	{
		//		error = "No file exists at the provided file path or the program does not have permission to access the file at the provided location";
		//		goto Completed;
		//	}

		//	try
		//	{
		//		await Task.Run(() =>
		//		{
		//			using TextReader deserializeStream = new StreamReader(filePath);
		//			JsonSerializer jsonS = JsonSerializer.CreateDefault();
		//			instance = (T)jsonS.Deserialize(deserializeStream, typeof(T));
		//		});

		//		result = true;
		//	}
		//	catch (Exception e)
		//	{
		//		error = e.Message;
		//		goto Completed;
		//	}

		//Completed:
		//	return new AsyncDeserializationResult<T>(result, error, instance);
		//}
	}
}
