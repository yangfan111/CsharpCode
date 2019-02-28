using App.Client.GameModules.Player;
using Entitas;

namespace App.Client.GameModules.SceneObject
{
    public class ClientDebugSystem :AbstractPlayerBackSystem<SceneObjectEntity>
    {
        public ClientDebugSystem(Contexts contexts) : base(contexts)
        {
        }

        protected override IGroup<SceneObjectEntity> GetIGroup(Contexts contexts)
        {
            return contexts.sceneObject.GetGroup(SceneObjectMatcher.AllOf(SceneObjectMatcher.Position,
                SceneObjectMatcher.UnityObject));
        }

       

        protected override void OnPlayBack(SceneObjectEntity entity)
        {
            entity.unityObject.UnityObject.AsGameObject.transform.position = entity.position.Value;
        }
    }
}