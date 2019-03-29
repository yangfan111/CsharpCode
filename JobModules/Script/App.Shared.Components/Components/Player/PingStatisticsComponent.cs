using Core.Components;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.UpdateLatest;
using Entitas.CodeGeneration.Attributes;

namespace App.Shared.Components.Player
{
    [Player]
    
    public class PingStatisticsComponent : IUpdateComponent
    {
        [NetworkProperty] [DontInitilize] public short Ping;
        [NetworkProperty] [DontInitilize] public short Fps5;

        public void CopyFrom(object rightComponent)
        {
            PingStatisticsComponent r = rightComponent as PingStatisticsComponent;
            Ping = r.Ping;
            Fps5 = r.Fps5;
        }

        public int GetComponentId()
        {
            return (int) EComponentIds.Statistics;
        }
    }
}