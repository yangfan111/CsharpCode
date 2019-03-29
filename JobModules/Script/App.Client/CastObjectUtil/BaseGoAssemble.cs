using App.Client.GameModules.SceneObject;
using Core.Utils;
using UnityEngine;
using UserInputManager.Lib;
using UserInputManager.Utility;

namespace App.Client.CastObjectUtil
{
    public static class BaseGoAssemble
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(BaseGoAssemble));
        public static RayCastTarget Assemble(GameObject model, Vector3 position, string name)
        {
            if(null == model)
            {
                Logger.ErrorFormat("model is null !");
                return null;
            }
            model.transform.position = position + GetGroundAnchorOffset(model);
            var colTrans = model.transform.Find(SceneObjectConstant.NormalColliderName);
            if(null == colTrans)
            {
                Logger.ErrorFormat("no normal collider find in {0}", model.name);
                var colGo = new GameObject(SceneObjectConstant.NormalColliderName);
                colTrans = colGo.transform;
                colTrans.parent = model.transform;
                colTrans.localPosition = Vector3.zero;
                colTrans.localEulerAngles = Vector3.zero;
                colTrans.localScale = Vector3.one;
                var col = colGo.AddComponent<BoxCollider>();
                col.isTrigger = true;
                col.size = Vector3.one;
            }
            var normalCollider = colTrans.GetComponent<BoxCollider>();
            if(null == normalCollider)
            {
                Logger.ErrorFormat("no box attached to collider go");
                return null;
            }

            RayCastTarget target = RayCastTargetUtil.AddRayCastTarget(normalCollider.gameObject);
            
            normalCollider.gameObject.layer = UnityLayerManager.GetLayerIndex(EUnityLayerName.UserInputRaycast);
            normalCollider.enabled = true;
            return target;
        }

        public static Vector3 GetGroundAnchorOffset(GameObject model)
        {
            var groundNode = model.transform.Find(SceneObjectConstant.GroundAnchorName);
            if(null == groundNode)
            {
                return Vector3.zero;
            }
            else
            {
                return model.transform.position - groundNode.transform.position;
            }
        }
    }
}