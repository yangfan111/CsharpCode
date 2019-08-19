using App.Client.GameModules.Player;
using Entitas;
using UnityEngine;

namespace App.Client.GameModules.SceneObject
{
    public class ClientSceneObjectRenderSystem : AbstractRenderSystem<SceneObjectEntity>
    {
        private Contexts _contexts;

        public ClientSceneObjectRenderSystem(Contexts contexts) : base(contexts)
        {
            _contexts = contexts;
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
            if (entity.hasUnityObject && null != entity.unityObject.UnityObject)
            {
                SetPositionWithAnchor(entity.unityObject.UnityObject, entity);
            }

            if (entity.hasMultiUnityObject && null != entity.multiUnityObject.FirstAsset)
            {
                SetPositionWithAnchor(entity.multiUnityObject.FirstAsset, entity);
            }
        }

        private void SetPositionWithAnchor(GameObject go, SceneObjectEntity entity)
        {
            if (entity.hasFlagImmutability)
                entity.flagImmutability.HasUsed();
            Bounds bounds = entity.position.Bounds;
            Vector3 delta = Vector3.zero;
            if (entity.position.Bounds != null && bounds.size != Vector3.zero)
            {
                if (entity.position.ModelRotate)
                {
                    delta = new Vector3(0, bounds.size.x / 2 + bounds.center.x, 0);
                }
                else
                {
                    delta = new Vector3(0, bounds.size.y / 2 - bounds.center.y, 0);
                }

                if (entity.hasSize) delta = delta * entity.size.Value;
            }
            go.transform.position = entity.position.Value + delta;
        }
    }
}
