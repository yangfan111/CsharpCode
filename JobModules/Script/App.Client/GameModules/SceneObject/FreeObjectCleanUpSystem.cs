using Core.GameModule.Interface;
using Entitas;
using UnityEngine;
using UserInputManager.Lib;

namespace App.Client.GameModules.SceneObject
{
    public class FreeObjectCleanUpSystem : IEntityCleanUpSystem
    {
        IGroup<FreeMoveEntity> _group;
        public FreeObjectCleanUpSystem(FreeMoveContext context)
        {
            _group = context.GetGroup(FreeMoveMatcher.AllOf(FreeMoveMatcher.FlagDestroy));
        }

        public void OnEntityCleanUp()
        {
            foreach (var entity in _group)
            {
                if (!entity.hasUnityGameObject)
                {
                    continue;
                }
                var model = entity.unityGameObject.UnityObject.AsGameObject;
                var col = model.transform.Find(SceneObjectConstant.NormalColliderName);
                if(null != col)
                {
                    var target = col.GetComponent<RayCastTarget>();
                    if(null != target)
                    {
                        Object.Destroy(target);
                    }
                    col.GetComponent<Collider>().enabled = false;
                }
            }
        }
    }
}
