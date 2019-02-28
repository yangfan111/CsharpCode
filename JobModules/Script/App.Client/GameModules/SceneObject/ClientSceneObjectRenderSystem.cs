using App.Client.CastObjectUtil;
using App.Client.GameModules.Player;
using Entitas;
using UnityEngine;

namespace App.Client.GameModules.SceneObject
{
    public class ClientSceneObjectRenderSystem : AbstractRenderSystem<SceneObjectEntity>
    {
        public ClientSceneObjectRenderSystem(Contexts contexts) : base(contexts)
        {

        }

        protected override bool Filter(SceneObjectEntity entity)
        {
            return !(entity.hasFlagImmutability && entity.flagImmutability.NeedSkipUpdate);
        }

        protected override IGroup<SceneObjectEntity> GetIGroup(Contexts contexts)
        {
            return contexts.sceneObject.GetGroup(SceneObjectMatcher.Position);
        }

        protected override void OnRender(SceneObjectEntity entity)
        {
            if(entity.hasUnityObject && null != entity.unityObject.UnityObject)
            {
                SetPositionWithAnchor(entity.unityObject.UnityObject, entity.position.Value);
            }
            if(entity.hasMultiUnityObject && null != entity.multiUnityObject.FirstAsset)
            {
                SetPositionWithAnchor(entity.multiUnityObject.FirstAsset, entity.position.Value);
            }
        }

        private void SetPositionWithAnchor(GameObject go, Vector3 position)
        {
            go.transform.position = position + BaseGoAssemble.GetGroundAnchorOffset(go);
        }
    }
}
