using Newtonsoft.Json;
using NuggetOfficial.Discord.Serialization.Interfaces;
using System;
using System.IO;

namespace NuggetOfficial.Discord.Serialization
{
    public struct DeserializationResult<T>
    {
        public DeserializationResult(T instance, bool success, string errorMessage)
        {
            Instance = instance;
            Success = success;
            ErrorMessage = errorMessage;
        }

        public T Instance { get; }
        public bool Success { get; }
        public string ErrorMessage { get; }
    }

    public static class Serializer
    {
        private static readonly JsonSerializer serializer = JsonSerializer.CreateDefault();

        public static bool Serialize(this ISerializable instance, string filePath, out string error)
        {
            if (!File.Exists(filePath))
            {
                File.Create(filePath);
            }

            error = string.Empty;

            try
            {
                StreamWriter writer = new StreamWriter(filePath);
                serializer.Serialize(writer, instance);
            }
            catch (Exception e)
            {
                error = e.Message;
                return false;
            }

            return true;
        }

        public static DeserializationResult<T> Deserialize<T>(string filePath, out string error) where T : ISerializable
        {
            if (!File.Exists(filePath))
            {
                error = "no serialized data on disk";
                return default;
            }

            error = string.Empty;

            try
            {
                JsonReader reader = new JsonTextReader(new StreamReader(filePath));
                T instance = serializer.Deserialize<T>(reader);
                return new DeserializationResult<T>(instance, true, error);
            }
            catch (Exception e)
            {
                error = e.Message;
                return new DeserializationResult<T>(default, false, error);
            }
        }
    }
}