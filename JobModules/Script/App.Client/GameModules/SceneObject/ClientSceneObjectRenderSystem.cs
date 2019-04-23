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
//#if UNITY_EDITOR
//            if (entity.hasAudioTestEmitter)
//            {
//                var go = entity.unityObject.UnityObject.AsGameObject;
//                var target = go.transform.Find("I004_LOD0");
//                target.transform.SetParent(null);
//                entity.audioTestEmitter.Self = target.gameObject;
//                var emitter = go.GetComponent<AudioEmitterEditor>();
//                if(!emitter)
//                    emitter = go.AddComponent<AudioEmitterEditor>();
//                go.name = "WS-AudioEmitter";
//                go.transform.scal = Vector3.one * 3f;
//                go.transform.position = entity.position.Value;
//                emitter.SetListener(entity.audioTestEmitter.P1);
//                SetPositionWithAnchor(entity.unityObject.UnityObject, new Vector3(-1,-1,-1));
//
//            }
//            #endif
        }

        private void SetPositionWithAnchor(GameObject go, Vector3 position)
        {
            go.transform.position = position + BaseGoAssemble.GetGroundAnchorOffset(go);
        }
    }
}
