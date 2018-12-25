//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System.Reflection;
//namespace   App.Shared.Audio
//{
//    public class AKAudioEngineDriver 
//    {
//        // public const uint PlayingId = AkSoundEngine.AK_INVALID_PLAYING_ID;
//        // public const uint EndEventFlag = (uint)AkCallbackType.AK_EndOfEvent;
        
//        public struct EngineEventCallBackData
//        {
//            public AkEventCallbackData callback;
//            public System.Object param;

//        }
//        #region//事件模块
//        //-事件队列
//        private readonly Dictionary<object, EngineEventCallBackData> callbackEvents = new Dictionary<object, EngineEventCallBackData>();


//        public void PostEvent_Callback(uint eventId, GameObject target, AudioEventCallbackData callbackData, int flags = -1, object in_pCookie = null)
//        {

//            EngineEventCallBackData callbackStruct = new EngineEventCallBackData();
//            callbackStruct.callback = callbackData as AkEventCallbackData;
//            if (in_pCookie == null) in_pCookie = callbackStruct.GetHashCode();
//            //TODO:调用失败时事件移除
//            callbackEvents[in_pCookie] = callbackStruct;
//            uint playingId = AkSoundEngine.PostEvent((uint)eventId, target, flags == -1 ? AudioConst.EndEventFlag : (uint)flags, HandlePostCallback, in_pCookie, 0, null,
//                    AkSoundEngine.AK_INVALID_PLAYING_ID);
//        }
//        public void PostEvent_Callback(uint eventId, GameObject target, AudioEventCallbackData callbackData, System.Object param, int flags = -1, object in_pCookie = null)
//        {

//            EngineEventCallBackData callbackStruct = new EngineEventCallBackData();
//            callbackStruct.callback = callbackData as AkEventCallbackData;
//            callbackStruct.param = param;
//            if (in_pCookie == null) in_pCookie = callbackStruct.GetHashCode();
//            //TODO:调用失败时事件移除
//            callbackEvents[in_pCookie] = callbackStruct;
//            uint playingId = AkSoundEngine.PostEvent((uint)eventId, target, flags == -1 ? AudioConst.EndEventFlag : (uint)flags, HandlePostCallback, in_pCookie, 0, null,
//                    AkSoundEngine.AK_INVALID_PLAYING_ID);
//        }

//        public void PostEvent(uint eventId, GameObject target)
//        {

//            uint playingId = AkSoundEngine.PostEvent((uint)eventId, target);
//        }

//        /// 复用已有的模式驱动事件操作

//        public void StopImmediately(uint eventId, GameObject target)
//        {
//            AkSoundEngine.ExecuteActionOnEvent(eventId, AkActionOnEventType.AkActionOnEventType_Stop, target, (int)AudioConst.DefaultTransitionDuration * 1000,
//                    AudioConst.DefualtCurveInterpolation);
//        }
//        public void StopGradually(uint eventId, GameObject target, CurveInterpolation curveInterpolation = CurveInterpolation.AkCurveInterpolation_Linear, float transitionDuration = 1.0f)
//        {
//            AkSoundEngine.ExecuteActionOnEvent(eventId, AkActionOnEventType.AkActionOnEventType_Stop, target, (int)AudioConst.DefaultTransitionDuration * 1000,
//                   AudioConst.DefualtCurveInterpolation);
//        }

//        public void ExcuteActionOnEvent(uint eventId, ActionOnEventType actionOnEvent, GameObject target, CurveInterpolation curveInterpolation, int transitionDuration)
//        {
//            AkSoundEngine.ExecuteActionOnEvent(eventId, (AkActionOnEventType)actionOnEvent, target, (int)transitionDuration * 1000,
//                    (AkCurveInterpolation)curveInterpolation);
//        }
//        public void ExcuteActionOnEvent(uint eventId, GameObject target, ActionOnEventType actionOnEvent)
//        {
//            AkSoundEngine.ExecuteActionOnEvent(eventId, (AkActionOnEventType)actionOnEvent, target, (int)AudioConst.DefaultTransitionDuration * 1000,
//                    AudioConst.DefualtCurveInterpolation);
//        }
//        //发送事件,事件函数必须是AkEventCallbackMsg类型
//        private void HandlePostCallback(object in_cookie, AkCallbackType in_type, AkCallbackInfo in_info)
//        {
//            EngineEventCallBackData value;
//            if (callbackEvents.TryGetValue(in_cookie, out value))
//            {
//                AkEventCallbackData akcallback = value.callback;
//                for (var i = 0; i < akcallback.callbackFunc.Count; i++)
//                {
//                    if (((int)in_type & akcallback.callbackFlags[i]) != 0 && akcallback.callbackGameObj[i] != null)
//                    {
//                        var callbackInfo = new AkEventCallbackMsg();
//                        callbackInfo.type = in_type;
//                        //   callbackInfo.sender = gameObject;
//                        callbackInfo.info = in_info;

//                        akcallback.callbackGameObj[i].SendMessage(akcallback.callbackFunc[i], callbackInfo);
//                    }
//                    callbackEvents.Remove(in_cookie);
//                }
//            }
//        }
//        #endregion
//        public void SetSourceInObjectSwitch(GameObject target, int groupId, int valueId)
//        {
//            AkSoundEngine.SetSwitch((uint)groupId, (uint)valueId, target);
//        }
//        public void LoadAllRes()
//        {
//            //System.Type bankType = typeof(BANKS);
//            //FieldInfo []fieldInfos =  bankType.GetFields(BindingFlags.Static);
//            //foreach(FieldInfo field in fieldInfos)
//            //{
//            //    field.GetValue(null);
//            //}

//        }
//        public void LoadBankFromPool(int poolID)
//        { 

//        }
  
       
        
//        //指定模式加载bank
//        public int LoadBank(string bankName, AudioBankLoadType loadType, bool loadAsync, AkCallbackManager.BankCallback callback = null)
//        {
//            bool saveDecode = loadType == AudioBankLoadType.DecodeOnLoadAndSave;
//            bool decode = loadType != AudioBankLoadType.Normal;
//            Debug.LogFormat("[AudioDriver]load bank name:{0}", bankName);
//            if (!loadAsync)
//               return (int)AkBankManagerExt.LoadBankRes(bankName, decode, saveDecode);
//            else
//                return (int)AkBankManagerExt.LoadBankResAsync(bankName, callback);

//        }
//        //加载bank资源
//        public void UnloadBank(string bankName)
//        {
//            AkBankManagerExt.UnloadBank(bankName);
//        }
//        //bank全部卸载
//        public void CleanBankTrace(bool enforce = false)
//        {
            
//        //    AkSoundEngine.ClearBanks();
//            AkBankManagerExt.Reset();

//        }
//        //切换目标组音效状态
//        public bool ChangeTargetGroupState(uint switchGroup, uint switchState, GameObject target)

//        {
//            AKRESULT ret = AkSoundEngine.SetSwitch(switchGroup, switchState, target);
//            return AssertAKResult(ret, string.Format("Change target Switchgroup failed=>group:{0},state:{1},target:{2}", switchGroup, switchState, target));

//        }
  
     
//        //切换全局RTPC值
//        public bool ChangeRTPC(uint ID, float value)
//        {
//            AKRESULT ret = AkSoundEngine.SetRTPCValue(ID, value);
//            return AssertAKResult(ret, string.Format("Change RTPC failed=>id:{0},value:{1}", ID, value));
//        }
//        //切换目标RTPC值
//        public bool ChangeRTPC(uint ID, float value,GameObject target,int changeDuration=0)
//        {
//            AKRESULT ret = AkSoundEngine.SetRTPCValue(ID, value,target,changeDuration);
//            return AssertAKResult(ret, string.Format("Change RTPC failed=>id:{0},value:{1},target:{2}", ID, value,target));
//        }

//        //TODO:ResetRtpc????
//        //public void PostState(GameObject target, int groupId, int valueId)
//        //{
//        //    AkSoundEngine.SetState((uint)groupId, (uint)valueId);
//        //}

//    }
//}