using System.Collections.Generic;
using UnityEngine;
namespace   App.Shared.Audio
{

    public static class AudioTriggerArgsFactory
    {
        public static AudioTriggerArgs CreateDefault(AudioTriggerEventType evtType, UnityEngine.GameObject target = null)
        {
            return new AudioTriggerArgs(target, evtType);
        }
        public static AudioTriggerArgs CreateSceneArgs(string sceneName)
        {
            return new AudioTriggerSceneLoadArgs(sceneName);
        }
        public static AudioTriggerArgs CreateRTPCArgs(AudioTriggerEventType evtType, float value, UnityEngine.GameObject target = null, int transition = 0)
        {
            return new AudioTriggerRTPCArgs(evtType, value, target, transition);
        }
   

    }
    public class AudioTriggerGroupArgs : AudioTriggerArgs
    {
        public int StateId { get; private set; }
        public AudioTriggerGroupArgs(AudioTriggerEventType evtType, int stateId, UnityEngine.GameObject target) : base(target, evtType)
        {
            StateId = stateId;
        }
    }
    public class AudioTriggerRTPCArgs : AudioTriggerArgs
    {
        public float Value { get; private set; }
        public bool IsGlobal { get; private set; }
        public int Transition;
        public AudioTriggerRTPCArgs(AudioTriggerEventType evtType, float value,
            UnityEngine.GameObject target, int transition) : base(target, evtType)
        {
            IsGlobal = (target == null);
            Value = value;
            Transition = transition;
        }
        public override bool IsValid()
        {
            return !IsGlobal || (IsGlobal && Target);
        }
    }
    public class AudioTriggerSceneLoadArgs : AudioTriggerArgs
    {
        public string SceneName { get; private set; }

        public AudioTriggerSceneLoadArgs(string sceneName)
        {
            EventType = AudioTriggerEventType.SceneLoad;
            this.SceneName = sceneName;
        }

    }
    public class AudioTriggerEvts_VaryListenerTarget:AudioEventArgs
    {
        public UnityEngine.GameObject Target { get; protected set; }
        public UnityEngine.Vector3 posOffset { get; protected set; }
        public AudioTriggerEvts_VaryListenerTarget(UnityEngine.GameObject target,UnityEngine.Vector3 offset)
        {

        }
    }
    public class AudioTriggerArgs : AudioEventArgs
    {
        public UnityEngine.GameObject Target { get; protected set; }
        // protected AudioTriggerEventType eventType;
        public AudioTriggerEventType EventType { get; protected set; }

        public AudioTriggerArgs()
        {

        }
        public virtual bool IsValid()
        {
            return true;
        }
        public AudioTriggerArgs(UnityEngine.GameObject target, AudioTriggerEventType evtType)
        {
            Target = target;
            EventType = evtType;
        }
    }
    public abstract class AudioEventArgs : System.EventArgs
    {
    }

}
