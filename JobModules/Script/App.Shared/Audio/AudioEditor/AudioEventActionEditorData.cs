#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Core.Utils;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class AudioEventActionEditorData
{
    public AkCurveInterpolation Interpolation = AudioConstEditor.DefaultExcuteInterpolation;
    public float Transition = AudioConstEditor.DefaultExcuteTransition;
    public AkActionOnEventType ActionOnEventType;
    [NonSerialized] public string lastExcutedEvent;
    [NonSerialized] public HashSet<string> unStopedEvents = new HashSet<string>();

    public void PlayEventExcuteTemplate(string eventName, AudioEmitterEditor audioEmitterEditor)
    {
        var result = AkSoundEngine.ExecuteActionOnEvent(eventName, ActionOnEventType, audioEmitterEditor.gameObject,
             AudioConstEditor.DefaultExcuteTransition * 1000,
             AudioConstEditor.DefaultExcuteInterpolation);
        if (audioEmitterEditor.EnableEmitterLog)
        {
            DebugUtil.MyLog("[Emitter] PlayExcuteTemplate result :" + result);
        }

        switch (ActionOnEventType)
        {
            case AkActionOnEventType.AkActionOnEventType_Resume:
                lastExcutedEvent = eventName;
                break;
            case AkActionOnEventType.AkActionOnEventType_Stop:
                unStopedEvents.Remove(eventName);
                break;
        }

    }
    public void StopAllExcutedEvents(AudioEmitterEditor instance)
    {
        foreach (var evt in unStopedEvents)
        {
            var result = AkSoundEngine.ExecuteActionOnEvent(evt, AkActionOnEventType.AkActionOnEventType_Stop, instance.gameObject,
           AudioConstEditor.DefaultExcuteTransition * 1000,
           AudioConstEditor.DefaultExcuteInterpolation);
            if (result != AKRESULT.AK_Success && instance.EnableEmitterLog)
            {
                DebugUtil.MyLog("[Emitter] PlayExcuteTemplate result :" + result);
            }
        }
        unStopedEvents.Clear();
    }

    public void StopLastExcutedEvent(AudioEmitterEditor instance)
    {
        if (string.IsNullOrEmpty(lastExcutedEvent))
            return;
        var result = AkSoundEngine.ExecuteActionOnEvent(lastExcutedEvent, AkActionOnEventType.AkActionOnEventType_Stop, instance.gameObject,
             AudioConstEditor.DefaultExcuteTransition * 1000,
             AudioConstEditor.DefaultExcuteInterpolation);
        if (result != AKRESULT.AK_Success && instance.EnableEmitterLog)
        {
            DebugUtil.MyLog("[Emitter] PlayExcuteTemplate result :" + result);
        }

        lastExcutedEvent = null;

    }
}
#endif