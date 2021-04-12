// using System.Collections.Generic;
// using System.Threading;
// using System.Threading.Tasks;
// using Eventuous.EventStoreDB.Subscriptions;
// using JetBrains.Annotations;
// using Microsoft.Extensions.Logging;
// using SqlStreamStore.Subscriptions;
//
// namespace Eventuous.SqlStreamStore.Subscriptions {
//     [PublicAPI]
//     public class StreamSubscriptionService : SubscriptionService {
//         readonly string _streamName;
//
//         public StreamSubscriptionService(
//             EventStoreClient           streamStreamStoreClient,
//             string                     streamName,
//             string                     subscriptionName,
//             ICheckpointStore           checkpointStore,
//             IEventSerializer           eventSerializer,
//             IEnumerable<IEventHandler> projections,
//             ILoggerFactory?            loggerFactory = null,
//             ProjectionGapMeasure?      measure       = null
//         ) : base(
//             streamStreamStoreClient,
//             subscriptionName,
//             checkpointStore,
//             eventSerializer,
//             projections,
//             loggerFactory,
//             measure
//         )
//             => _streamName = streamName;
//
//         protected override ulong? GetPosition(ResolvedEvent resolvedEvent)
//             => resolvedEvent.Event.EventNumber.ToUInt64();
//
//         protected override Task<StreamSubscription> Subscribe(
//             Checkpoint checkpoint, CancellationToken cancellationToken
//         )
//             => checkpoint.Position == null
//                 ? StreamStoreClient.SubscribeToStreamAsync(
//                     _streamName,
//                     Handler,
//                     true,
//                     Dropped,
//                     cancellationToken: cancellationToken
//                 )
//                 : StreamStoreClient.SubscribeToStreamAsync(
//                     _streamName,
//                     StreamPosition.FromInt64((long) checkpoint.Position),
//                     Handler,
//                     true,
//                     Dropped,
//                     cancellationToken: cancellationToken
//                 );
//     }
// }