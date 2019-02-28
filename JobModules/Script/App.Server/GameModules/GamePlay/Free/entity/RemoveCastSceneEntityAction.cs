using com.wd.free.action;
using com.wd.free.@event;

namespace App.Server.GameModules.GamePlay.Free.entity
{
    [System.Serializable]
    public class RemoveCastSceneEntityAction : AbstractGameAction
    {
        private int key;

        public override void DoAction(IEventArgs args)
        {
            var contexts = args.GameContext;
            var factory = contexts.session.entityFactoryObject.SceneObjectEntityFactory;
            factory.FreeCastEntityToDestoryList.Add(key);
        }
    }
}
