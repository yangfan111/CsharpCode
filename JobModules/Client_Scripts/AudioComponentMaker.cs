using Core.Audio;
using UnityEngine;
[UnityEngine.ExecuteInEditMode]
[UnityEngine.DisallowMultipleComponent]

public class AudioComponentMaker : MonoBehaviour
{

    public AudioComponentMaker S_Instance { get; private set; }
#if UNITY_EDITOR
    public bool openEditorOptions = false;

    public AudioConfigData configData;

    public bool EnableEditorComponent = false;
#endif
    private void OnEnable()
    {
        if (S_Instance == this)
        {
            DestroyWwiseGlobalExistance();

        }

        // AkWwisePostImportCallbackSetup.CheckWwiseGlobalExistance();

    }
    private void OnDisable()
    {
        if (S_Instance == this)
            DestroyWwiseGlobalExistance();
    }
    private void OnDestroy()
    {
        S_Instance = null;
    }
    private void Awake()
    {

        if (S_Instance && S_Instance != this)
        {
            gameObject.SetActive(false);
            return;

        }
        if (!Application.isPlaying)
        {
            DestroyWwiseGlobalExistance();
        }
        else
        {
            S_Instance = this;
            CreateWiseGlobal();
        }
    }
    public void CreateWiseGlobal()
    {
#if UNITY_EDITOR
        if (configData != null)
        {
            SyncCoreSettings(configData);
        }
        else
        {
            SyncCoreSettings(AudioConfigData.Output);
        }
#else
            SyncCoreSettings(AudioConfigData.Output);
#endif
        // wiseObject = new GameObject("WwiseGlobal");
        gameObject.AddComponent<AkInitializer>();
        gameObject.AddComponent<AudioPluginsDriver>();
    }
    private void OnApplicationQuit()
    {
        //  DestroyWwiseGlobalExistance();
    }
    private void SyncCoreSettings(AudioConfigData configSetting)
    {
        AudioInfluence.IsForbidden = configSetting.isForbiden;
        AudioInfluence.AudioLoadTypeWhenStarup = configSetting.audioLoadTypeWhenStarup;
        AudioPluginSettingAgent.SetCreatePacker(configSetting.usePicker);
        AudioPluginSettingAgent.DeveloperWwiseInstallationPath = configSetting.wiseInstallationPath;
        AudioPluginSettingAgent.DeveloperWwiseProjectPath = configSetting.wiseProjectPath;
    }
    public static void DestroyWwiseGlobalExistance()
    {
        //TODO:
//#if UNITY_EDITOR
//        if (Application.isPlaying) return;
//        //while (true)
//        //{
//        //    //var oldObj = UnityEngine.GameObject.Find("AudioMaker");
//        //    //if (oldObj == null)
//        //    //    break;
//        //    //UnityEditor.Undo.DestroyObjectImmediate(oldObj);

//        //}
//#endif
    }
}
