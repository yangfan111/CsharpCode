using Core;
using Core.GameModule.Interface;
using Core.Utils;
using Entitas;

namespace App.Server.GameModules.SceneObject
{
    class ServerFreeCastSceneEntityDestroySystem : IGamePlaySystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ServerFreeCastSceneEntityDestroySystem));
        private ISceneObjectEntityFactory _sceneObjectEntityFactory;
        private IGroup<SceneObjectEntity> _castSceneObjectGroup;
        private const int exceptMaxCount = 16;
        private int _clearCount = 0;

        public ServerFreeCastSceneEntityDestroySystem(Contexts contexts)
        {
            _sceneObjectEntityFactory = contexts.session.entityFactoryObject.SceneObjectEntityFactory;
            _castSceneObjectGroup = contexts.sceneObject.GetGroup(SceneObjectMatcher.SimpleCastTarget);
        }

        public void OnGamePlay()
        {
            _clearCount = 0;
            foreach(var removeId in _sceneObjectEntityFactory.FreeCastEntityToDestoryList)
            {
                foreach(var entity in _castSceneObjectGroup)
                {
                    _clearCount++;
                    if(entity.simpleCastTarget.Key == removeId)
                    {
                        entity.isFlagDestroy = true;
                    }
                    if(_clearCount > exceptMaxCount)
                    {
                        Logger.ErrorFormat("too many free cast entities to clear {0} clear {1} total {2}", _clearCount, 
                            _sceneObjectEntityFactory.FreeCastEntityToDestoryList.Count,
                            _castSceneObjectGroup.count);
                    }
                }
            }
            _sceneObjectEntityFactory.FreeCastEntityToDestoryList.Clear();
        }
    }
}
