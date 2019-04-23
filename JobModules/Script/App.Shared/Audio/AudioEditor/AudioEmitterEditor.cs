#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using App.Shared.Audio;
using Core.Utils;
using UnityEngine;
using UnityEngine.Serialization;
using Utils.Utils;

public enum EAudioEmitType
{
    PlayOnce,
    AutoPlayByInterval,
}

public enum EAudioEventClassify
{
    Action,
    Weapon,
    Pullbolt,
    Magazine,
    Other,
}

public enum EAudioGroupType
{
    SwitchGroup=1,
    StateGroup=2,
}

public enum EAudioTriggerType
{
    None,        //自动触发
    ButtonClick, //点击触发
}

public class AudioConstEditor
{
    public const  float  minPlayInterval = 0.5f;
    public static string EventNone       = "Nothing";

    public const AkCurveInterpolation DefaultExcuteInterpolation =
        AkCurveInterpolation.AkCurveInterpolation_Linear;

    public const int DefaultExcuteTransition = 0;
}

[AttributeUsage(AttributeTargets.Field)]
public class IgnoreRelativeAttribute : Attribute
{
}

[ExecuteInEditMode]
[DisallowMultipleComponent]
public class AudioEmitterEditor : MonoBehaviour
{
    [HideInInspector] public float             EmitTimeInterval;
    private                  float             LastPlayTimeStamp;
    public                   EAudioEmitType    EmitType;
    public                   EAudioTriggerType TriggerType;

    public AudioSourceEditorData SourceData;

    public AudioEventActionEditorData EventActionData;

    public static HashSet<AudioEmitterEditor> Instances = new HashSet<AudioEmitterEditor>();

    //[FormerlySerializedAs("LogEnabled")] 
    public bool EnableEmitterLog;
    public bool LookAtListener;
    public bool StopContinueAudio;


    public GameObject SelfEntity;

    private void Awake()
    {
//        for (int i = 0; i < transform.childCount; i++)
//        {
//
//            var child= transform.GetChild(i);
//            if (child.gameObject.activeSelf)
//            {
//                SelfEntity = child.gameObject;
//                break;
//            }
//        }

        SelfEntity = SelfEntity ?? this.gameObject;
    }

     void OnEnable()
     {
         Instances.Add(this);

     }

     private void OnDisable()
     {
         Instances.Remove(this);
     }

     private GameObject Player;
    // event action template 

    public void SetListener(GameObject player)
    {
        Player = player;
    }


    public void StopAll()
    {
        EventActionData.StopAllExcutedEvents(this);
    }

    public void Stop()
    {
        StopContinueAudio = true;
    }

    public void ReloadXML()
    {
        AudioSourceEditorData.initialize = false;
    }

    public void ReloadWiseBank()
    {
        if (Application.isPlaying)
        {
            AudioEntry.ReloadWiseBank();
        }
    }
//    else
//
//    {
//            var listenerGo = GameObject.Find("AudioListener");
//            if (!listenerGo)
//                return;
//            AkSoundEngineController.OnAudioPluginInitializedInEditor = LaunchAudio;
//            AkSoundEngineController.Instance.Init(this);
//            
//        }
//    }
//    public static void LaunchAudio()
//    {
//        var listenerGo = GameObject.Find("AudioListener");
//        listenerGo.GetComponent<AkAudioListener>().SetIsDefaultListener(true);
//        AudioEntry.LaunchAppAudio(listenerGo);
//    }

    public void PlayEventTemplate()
    {
        EventActionData.PlayEventExcuteTemplate(SourceData.EventCfg.Event, this);
    }

    public void Resume()
    {
        StopContinueAudio = false;
    }

    public void PlayOnce()
    {
        var groups = SourceData.EventSwitchGroups;
        foreach (var grp in groups)
        {
            AKRESULT akresult = AkSoundEngine.SetSwitch(grp.Group, grp.States[grp.selectedIndex], SelfEntity);
            if (EnableEmitterLog)
            {
                DebugUtil.MyLog(string.Format("[Emitter] set akgroup:{0} akstate:{1} akresult:{2}", grp.Group,
                    grp.States[grp.selectedIndex], akresult));
            }
        }

        var currEvt = SourceData.EventCfg.Event;
        //     EventActionData.StopLastExcutedEvent(this);
        AkSoundEngine.PostEvent(currEvt, SelfEntity);
        EventActionData.lastExcutedEvent = currEvt;
        EventActionData.unStopedEvents.Add(currEvt);

        if (EnableEmitterLog)
            DebugUtil.MyLog("[Emitter] post event:" + currEvt);
    }

    float accumulate = 0f;

    private float EmitTimeIntervalReal
    {
        get { return Math.Max(AudioConstEditor.minPlayInterval, EmitTimeInterval); }
    }

    void Update()
    {
        if (Player)
        {
            
        }
        if (LookAtListener && Player != null)
        {
            var lookAtPos = new Vector3(Player.transform.position.x,transform.position.y,Player.transform.position.z); 
            transform.LookAt(lookAtPos);
            //transform.rotation = Quaternion.LookRotation(Player.transform.position, transform.position);
        }


        if (StopContinueAudio)
            return;
        if (EmitType == EAudioEmitType.AutoPlayByInterval)
        {
            accumulate += Time.deltaTime;
            if (EmitTimeIntervalReal > accumulate)
            {
                return;
            }

            PlayOnce();
            accumulate = 0;
            ;
            //LastPlayTimeStamp = Time.realtimeSinceStartup +
            //                    Math.Max(AudioConstEditor.minPlayInterval, EmitTimeInterval) * 1000;
        }
    }
}


#endif