


using System.IO;
using UnityEngine;
public class AudioPluginManagement
{
    private static AudioPluginManagement instance;
    public static AudioPluginManagement Instance
    {
        get
        {
            if (instance == null)
                instance = new AudioPluginManagement();
            return instance;
        }
    }
    public readonly AKSettingsManager SettingMgr = new AKSettingsManager();
    private AudioPluginSettingData settingData;
    public AudioPluginSettingData SettingData
    {
        get
        {
            if (settingData == null)
            {
#if UNITY_EDITOR
                settingData = TryLoadSettingFile();
#else

 customizeSettings = new AudioPluginSettingData();
#endif
            }
            return settingData;
        }
    }





    public class AKSettingsManager
    {
        private AkInitSettings initSettings;
        private AkSpatialAudioInitSettings spatialSetting;
        private AkPlatformInitSettings platformSetting;
        private AkMusicSettings musicSetting;
        private AkMemSettings memSetting;
        private AkDeviceSettings deviceSetting;
        private AkStreamMgrSettings streamingSetting;
        //媒体内存池设置，default null
        private AkSourceSettings sourceSetting;
        //采样率数据设置，default null
        private AkAudioSettings sampleSetting;
        private bool isInitialized = false;
        public bool Initialize()
        {
            if (!isInitialized)
            {
                CreateInitSetting();
                CreatePlatformSetting();
                CreateSpatialSetting();
                CreateMicsSettings();
                isInitialized = true;
            }

            return RegisterSettings();
        }

        bool RegisterSettings()
        {
#if UNITY_EDITOR
            AkSoundEngine.SetGameName(UnityEngine.Application.productName + " (Editor)");
#else
		AkSoundEngine.SetGameName(UnityEngine.Application.productName);
#endif

            AKRESULT result = AkSoundEngine.Init(memSetting, streamingSetting, deviceSetting, initSettings, platformSetting,
                musicSetting, spatialSetting, instance.SettingData.preparePoolSizeKB * 1024);

            if (result != AKRESULT.AK_Success)
            {
                UnityEngine.Debug.LogError("WwiseUnity: Failed to initialize the sound engine. Abort.");
                AkSoundEngine.Term();
                return false; //AkSoundEngine.Init should have logged more details.
            }

            var basePathToSet = AkBasePathGetter.GetSoundbankBasePath();
            if (string.IsNullOrEmpty(basePathToSet))
            {
                UnityEngine.Debug.LogError("WwiseUnity: Couldn't find soundbanks base path. Terminate sound engine.");
                AkSoundEngine.Term();
                return false;
            }

            result = AkSoundEngine.SetBasePath(basePathToSet);
            if (result != AKRESULT.AK_Success)
            {
                UnityEngine.Debug.LogError("WwiseUnity: Failed to set soundbanks base path. Terminate sound engine.");
                AkSoundEngine.Term();
                return false;
            }
            LateInitSetting();
            return true;


        }
        void LateInitSetting()
        {
            //sampleSetting = new AkAudioSettings();
            //AkSoundEngine.GetAudioSettings(sampleSetting);
            //sampleSetting.uNumSamplesPerFrame = 0;
            //sampleSetting.uNumSamplesPerSecond = 0;
            //sourceSetting = new AkSourceSettings();
            //sourceSetting.uMediaSize = 0;
            //sourceSetting.sourceID = 0;
            //sourceSetting.pMediaMemory = 0b0;
        }
        void CreateInitSetting()
        {
            //自定义
            initSettings = new AkInitSettings();
            AkSoundEngine.GetDefaultInitSettings(initSettings);
            initSettings.uDefaultPoolSize = instance.SettingData.defaultPoolSizeKB * 1024;
            //可选
            // /// Use a separate thread for loading sound banks. Allows asynchronous operations.
            // akInitSetting.bUseSoundBankMgrThread = false;


            // //Memory pool where data allocated by AK::SoundEngine::PrepareEvent() and AK::SoundEngine::PrepareGameSyncs() will be done.
            // /*source code
            //  * if (in_preparePoolSizeByte > 0)
            // {
            //     g_PrepareEventPoolId = AK::MemoryMgr::CreatePool(NULL, in_preparePoolSizeByte, AK_UNITY_DEFAULT_POOL_SIZE, AkMalloc);
            //     AK::MemoryMgr::SetPoolName(g_PrepareEventPoolId, AKTEXT("PreparePool"));
            //     in_pSettings->uPrepareEventMemoryPoolID = g_PrepareEventPoolId;
            // }
            // else
            // {
            //     g_PrepareEventPoolId = AK_INVALID_POOL_ID;
            // }*/
            // akInitSetting.uPrepareEventMemoryPoolID = 0;
            // //0.0f to 1.0f value: The percentage of occupied memory where the sound engine should enter in Low memory Mode. 定义内存阈值
            // akInitSetting.fDefaultPoolRatioThreshold = 1.0f;//无需更改,手动更改会造成加载失败问题
            // //Use a separate thread for processing audio. If set to false, audio processing will occur in RenderAudio()
            // //Sets to true to enable AK::SoundEngine::PrepareGameSync usage.
            //// akInitSetting.bUseLEngineThread = false;
            //// akInitSetting.bEnableGameSyncPreparation = false;



            initSettings.uMonitorPoolSize = (uint)AkSoundEngineController.s_MonitorPoolSize * 1024;
            initSettings.uMonitorQueuePoolSize = (uint)AkSoundEngineController.s_MonitorQueuePoolSize * 1024;
            ///设置dll的路径
#if (!UNITY_ANDROID && !PLATFORM_LUMIN && !UNITY_WSA) || UNITY_EDITOR // Exclude WSA. It only needs the name of the DLL, and no path.
            initSettings.szPluginDLLPath = System.IO.Path.Combine(UnityEngine.Application.dataPath,
                "Plugins" + System.IO.Path.DirectorySeparatorChar);
#elif PLATFORM_LUMIN && !UNITY_EDITOR
        akInitSetting.szPluginDLLPath = UnityEngine.Application.dataPath.Replace("Data", "bin") + System.IO.Path.DirectorySeparatorChar;
#endif
        }
        void CreatePlatformSetting()
        {
            platformSetting = new AkPlatformInitSettings();
            AkSoundEngine.GetDefaultPlatformInitSettings(platformSetting);
            platformSetting.uLEngineDefaultPoolSize = instance.SettingData.lowerPoolSizeKB * 1024;
            platformSetting.fLEngineDefaultPoolRatioThreshold = instance.SettingData.memoryCutoffThreshold;
            ////采样率设置:Sampling Rate. Set to 0 to get the native sample rate. Default value is 0. 
            //platformSettings.uSampleRate = 0;
            //// Lower engine threading properties. 
            //platformSettings.threadLEngine = null;
            //// Bank manager threading properties (its default priority AK_THREAD_PRIORITY_NORMAL). 
            //platformSettings.threadBankManager = null;

        }
        void CreateSpatialSetting()
        {
            spatialSetting = new AkSpatialAudioInitSettings();
            /// Desired memory pool size if a new pool should be created. A pool will be created if uPoolID is not set (AK_INVALID_POOL_ID).
            spatialSetting.uPoolSize = instance.SettingData.spatialAudioPoolSizeKB * 1024;
            //Maximum number of portals that sound can propagate through; must be less than or equal to AK_MAX_SOUND_PROPAGATION_DEPTH.
            spatialSetting.uMaxSoundPropagationDepth = AkSoundEngine.AK_MAX_SOUND_PROPAGATION_DEPTH;

            ////多普勒效应参数相关 Diffraction occurs when a sound wave strikes a small obstacle
            spatialSetting.uDiffractionFlags = (uint)AkDiffractionFlags.DefaultDiffractionFlags;
            //// Multiplier that is applied to the distance attenuation of diffracted sounds (sounds that are in the 'shadow region') to simulate the phenomenon where by diffracted sound waves decay faster than incident sound waves. 
            //spatialSetting.fDiffractionShadowAttenFactor = 1.0f;
            //// Interpolation angle, in degrees, over which the fDiffractionShadowAttenFactor is applied.  
            //spatialSetting.fDiffractionShadowDegrees = 0f;
        }

        public void CreateMicsSettings()
        {
            musicSetting = new AkMusicSettings();
            AkSoundEngine.GetDefaultMusicSettings(musicSetting);
            memSetting = new AkMemSettings();
            memSetting.uMaxNumPools = instance.SettingData.maxPoolNum;

            deviceSetting = new AkDeviceSettings();
            //带回默认设置
            AkSoundEngine.GetDefaultDeviceSettings(deviceSetting);

            streamingSetting = new AkStreamMgrSettings();
            // Size of memory pool for small objects of Stream Manager.
            streamingSetting.uMemorySize = instance.SettingData.streamingPoolSizeKB * 1024;


        }
    }
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void SaveSettingFile()
    {
        var path = Path.Combine(Application.dataPath, "Wwise/config.xml");

        if (!File.Exists(path))
        {
            var xmlDoc = new System.Xml.XmlDocument();
            var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(AudioPluginSettingData));
            using (var xmlStream = new System.IO.MemoryStream())
            {
                var streamWriter = new System.IO.StreamWriter(xmlStream, System.Text.Encoding.UTF8);
                xmlSerializer.Serialize(streamWriter, Instance.SettingData);
                xmlStream.Position = 0;
                xmlDoc.Load(xmlStream);
                xmlDoc.Save(path);
            }


        }
    }

    AudioPluginSettingData TryLoadSettingFile()
    {
        AudioPluginSettingData localConfigData;
        try
        {
            var path = Path.Combine(Application.dataPath, "Wwise/config.xml");
            if (File.Exists(path))
            {
                var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(AudioPluginSettingData));
                var xmlFileStream = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                localConfigData = xmlSerializer.Deserialize(xmlFileStream) as AudioPluginSettingData;
                xmlFileStream.Close();
            }
            else
            {
                localConfigData = new AudioPluginSettingData();

            }
            return localConfigData;
        }
        catch (System.Exception)
        {
        }
        return null;

    }













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
    /// <summary>
    /// bank的最终目录
    /// </summary>
    /// <returns></returns>
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
    public static string[] GetBankAssetNamesByFolder(string folder)
    {
        try
        {
            string assetFolder = GetBankAssetFolder();
            if (string.IsNullOrEmpty(folder))
            {
                var paths = Directory.GetFiles(assetFolder, "*.bnk", SearchOption.TopDirectoryOnly);
                for (int i = 0; i < paths.Length; i++)
                    paths[i] = Path.GetFileName(paths[i]);
                return paths;
            }
        }
        catch (System.Exception e)
        {

        }
        return null;

    }




}