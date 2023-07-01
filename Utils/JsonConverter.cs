using System;
using System.IO;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Hikaria.WeaponDataManager.Utils
{
    public class JsonConverter
    {
        public JsonConverter()
        {
            Options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                IncludeFields = true,
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                WriteIndented = true
            };
            Options.Converters.Add(new JsonStringEnumConverter());
        }

        public JsonConverter(bool allowTrailingCommas = true, bool includeFields = true, bool propertyNameCaseInsensitive = true, bool writeIndented = true)
        {
            Options = new JsonSerializerOptions
            {
                AllowTrailingCommas = allowTrailingCommas,
                IncludeFields = includeFields,
                PropertyNameCaseInsensitive = propertyNameCaseInsensitive,
                ReadCommentHandling = JsonCommentHandling.Skip,
                WriteIndented = writeIndented
            };
            Options.Converters.Add(new JsonStringEnumConverter());
        }

        public void TryRead<T>(string path, out T output) where T : new()
        {
            string text;
            if (File.Exists(path))
            {
                text = File.ReadAllText(path);
                output = Deserialize<T>(text);
                return;
            }
            output = new T();
            text = Serialize(output);
            File.WriteAllText(path, text);
        }

        public string Serialize(object value)
        {
            return JsonSerializer.Serialize(value, Options);
        }

        public T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, Options);
        }

        public JsonSerializerOptions Options;
    }
}
