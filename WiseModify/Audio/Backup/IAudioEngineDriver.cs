//using System;
//using System.Collections.Generic;
//using UnityEngine;

//namespace   App.Shared.Audio
//{
//    //插值曲线
//    public enum CurveInterpolation
//    {

//        //AK插值曲线
//        AkCurveInterpolation_Log3 = 0,
//        AkCurveInterpolation_Sine = 1,
//        AkCurveInterpolation_Log1 = 2,
//        AkCurveInterpolation_InvSCurve = 3,
//        AkCurveInterpolation_Linear = 4,
//        AkCurveInterpolation_SCurve = 5,
//        AkCurveInterpolation_Exp1 = 6,
//        AkCurveInterpolation_SineRecip = 7,
//        AkCurveInterpolation_Exp3 = 8,
//        AkCurveInterpolation_LastFadeCurve = 8,
//        AkCurveInterpolation_Constant = 9,

//    }
//    public enum ActionOnEventType
//    {
//        //ak预制事件薄
//        AkActionOnEventType_Stop = 0,
//        AkActionOnEventType_Pause = 1,
//        AkActionOnEventType_Resume = 2,
//        AkActionOnEventType_Break = 3,
//        AkActionOnEventType_ReleaseEnvelope = 4
//    }
//    public abstract class AudioEventCallbackData : UnityEngine.ScriptableObject
//    {
//        public System.Collections.Generic.List<int> callbackFlags = new System.Collections.Generic.List<int>();

//        ////Names of the callback functions.
//        public System.Collections.Generic.List<string> callbackFunc = new System.Collections.Generic.List<string>();

//        ////GameObject that will receive the callback
//        public System.Collections.Generic.List<UnityEngine.GameObject> callbackGameObj =
//            new System.Collections.Generic.List<UnityEngine.GameObject>();

//        ////-TODO:The sum of the flags of all game objects. This is the flag that will be passed to AkSoundEngine.PostEvent
//        public int uFlags = 0;
//    }
//    public interface IAudioEngineDriver
//    {

//        //抛出声音事件
//        void PostEvent(uint eventId, GameObject target);

//        //停止当前事件
//        void StopImmediately(uint eventId, GameObject target);
//        /// <summary>
//        /// 抛出声音事件并处理回调
//        /// </summary>
//        /// <param name="eventId"></param>
//        /// <param name="target"></param>
//        /// <param name="callbackData"></param>
//        /// <param name="param"></param>
//        /// <param name="flags"></param>
//        /// <param name="in_pCookie"></param>
//        void PostEvent_Callback(uint eventId, GameObject target, AudioEventCallbackData callbackData, System.Object param, int flags = -1, object in_pCookie = null);
//        /// <summary>
//        /// 抛出声音事件并处理回调
//        /// </summary>
//        /// <param name="eventId"></param>
//        /// <param name="target"></param>
//        /// <param name="callbackData"></param>
//        /// <param name="flags"></param>
//        /// <param name="in_pCookie"></param>
//        void PostEvent_Callback(uint eventId, GameObject target, AudioEventCallbackData callbackData, int flags = -1, object in_pCookie = null);

//        /// <summary>
//        /// 渐进停止
//        /// </summary>
//        /// <param name="eventId"></param>
//        /// <param name="target"></param>
//        /// <param name="curveInterpolation"></param>
//        /// <param name="transitionDuration"></param>
//        void StopGradually(uint eventId, GameObject target, CurveInterpolation curveInterpolation = CurveInterpolation.AkCurveInterpolation_Linear, float transitionDuration = 1.0f);
//        /// <summary>
//        /// 事件上执行行为
//        /// </summary>
//        /// <param name="eventId"></param>
//        /// <param name="actionOnEvent">事件类型</param>
//        /// <param name="target"></param>
//        /// <param name="curveInterpolation">插值曲线</param>
//        /// <param name="transitionDuration">过渡时长</param>
//        void ExcuteActionOnEvent(uint eventId, ActionOnEventType actionOnEvent, GameObject target, CurveInterpolation curveInterpolation, int transitionDuration);
//        /// <summary>
//        /// 事件上执行行为
//        /// </summary>
//        /// <param name="eventId"></param>
//        /// <param name="target"></param>
//        /// <param name="actionOnEvent"></param>
//        void ExcuteActionOnEvent(uint eventId, GameObject target, ActionOnEventType actionOnEvent);
//        /// <summary>
//        /// 切换声源状态
//        /// </summary>
//        /// <param name="target"></param>
//        /// <param name="groupId"></param>
//        /// <param name="valueId"></param>
//        void SetSourceInObjectSwitch( GameObject target, int groupId, int valueId);


//        int LoadBank(string bankName, AudioBankLoadType loadType, bool loadAsync, AkCallbackManager.BankCallback callback = null);

//        int UnloadBank(string bankName);
//        //切换目标状态
//        bool ChangeTargetGroupState(uint switchGroup, uint switchState, GameObject target);
//        //切换全局状态

//        bool ChangeGlobalGroupState(uint group, uint state);
//        bool ChangeRTPC(uint ID, float value);
//        bool ChangeRTPC(uint ID, float value,GameObject target,int duration);
//    }
//}

