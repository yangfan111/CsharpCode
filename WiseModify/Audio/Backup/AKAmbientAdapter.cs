
//using UnityEngine;

//namespace   App.Shared.Audio
//{
//    public class AKAmbientAdapter
//    {
//        public GameObject HanlderObject
//        {
//            get {
//                if (!ambient || ambient.useOtherObject) {
//                    return null;
//                }
//                return ambient.gameObject;
//            }
//        }
//        private AkAmbient ambient;
//        //发布方式
//        public AudioAmbientEmitterType EmitterType
//        {
//            get
//            {

//                if (ambient.enableActionOnEvent) return AudioAmbientEmitterType.ActionOnCustomEventType;
//                return AudioAmbientEmitterType.UseCallback;
//            }

//        }
//        public AKAmbientAdapter(AkAmbient am)
//        {
//            ambient = am;

//        }
//        /// 设置触发对象，对象实体从audioTrigger中传入
//        public bool EmitSelf
//        {
//            get { return !ambient.useOtherObject; }
//            set
//            {
//                if (ambient.useOtherObject == value)
//                {
//                    Debug.Log("[ak event]useOtherObject changed");
//                    ambient.useOtherObject = !value;
//                }
//            }
//        }
//        //对应资源Event的 WwiseID.cs file
//        public uint AudioAssetEventId { get { return (uint)ambient.eventID; } }
//        public uint AudioEnginePlayerId { get { return ambient.playingId; } }


//        /// 应用Ambient内容
//        public void ReBind(AkAmbient am)
//        {
//            if (ambient != null)
//                Debug.Log("[ak event]bind instance changed");
//            ambient = am;
//        }
//        ///指定回调
//        ///实际执行AkSoundEngine.ExecuteActionOnEvent API
//        public void DesignateAmbientActionOnEventType(AkActionOnEventType actionOnEventType, float actionTransition,
//            AkCurveInterpolation actionCurve = AkCurveInterpolation.AkCurveInterpolation_Linear)
//        {
//            if (!ambient.enableActionOnEvent)
//            {
//                Debug.Log("[ak event]enable action on event");
//                ambient.enableActionOnEvent = true;
//            }
//            ambient.curveInterpolation = actionCurve;
//            ambient.transitionDuration = actionTransition;

//        }
//        public void DesignateAmbientActionOnEventType(AkActionOnEventType actionOnEventType)
//        {
//            DesignateAmbientActionOnEventType(actionOnEventType, 0.0f);
//        }
//        /// 指定回调数据构建
//        ///  实际执行AkSoundEngine.PostEvent
//        /// 必须是void FunctionName(AkEventCallbackMsg in_info)。
//        public void DesignateCallback(AkEventCallbackData akCallbackData)
//        {
//            if (ambient.enableActionOnEvent)
//            {
//                ambient.enableActionOnEvent = false;
//                Debug.Log("[ak event]callback data change");
//            }
//            ambient.m_callbackData = akCallbackData;

//        }
//        /// <summary>
//        ///-音效引擎采样数据
//        /// </summary>
//        public void DefineAudioEngineSampleData()
//        {

//        }
//    }
//    ///// TODO:重定向目标gameobject，只针对Collider生效
//    ///// <param name="tarObject"></param>
//    //public void RedirectCollisionObject(GameObject tarObject)
//    //{

//    //}
//}
