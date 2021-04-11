using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eventuous.Json;
using SqlStreamStore.Infrastructure;
using SqlStreamStore.Streams;

namespace Eventuous.SqlStreamStore {
    public class SssEventStore : IJsonEventStore {
        private readonly StreamStoreBase _eventStore;

        public SssEventStore(StreamStoreBase eventStore) => _eventStore = eventStore;

        public async Task AppendEvents(string stream, ExpectedStreamVersion expectedVersion,
            IReadOnlyCollection<JsonStreamEvent> events) {
            var proposedEvents = events.Select(ToEventData).ToArray();

            var resultTask = expectedVersion == ExpectedStreamVersion.NoStream
                ? _eventStore.AppendToStream(stream, ExpectedVersion.NoStream, proposedEvents)
                : _eventStore.AppendToStream(stream, (int) expectedVersion.Value, proposedEvents);

            await resultTask;

            static NewStreamMessage ToEventData(JsonStreamEvent e) {
                var (eventType, data, metadata) = e;
                return new NewStreamMessage(Guid.NewGuid(), eventType, data, metadata);
            }
        }

        public async Task<JsonStreamEvent[]> ReadEvents(string stream, StreamReadPosition start, int count) {
            var page = await _eventStore.ReadStreamForwards(
                stream, 
                (int) start.Value, 
                count, 
                prefetchJsonData: true);

            return page.Messages.Select(ToJsonStreamEvent).ToArray();
        }

        public async Task<JsonStreamEvent[]> ReadEventsBackwards(string stream, int count) {
            var page = await _eventStore.ReadStreamBackwards(
                stream, 
                StreamVersion.End,
                count, 
                prefetchJsonData: true);

            return page.Messages.Select(ToJsonStreamEvent).ToArray();
        }

        public async Task ReadStream(string stream, StreamReadPosition start, Action<JsonStreamEvent> callback) {
            var page = await _eventStore.ReadStreamForwards(
                stream, 
                (int) start.Value, 
                100, 
                prefetchJsonData: true);
            while (true) {
                foreach (var streamMessage in page.Messages) {
                    callback(ToJsonStreamEvent(streamMessage));
                }
                if (page.IsEnd) break;
                page = await page.ReadNext();
            }
        }

        private static JsonStreamEvent ToJsonStreamEvent(StreamMessage streamMessage) =>
            new(streamMessage.Type, 
                // data was prefetched so await is not needed
                streamMessage.GetJsonData().Result,
                streamMessage.JsonMetadata);
    }
}