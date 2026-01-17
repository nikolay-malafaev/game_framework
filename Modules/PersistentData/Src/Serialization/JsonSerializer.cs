using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace GameFramework.PersistentData
{
    public class JsonSerializer : ISerializer
    {
        private static readonly Encoding Utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        private readonly JsonSerializerSettings _settings;
        
        public static JsonSerializerSettings Default
        {
            get
            {
                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore,
                    DateParseHandling = DateParseHandling.DateTime,
                    DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    },
                    Formatting = Formatting.None
                };
                settings.Converters.Add(new StringEnumConverter());
                return settings;
            }
        }
        
        public JsonSerializer(JsonSerializerSettings settings = null)
        {
            _settings = settings ?? Default;
        }

        public byte[] Serialize<T>(T value)
        {
            string json = JsonConvert.SerializeObject(value, typeof(T), _settings);
            if (string.IsNullOrEmpty(json))
            {
                return Array.Empty<byte>();
            }
            return Utf8NoBom.GetBytes(json);
        }

        public T Deserialize<T>(ReadOnlyMemory<byte> data)
        {
            string json = Utf8NoBom.GetString(data.ToArray());
            T obj = JsonConvert.DeserializeObject<T>(json, _settings);
            return obj;
        }
    }
}