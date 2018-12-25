using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WisePluginNotificationSender : MonoBehaviour
{
    void OnWiseEngineStartupReady(System.Object obj)
    {
        App.Shared.Audio.AKAudioEntry.LaunchAppAudio(obj);
    }
   
}
