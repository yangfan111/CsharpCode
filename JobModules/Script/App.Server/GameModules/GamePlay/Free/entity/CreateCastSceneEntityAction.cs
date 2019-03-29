using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.map.position;

namespace App.Server.GameModules.GamePlay.Free.entity
{
    [System.Serializable]
    class CreateCastSceneEntityAction : AbstractGameAction
    {
        private IPosSelector pos;
        private int key;
        private float size;
        private string tip;
        private int castflag;

        public override void DoAction(IEventArgs args)
        {
            var contexts = args.GameContext;
            var p = pos.Select(args);
            var entity = contexts.session.entityFactoryObject.SceneObjectEntityFactory.CreateCastEntity(new UnityEngine.Vector3(p.GetX(), p.GetY(), p.GetZ()),
                size, key, tip);
            var sceneObjectEntity = entity as SceneObjectEntity;

            if(null != sceneObjectEntity && castflag != 0)
            {
                sceneObjectEntity.AddCastFlag(castflag);
            }
        }
    }
}
