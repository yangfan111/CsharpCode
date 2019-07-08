using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public enum AudioBank_LoadMode
{
    Sync,
    Async,
    Prepare
}

public enum AudioBank_LoadAction
{
    DecodeOnLoad,
    DecodeOnLoadAndSave,
    Normal
}

public enum AudioSourceType
{
    Hall,
    Battle
}

public interface IAudioProjectManager
{
    void OnWiseInitializedSucess();
    bool prepareReady { get;}
    HashSet<GameObject> AmbWaitQueue { get; }
    /// <summary>
    /// 大厅端
    /// </summary>
    AkAudioListener listener { get; }
    AkSpatialAudioListener spatialListener { get; }
    /// <summary>
    /// 战斗端
    /// </summary>
    AudioBattleListener battleListener { get; }

    void GoBattle();
    void HallBack();
    void PlayAmbient();
}

public abstract class AudioProjectManager<T> where T : AudioProjectManager<T>, IAudioProjectManager
{
    private static T instance;

    protected GameObject AudioMakerObj { get; private set; }

    public AkAudioListener listener { get; protected set; }
    public AkSpatialAudioListener spatialListener { get; protected set; }

    public virtual void GoBattle()
    {
    }

    public virtual void HallBack()
    {
    }
    public HashSet<GameObject> AmbWaitQueue
    {
        get { return ambWaitQueue; }
    }
    
    private HashSet<GameObject> ambWaitQueue = new HashSet<GameObject>();
    public AudioBattleListener battleListener { get; protected set; }
    public bool                prepareReady   { get; protected set; }

    public AudioSourceType SourceType { get;}

    private HashSet<GameObject> ambToPlayList = new HashSet<GameObject>();
    public HashSet<GameObject> AmbToPlayList
    {
        get { return ambToPlayList; }
    }

    public static void Allocate(GameObject audioMaker)
    {
        if (instance == null)
        {
            instance                             = Activator.CreateInstance<T>();
            AkSoundEngineController.AudioManager = instance;
            instance.AudioMakerObj = audioMaker;
        }
      
    }

    protected abstract void ProjectInitialize();

    public void OnWiseInitializedSucess()
    {
        prepareReady = true;
        ProjectInitialize();
        battleListener = new AudioBattleListener(listener, spatialListener);
        Debug.Log("Customize Wise Initialized Sucess");
    }

    public void PlayAmbient()
    {
        if (ambToPlayList.Count > 0)
        {
            foreach (var value in ambToPlayList)
            {
                value.SendMessage("CallEnable",value,SendMessageOptions.DontRequireReceiver);
            }
            ambToPlayList.Clear();
        }
    }
    protected void InitBattleListner()
    {
        var listenerGo = new GameObject("AKAudioListener");
        listener        = listenerGo.AddComponent<AkAudioListener>();
        spatialListener = listenerGo.AddComponent<AkSpatialAudioListener>();
        listener.SetIsDefaultListener(true);
    }
}