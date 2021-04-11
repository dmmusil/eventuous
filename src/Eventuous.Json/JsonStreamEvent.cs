namespace Eventuous.Json {
    public record JsonStreamEvent(string EventType, string Data, string? Metadata = null);
}