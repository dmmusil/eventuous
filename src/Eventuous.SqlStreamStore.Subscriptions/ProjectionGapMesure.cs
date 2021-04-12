using System.Collections.Generic;

namespace Eventuous.SqlStreamStore.Subscriptions {
    public class ProjectionGapMeasure {
        readonly Dictionary<string, long> _gaps = new();

        public void PutGap(string checkpointId, long gap) {
            _gaps[checkpointId] = gap;
        }

        public long GetGap(string checkpointId) => _gaps[checkpointId];
    }
}
