namespace Eventuous.Json {
    public interface IJsonEventSerializer {
        object? Deserialize(string json, string eventType);

        string Serialize(object evt);
    }
}