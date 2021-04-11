using System.Text.Json;
using JetBrains.Annotations;

namespace Eventuous.Json {
    [PublicAPI]
    public class JsonEventSerializer : IJsonEventSerializer {
        public static readonly IJsonEventSerializer Instance =
            new JsonEventSerializer(new JsonSerializerOptions(JsonSerializerDefaults.Web));

        readonly JsonSerializerOptions _options;

        public JsonEventSerializer(JsonSerializerOptions options) => _options = options;

        public object? Deserialize(string data, string eventType)
            => !TypeMap.TryGetType(eventType, out var dataType)
                ? null!
                : JsonSerializer.Deserialize(data, dataType!, _options);

        public string Serialize(object evt) => JsonSerializer.Serialize(evt, _options);
    }
}