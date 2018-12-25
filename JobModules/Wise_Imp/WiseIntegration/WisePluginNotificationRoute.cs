using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WisePluginNotificationRoute : MonoBehaviour
{
    [HideInInspector]
    public AkInitializer akInteractComponent;
    [HideInInspector]
    AkAudioListener defaultSpatialListener;
    void Start()
    {
        
        akInteractComponent = GetComponent<AkInitializer>();
        if(!defaultSpatialListener)
        {
            GameObject listenerObj = new GameObject("DefaultAudioListenerObj");
            defaultSpatialListener = listenerObj.AddComponent<AkAudioListener>();
            defaultSpatialListener.SetIsDefaultListener(true);
            var camGo = Camera.main.gameObject;
            listenerObj.transform.SetParent(camGo.transform);
            listenerObj.transform.localPosition = Vector3.zero;
        }
        //TODO:引擎状态判断
        Core.Audio.AKAudioEntry.LaunchAppAudio(this);
    }
    //void OnWiseEngineStartupReady(System.Object obj)
    //{

    //    Core.Audio.AKAudioEntry.LaunchAppAudio(obj);
    //}
    //public override void HandleEvent(UnityEngine.GameObject in_gameObject)
    //{
    //    Core.Audio.AKAudioEntry.LaunchAppAudio(gameObject);
    //}
    ////void AddDefaultListener()
    //{

    //}

}
