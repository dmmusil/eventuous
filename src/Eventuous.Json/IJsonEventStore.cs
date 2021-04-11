using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eventuous.Json {
    public interface IJsonEventStore {
        Task AppendEvents(string stream, ExpectedStreamVersion expectedVersion, IReadOnlyCollection<JsonStreamEvent> events);
        
        Task<JsonStreamEvent[]> ReadEvents(string stream, StreamReadPosition start, int count);
        
        Task<JsonStreamEvent[]> ReadEventsBackwards(string stream, int count);
        
        Task ReadStream(string stream, StreamReadPosition start, Action<JsonStreamEvent> callback);
    }
}