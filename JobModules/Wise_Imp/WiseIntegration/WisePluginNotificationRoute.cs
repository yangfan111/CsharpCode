using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WisePluginNotificationRoute : MonoBehaviour
{
    [HideInInspector]
    public AkInitializer akInteractComponent;
    [HideInInspector]
    public AkAudioListener defaultSpatialListener;
    public Transform defaultListenerTrans;

    void Start()
    {
        
        akInteractComponent = GetComponent<AkInitializer>();
        if(!defaultSpatialListener)
        {
            GameObject listenerObj = new GameObject("DefaultAudioListenerObj");
            defaultSpatialListener = listenerObj.AddComponent<AkAudioListener>();
            defaultSpatialListener.SetIsDefaultListener(true);
            defaultListenerTrans = defaultSpatialListener.transform;
            if (Camera.main)
            {
                defaultListenerTrans.SetParent(Camera.main.transform);
                defaultListenerTrans.localPosition = Vector3.zero;
            }
            else
            {
                defaultListenerTrans.position = Vector3.zero;
            }
         
        }
        //TODO:引擎状态判断
        Core.Audio.AKAudioEntry.LaunchAppAudio(this);
    }
    private void LateUpdate()
    {
        if (defaultListenerTrans.parent) return;
        if (Camera.main)
        {
            defaultListenerTrans.SetParent(Camera.main.transform);
            defaultListenerTrans.localPosition = Vector3.zero;
        }
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
