using System.Collections.Generic;
using System.Threading;
using Eventuous.Json;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using SqlStreamStore;
using SqlStreamStore.Infrastructure;
using SqlStreamStore.Streams;

namespace Eventuous.SqlStreamStore.Subscriptions {
    [PublicAPI]
    public class AllStreamSubscriptionService : SubscriptionService {
        protected AllStreamSubscriptionService(
            StreamStoreBase            streamStreamStoreClient,
            string                     subscriptionName,
            ICheckpointStore           checkpointStore,
            IJsonEventSerializer       eventSerializer,
            IEnumerable<IEventHandler> projections,
            ILoggerFactory?            loggerFactory = null,
            ProjectionGapMeasure?      measure       = null
        ) : base(
            streamStreamStoreClient,
            subscriptionName,
            checkpointStore,
            eventSerializer,
            projections,
            loggerFactory,
            measure
        )
        {}
        protected override long GetPosition(StreamMessage resolvedEvent)
            => resolvedEvent.Position;

        protected override IAllStreamSubscription Subscribe(
            Checkpoint checkpoint, CancellationToken cancellationToken
        )
            => StreamStoreClient.SubscribeToAll(
                checkpoint.Position,
                Handler,
                Dropped
            );

    }
}