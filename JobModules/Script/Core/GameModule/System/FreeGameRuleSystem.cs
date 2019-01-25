using System;
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
    public class FreeGameRuleSystem : IExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(FreeGameRuleSystem));
        private IRoom _room;


        public FreeGameRuleSystem(IRoom room)
        {
            _room = room;
        }

        public void Execute()
        {
            try
            {
                SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.FreeRule);
                _room.RunFreeGameRule();
            }
            catch (Exception e)
            {
                _logger.ErrorFormat("FreeGameRuleSystem error executing {0}", e);
            }
            finally
            {
                SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.FreeRule);
            }
           
        }
    }
}