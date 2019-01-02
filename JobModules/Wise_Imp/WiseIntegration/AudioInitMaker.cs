using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[UnityEngine.ExecuteInEditMode]
[UnityEngine.DisallowMultipleComponent]
public class AudioInitMaker : MonoBehaviour
{
    private GameObject wiseObject;
    private void OnEnable()
    {
        DestroyWwiseGlobalExistance();
        // AkWwisePostImportCallbackSetup.CheckWwiseGlobalExistance();

    }
    private void OnDisable()
    {
        DestroyWwiseGlobalExistance();
    }
    private void Awake()
    {
        if (!Application.isPlaying)
        {
            DestroyWwiseGlobalExistance();
        }
        else
        {
            CreateWiseGlobal();
        }
    }
    public void CreateWiseGlobal()
    {
        wiseObject = new GameObject("WwiseGlobal");
        wiseObject.AddComponent<AkInitializer>();
        wiseObject.AddComponent<WisePluginNotificationRoute>();
    }
    private void OnApplicationQuit()
    {
        //  DestroyWwiseGlobalExistance();
    }
    public static void DestroyWwiseGlobalExistance()
    {

#if UNITY_EDITOR
        if (Application.isPlaying) return;
        while (true)
        {
            var oldObj = UnityEngine.GameObject.Find("WwiseGlobal");
            if (oldObj == null)
                break;
            UnityEditor.Undo.DestroyObjectImmediate(oldObj);

        }
#endif
    }



}
