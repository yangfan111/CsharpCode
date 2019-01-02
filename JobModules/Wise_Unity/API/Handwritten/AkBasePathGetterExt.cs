
public partial class AkBasePathGetter
{

    /// <summary>
    ///  PlatformBasePath === SoundbankBasePath
    /// </summary>
    /// <returns></returns>
    public static string GetPlatformBasePath()
    {
        return AudioPluginSettingAgent.GetBankAssetFolder();

//        var projSettingData = AKCustomizeSettings.ProjCustomizeSettings;
//#if UNITY_EDITOR

//        return projSettingData.BankFolder_UnityEditor;
//#endif
//        return "";
        // Combine base path with platform sub-folder
        //var platformBasePath = System.IO.Path.Combine(GetFullSoundBankPath(), platformName);
        //FixSlashes(ref platformBasePath);
        //return platformBasePath;
    }
    public static string GetPlatformName()
    {
        return "Windows";
    }
    public static string GetFullSoundBankPath()
    {
        return GetPlatformBasePath();
    }
    ///-纯粹复制，方便查看
    public static string GetSoundbankBasePath()
    {
        var basePathToSet = GetPlatformBasePath();
        var InitBnkFound = true;
#if UNITY_EDITOR || !(UNITY_ANDROID || PLATFORM_LUMIN)// Can't use File.Exists on Android, assume banks are there
        var InitBankPath = System.IO.Path.Combine(basePathToSet, "Init.bnk");
        if (!System.IO.File.Exists(InitBankPath))
            InitBnkFound = false;
#endif

        if (basePathToSet == string.Empty || InitBnkFound == false)
        {
            UnityEngine.Debug.Log("WwiseUnity: Looking for SoundBanks in " + basePathToSet);

#if UNITY_EDITOR
            UnityEngine.Debug.LogError("WwiseUnity: Could not locate the SoundBanks. Did you make sure to generate them?");
#else
			UnityEngine.Debug.LogError("WwiseUnity: Could not locate the SoundBanks. Did you make sure to copy them to the StreamingAssets folder?");
#endif
        }

        return basePathToSet;
    }
}