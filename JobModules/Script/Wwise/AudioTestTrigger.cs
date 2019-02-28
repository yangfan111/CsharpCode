using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
public class AudioTestTrigger : MonoBehaviour
{
    public bool PlayOnce = false;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!AkSoundEngine.IsInitialized())
            return;
        if (!PlayOnce)
            return;
        AkBankManager.LoadBank("Weapon_Footstep",true,true);
       uint playerId = AkSoundEngine.PostEvent("Gun_magazine_AWM", gameObject);
        Debug.Log(playerId);
        PlayOnce = false;
    }
}
#endif