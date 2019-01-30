


using UnityEngine;
public class AudioPluginSettingAgent
{
    public static AudioPluginSettingData ProjCustomizeSettings
    {
        get
        {
            if (customizeSettings == null)
            {
                customizeSettings = new AudioPluginSettingData();
                customizeSettings.Init();
            }
            return customizeSettings;
        }
    }
    private static AudioPluginSettingData customizeSettings;
    public static string DeveloperWwiseInstallationPath = "";//= @"E:\Wwise 2017.2.8.6698\";
    public static string DeveloperWwiseProjectPath = ""; //@"E:\MyWwise\ShengSiJuJi\ShengSiJuJi\ShengSiJuJi.wproj";
    public static bool GetCreatePacker()
    {
#if UNITY_EDITOR

        return AudioPluginSettingData.D_CreatedPicker;

#else
                    return false;
#endif
    }

    public static void SetCreatePacker(bool usePicker)
    {

#if UNITY_EDITOR
        AudioPluginSettingData.D_CreatedPicker = usePicker;
#endif
    }


    public static bool GetCreateWwiseGlobal()
    {
#if UNITY_EDITOR


        return AudioPluginSettingData.D_CreateWwiseGlobal;

#else
          return ProjCustomizeSettings.CreateWwiseGlobal;
#endif
    }
    public static bool GetCreateWwiseListener()
    {
#if UNITY_EDITOR

        return AudioPluginSettingData.D_CreateWwiseListener;
#else
          return ProjCustomizeSettings.CreateWwiseListener;
#endif
    }
    public static string GetBankAssetFolder()
    {
#if UNITY_EDITOR

        string path_unityEditor = System.IO.Path.Combine(Application.dataPath, AudioPluginSettingData.BankEditorAssetRelativePath);
        AkBasePathGetter.FixSlashes(ref path_unityEditor);
        return path_unityEditor;


#else
                return Application.streamingAssetsPath;
          //return Application.dataPath + "/StreamingAssets";
#endif
    }

    [System.Serializable]
    public class AudioPluginSettingData
    {

        //  public const string WwiseSettingsFilename = "WwiseSettings.xml";

        //  private static WwiseSettings instance;

        public bool CopySoundBanksAsPreBuildStep = false;

        public static bool D_CreatedPicker = false;
        //default is true
        public bool CreateWwiseGlobal = true;
        public const bool D_CreateWwiseGlobal = true;
        //default is false
        public bool CreateWwiseListener = false;
        public static bool D_CreateWwiseListener = false;

        public bool GenerateSoundBanksAsPreBuildStep = false;
        public bool ShowMissingRigidBodyWarning = false;
        public static string BankEditorAssetRelativePath = "Assets/Sound/WiseBank";


        public string BankFolder_UnityEditor;
        // public string WwiseInstallationPathMac = @"E:\Wwise 2017.2.8.6698\";



        public void Init()
        {
#if UNITY_EDITOR
            BankFolder_UnityEditor = System.IO.Path.Combine(Application.dataPath, BankEditorAssetRelativePath);
            AkBasePathGetter.FixSlashes(ref BankFolder_UnityEditor);
#else
#endif
        }



    }
}