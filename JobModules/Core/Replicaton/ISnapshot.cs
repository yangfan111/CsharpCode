using System;
using System.Collections.Generic;
using Core.Components;
using Core.EntityComponent;
using Core.ObjectPool;
using Core.Utils;

namespace Core.Replicaton
{
    public class SnapshotDebugger
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(SnapshotDebugger));
        public static void PrintPosition(string label, ISnapshot s)
        {
            foreach (var entry in s.EntityMap)
            {
                var e = entry.Value;
                var comp = e.GetComponent<PositionComponent>();
                if (comp != null)
                    _logger.InfoFormat("label {3}, snapshot {0}, entity {1}, pos {2}", s.SnapshotSeq, e.EntityKey, comp.Value, label);
            }
        }
    }
    public interface ISnapshot : IRefCounter
    {
        SnapshotHeader Header { get; set; }
        int VehicleSimulationTime { get; set; }
        int ServerTime { get; set; }
        int SnapshotSeq { get; set; }
        int LastUserCmdSeq { get; set; }
        EntityKey Self{ get; set; }
        IGameEntity GetEntity(EntityKey entityKey);
        IGameEntity GetOrCreate(EntityKey entityKey);
        EntityMap EntityMap { get; }
        EntityMap SelfEntityMap { get; }
        EntityMap NonSelfEntityMap { get; }
        EntityMap CompensationEntityMap { get; }
        EntityMap LatestEntityMap { get;  }
        ICollection<IGameEntity> EntityList { get; }
        void ForeachGameEntity(Action<IGameEntity> action);
        void RemoveEntity(EntityKey key);
		void AddEntity(IGameEntity entity);
    }
}