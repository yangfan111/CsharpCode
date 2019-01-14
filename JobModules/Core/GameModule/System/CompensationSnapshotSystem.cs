using System.Collections.Generic;
using App.Shared;
using Core.EntitasAdpater;
using Core.Network;
using Core.Replicaton;
using Core.Room;
using Core.SpatialPartition;
using Core.Utils;
using Entitas;
using Utils.Singleton;

namespace App.Client.ClientGameModules.System
{
    public class CompensationSnapshotSystem : IExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(CompensationSnapshotSystem));
        private IRoom _room;


        public CompensationSnapshotSystem(IRoom room)
        {
            _room = room;
        }

        public void Execute()
        {
            try
            {
                SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.CompensationSnapshot);
                _room.CompensationSnapshot();
            }
            finally
            {
                SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.CompensationSnapshot);
            }
           
        }
    }
}