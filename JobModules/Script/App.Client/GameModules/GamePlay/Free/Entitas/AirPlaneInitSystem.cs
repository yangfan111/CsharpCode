using System.Collections.Generic;
using Entitas;
using Core.Utils;
using Core.GameModule.System;

namespace App.Client.GameModules.GamePlay.Free.Entitas
{
    public class AirPlaneInitSystem : ReactiveEntityInitSystem<FreeMoveEntity>
    {
        private string AirPlane = "plane";
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(AirPlaneInitSystem));
        private IGroup _freeMovegroup;
        public AirPlaneInitSystem(FreeMoveContext freeMoveContext) : base(freeMoveContext)
        {
            //_freeMovegroup = freeMoveContext.GetGroup(FreeMoveMatcher.FreeData);
        }

        public override void SingleExecute(FreeMoveEntity entity)
        {
            if(entity.freeData.Key == AirPlane) 
            {
                entity.isAirPlane = true;
            }
        }

        protected override bool Filter(FreeMoveEntity entity)
        {
            return true;
        }

        protected override ICollector<FreeMoveEntity> GetTrigger(IContext<FreeMoveEntity> context)
        {
            return context.CreateCollector(FreeMoveMatcher.FreeData);
        }
    }
}
