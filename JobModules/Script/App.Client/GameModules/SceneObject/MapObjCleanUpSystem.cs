using App.Shared.SceneTriggerObject;
using App.Shared.Util;
using Core.GameModule.System;
using Entitas;
using Utils.Singleton;

namespace App.Client.GameModules.SceneObject
{
    public class MapObjCleanUpSystem:ReactiveEntityCleanUpSystem<MapObjectEntity>
    {
        private Contexts _contexts;
        
        public MapObjCleanUpSystem(Contexts context) : base(context.mapObject)
        {
            _contexts = context;
        }

        protected override ICollector<MapObjectEntity> GetTrigger(IContext<MapObjectEntity> context)
        {
            return context.CreateCollector(MapObjectMatcher.FlagDestroy);
        }

        protected override bool Filter(MapObjectEntity entity)
        {
            return entity.hasTriggerObjectId;
        }

        public override void SingleExecute(MapObjectEntity entity)
        {
            MapObjectUtility.DeleteRecord(entity.triggerObjectId.Id);
        }
    }
}