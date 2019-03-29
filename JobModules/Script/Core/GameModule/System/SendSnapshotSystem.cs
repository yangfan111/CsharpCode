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
    public class SendSnapshotSystem : IExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(SendSnapshotSystem));
        private IRoom _room;


        public SendSnapshotSystem(IRoom room)
        {
            _room = room;
        }

        public void Execute()
        {
            try
            {
                SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.SendSnapshot);
                _room.SendSnapshot();
            }
            finally
            {
                SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.SendSnapshot);
            }
           
        }
    }
}