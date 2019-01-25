


using System.IO;
using UnityEngine;
public class AudioPluginManagement
{
    

    [System.Serializable]
    public class CustomizeSettingData
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

        /// <summary>
        ///�� WAV ���ݵĴ洢λ��(KB)
        /// </summary>
        public uint defaultPoolSizeKB = 4 * 1024;

        /// <summary>
        /// �ײ㲥�Ż����ڴ��.
        ///This contains the audio processing buffers and DSP data.  
        /// </summary>
        public uint lowerPoolSizeKB = 2048;

        /// <summary>
        /// uLEngineDefaultPoolSize �ڴ�ط�ֵ
        /// </summary>
        public float memoryCutoffThreshold = 0.95f;

        /// <summary>
        /// �ռ������Ƶ�ߴ�
        /// </summary>
        public uint spatialAudioPoolSizeKB = 4 * 1024;


        /// <summary>
        /// streaming pool
        /// </summary>
        public uint streamingPoolSizeKB = 2 * 1024;
        /// <summary>
        /// preparePool
        /// </summary>
        public uint preparePoolSizeKB = 0;
        /// <summary>
        /// mem max Pool length
        /// </summary>
        public uint maxPoolNum = 20;

        
        /// <summary>
        /// �ص��ڴ����
        /// </summary>
        public int callbackManagerBufferSize = 4 * 1024;

        //optional 
        //  public string language = AkSoundEngineController.s_Language;


        public string BankFolder_UnityEditor;
        // public string WwiseInstallationPathMac = @"E:\Wwise 2017.2.8.6698\";
        public CustomizeSettingData()
        {
            Init();
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public void Init()
        {
            BankFolder_UnityEditor = System.IO.Path.Combine(Application.dataPath, BankEditorAssetRelativePath);
            AkBasePathGetter.FixSlashes(ref BankFolder_UnityEditor);
        }
    }


    public class AKSettingsManager
    {
        private AkInitSettings akInitSetting;
        private AkSpatialAudioInitSettings spatialSetting;
        private AkPlatformInitSettings platformSetting;
        private AkMusicSettings musicSetting;
        private AkMemSettings memSetting;
        private AkDeviceSettings deviceSetting;
        private AkStreamMgrSettings streamingSetting;
        //ý���ڴ�����ã�default null
        private AkSourceSettings sourceSetting;
        //�������������ã�default null
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

            return Submit();
        }

        bool Submit()
        {
#if UNITY_EDITOR
            AkSoundEngine.SetGameName(UnityEngine.Application.productName + " (Editor)");
#else
		AkSoundEngine.SetGameName(UnityEngine.Application.productName);
#endif

            AKRESULT result = AkSoundEngine.Init(memSetting, streamingSetting, deviceSetting, akInitSetting, platformSetting,
                musicSetting, spatialSetting, SettingData.preparePoolSizeKB * 1024);

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
            //�Զ���
            akInitSetting = new AkInitSettings();
            akInitSetting.uDefaultPoolSize = SettingData.defaultPoolSizeKB * 1024;
            //��ѡ
            ///// Use a separate thread for loading sound banks. Allows asynchronous operations.
            //akInitSetting.bUseSoundBankMgrThread = false;


            ////Memory pool where data allocated by AK::SoundEngine::PrepareEvent() and AK::SoundEngine::PrepareGameSyncs() will be done.
            ///*source code
            // * if (in_preparePoolSizeByte > 0)
            //{
            //    g_PrepareEventPoolId = AK::MemoryMgr::CreatePool(NULL, in_preparePoolSizeByte, AK_UNITY_DEFAULT_POOL_SIZE, AkMalloc);
            //    AK::MemoryMgr::SetPoolName(g_PrepareEventPoolId, AKTEXT("PreparePool"));
            //    in_pSettings->uPrepareEventMemoryPoolID = g_PrepareEventPoolId;
            //}
            //else
            //{
            //    g_PrepareEventPoolId = AK_INVALID_POOL_ID;
            //}*/
            //akInitSetting.uPrepareEventMemoryPoolID = 0;
            ////0.0f to 1.0f value: The percentage of occupied memory where the sound engine should enter in Low memory Mode. �����ڴ���ֵ
            //akInitSetting.fDefaultPoolRatioThreshold = 1.0f;//�������,�ֶ����Ļ���ɼ���ʧ������
            ////Use a separate thread for processing audio. If set to false, audio processing will occur in RenderAudio()
            akInitSetting.bUseLEngineThread = false;
            ////Sets to true to enable AK::SoundEngine::PrepareGameSync usage.
            akInitSetting.bEnableGameSyncPreparation = false;



            akInitSetting.uMonitorPoolSize = (uint)AkSoundEngineController.s_MonitorPoolSize * 1024;
            akInitSetting.uMonitorQueuePoolSize = (uint)AkSoundEngineController.s_MonitorQueuePoolSize * 1024;
            ///����dll��·��
#if (!UNITY_ANDROID && !PLATFORM_LUMIN && !UNITY_WSA) || UNITY_EDITOR // Exclude WSA. It only needs the name of the DLL, and no path.
            akInitSetting.szPluginDLLPath = System.IO.Path.Combine(UnityEngine.Application.dataPath,
                "Plugins" + System.IO.Path.DirectorySeparatorChar);
#elif PLATFORM_LUMIN && !UNITY_EDITOR
        akInitSetting.szPluginDLLPath = UnityEngine.Application.dataPath.Replace("Data", "bin") + System.IO.Path.DirectorySeparatorChar;
#endif
        }
        void CreatePlatformSetting()
        {
            platformSetting = new AkPlatformInitSettings();
            AkSoundEngine.GetDefaultPlatformInitSettings(platformSetting);
            platformSetting.uLEngineDefaultPoolSize = SettingData.lowerPoolSizeKB * 1024;
            platformSetting.fLEngineDefaultPoolRatioThreshold = SettingData.memoryCutoffThreshold;
            ////����������:Sampling Rate. Set to 0 to get the native sample rate. Default value is 0. 
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
            spatialSetting.uPoolSize = SettingData.spatialAudioPoolSizeKB * 1024;
            //Maximum number of portals that sound can propagate through; must be less than or equal to AK_MAX_SOUND_PROPAGATION_DEPTH.
            spatialSetting.uMaxSoundPropagationDepth = AkSoundEngine.AK_MAX_SOUND_PROPAGATION_DEPTH;
            
            ////������ЧӦ������� Diffraction occurs when a sound wave strikes a small obstacle
            //spatialSetting.uDiffractionFlags = AkDiffractionFlags.DefaultDiffractionFlags;
            //// Multiplier that is applied to the distance attenuation of diffracted sounds (sounds that are in the 'shadow region') to simulate the phenomenon where by diffracted sound waves decay faster than incident sound waves. 
            //spatialSetting.fDiffractionShadowAttenFactor = 1.0f;
            //// Interpolation angle, in degrees, over which the fDiffractionShadowAttenFactor is applied.  
            //spatialSetting.fDiffractionShadowDegrees =0f;
        }

        public void CreateMicsSettings()
        {
            musicSetting = new AkMusicSettings();
            AkSoundEngine.GetDefaultMusicSettings(musicSetting);
            memSetting = new AkMemSettings();
            memSetting.uMaxNumPools = SettingData.maxPoolNum;

            deviceSetting = new AkDeviceSettings();
            //����Ĭ������
            AkSoundEngine.GetDefaultDeviceSettings(deviceSetting);

            streamingSetting = new AkStreamMgrSettings();
            // Size of memory pool for small objects of Stream Manager.
            streamingSetting.uMemorySize = SettingData.streamingPoolSizeKB * 1024;


        }
    }


    static CustomizeSettingData TryLoadSettingFile()
    {
        CustomizeSettingData localConfigData;
        try
        {
            var path = Path.Combine(Application.dataPath, "Wwise/config.xml");
            if (File.Exists(path))
            {
                var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(CustomizeSettingData));
                var xmlFileStream = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                localConfigData = xmlSerializer.Deserialize(xmlFileStream) as CustomizeSettingData;
                xmlFileStream.Close();
            }
            else
            {
                localConfigData = new CustomizeSettingData();
                var xmlDoc = new System.Xml.XmlDocument();
                var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(CustomizeSettingData));
                using (var xmlStream = new System.IO.MemoryStream())
                {
                    var streamWriter = new System.IO.StreamWriter(xmlStream, System.Text.Encoding.UTF8);
                    xmlSerializer.Serialize(streamWriter, localConfigData);
                    xmlStream.Position = 0;
                    xmlDoc.Load(xmlStream);
                    xmlDoc.Save(path);
                }

            }
            return localConfigData;
        }
        catch (System.Exception)
        {
        }
        return null;

    }




    public static CustomizeSettingData SettingData
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
    private static CustomizeSettingData settingData;



    private static AKSettingsManager settingMgr;
    public static AKSettingsManager SettingMgr
    {
        get
        {
            if (settingMgr == null) settingMgr = new AKSettingsManager();
            return settingMgr;
        }
    }





    public static string DeveloperWwiseInstallationPath = "";//= @"E:\Wwise 2017.2.8.6698\";
    public static string DeveloperWwiseProjectPath = ""; //@"E:\MyWwise\ShengSiJuJi\ShengSiJuJi\ShengSiJuJi.wproj";


    public static bool GetCreatePacker()
    {
#if UNITY_EDITOR

        return CustomizeSettingData.D_CreatedPicker;

#else
                    return false;
#endif
    }

    public static void SetCreatePacker(bool usePicker)
    {

#if UNITY_EDITOR
        CustomizeSettingData.D_CreatedPicker = usePicker;
#endif
    }


    public static bool GetCreateWwiseGlobal()
    {
#if UNITY_EDITOR


        return CustomizeSettingData.D_CreateWwiseGlobal;

#else
          return ProjCustomizeSettings.CreateWwiseGlobal;
#endif
    }
    public static bool GetCreateWwiseListener()
    {
#if UNITY_EDITOR

        return CustomizeSettingData.D_CreateWwiseListener;
#else
          return ProjCustomizeSettings.CreateWwiseListener;
#endif
    }
    public static string GetBankAssetFolder()
    {
#if UNITY_EDITOR

        string path_unityEditor = System.IO.Path.Combine(Application.dataPath, CustomizeSettingData.BankEditorAssetRelativePath);
        AkBasePathGetter.FixSlashes(ref path_unityEditor);
        return path_unityEditor;


#else
                return Application.streamingAssetsPath;
          //return Application.dataPath + "/StreamingAssets";
#endif
    }




}