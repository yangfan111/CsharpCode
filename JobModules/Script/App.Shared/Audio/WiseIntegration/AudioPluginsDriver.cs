using UnityEngine;
[RequireComponent(typeof(AkInitializer))]
public class AudioPluginsDriver: MonoBehaviour
{
    [HideInInspector]
    public AkInitializer akInteractComponent;
    [HideInInspector]
    public AkAudioListener defaultSpatialListener;
    [HideInInspector]
    public Transform defaultListenerTrans;

    void Start()
    {

        akInteractComponent = GetComponent<AkInitializer>();
        InitWiseDefaultListener();
        //TODO:引擎状态判断
        App.Shared.Audio.AKAudioEntry.LaunchAppAudio(this);
    }
    void InitWiseDefaultListener()
    {
        if (!defaultSpatialListener)
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
    }
    void LateUpdate()
    {
        if (!defaultListenerTrans)
        {
            InitWiseDefaultListener();
        }
        if (defaultListenerTrans.parent) return;
        if (Camera.main)
        {
            defaultListenerTrans.SetParent(Camera.main.transform);
            defaultListenerTrans.localPosition = Vector3.zero;
        }
    }
    //void OnWiseEngineStartupReady(System.Object obj)
    //{

    //    App.Shared.Audio.AKAudioEntry.LaunchAppAudio(obj);
    //}
    //public override void HandleEvent(UnityEngine.GameObject in_gameObject)
    //{
    //    App.Shared.Audio.AKAudioEntry.LaunchAppAudio(gameObject);
    //}
    ////void AddDefaultListener()
    //{

    //}

}
