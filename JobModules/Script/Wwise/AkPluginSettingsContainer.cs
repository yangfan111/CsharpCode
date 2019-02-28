public class AkPluginSettingsContainer : AkPluginSettingsContainerAbstract
{
    [UnityEngine.HideInInspector]
    public System.Collections.Generic.List<string> PlatformSettingsNameList
        = new System.Collections.Generic.List<string>();

    [UnityEngine.HideInInspector]
    public System.Collections.Generic.List<PlatformSettings> PlatformSettingsList
        = new System.Collections.Generic.List<PlatformSettings>();

    [UnityEngine.HideInInspector]
    public System.Collections.Generic.List<string> InvalidReferencePlatforms
        = new System.Collections.Generic.List<string>();

    public bool IsValid
    {
        get { return PlatformSettingsNameList.Count == PlatformSettingsList.Count; }
    }

    public int Count
    {
        get { return PlatformSettingsList.Count; }
    }

    [UnityEngine.HideInInspector]
    public AkCommonUserSettings UserSettings;
    [UnityEngine.HideInInspector]
    public AkCommonAdvancedSettings AdvancedSettings;
    [UnityEngine.HideInInspector]
    public AkCommonCommSettings CommsSettings;

    protected override AkCommonUserSettings GetUserSettings()
    {
        return UserSettings;
    }

    protected override AkCommonAdvancedSettings GetAdvancedSettings()
    {
        return AdvancedSettings;
    }

    protected override AkCommonCommSettings GetCommsSettings()
    {
        return CommsSettings;
    }








    private static readonly string[] AllGlobalValues = new[]
    {
        "UserSettings.m_BasePath",
        "UserSettings.m_StartupLanguage",
        "UserSettings.m_PreparePoolSize",
        "UserSettings.m_CallbackManagerBufferSize",
        "UserSettings.m_EngineLogging",
        "UserSettings.m_MaximumNumberOfMemoryPools",
        "UserSettings.m_MaximumNumberOfPositioningPaths",
        "UserSettings.m_DefaultPoolSize",
        "UserSettings.m_MemoryCutoffThreshold",
        "UserSettings.m_CommandQueueSize",
        "UserSettings.m_SamplesPerFrame",
        "UserSettings.m_MainOutputSettings.m_AudioDeviceShareset",
        "UserSettings.m_MainOutputSettings.m_DeviceID",
        "UserSettings.m_MainOutputSettings.m_PanningRule",
        "UserSettings.m_MainOutputSettings.m_ChannelConfig.m_ChannelConfigType",
        "UserSettings.m_MainOutputSettings.m_ChannelConfig.m_ChannelMask",
        "UserSettings.m_MainOutputSettings.m_ChannelConfig.m_NumberOfChannels",
        "UserSettings.m_StreamingLookAheadRatio",
        "UserSettings.m_StreamManagerPoolSize",
        "UserSettings.m_SampleRate",
        "UserSettings.m_LowerEnginePoolSize",
        "UserSettings.m_LowerEngineMemoryCutoffThreshold",
        "UserSettings.m_NumberOfRefillsInVoice",
        "UserSettings.m_SpatialAudioSettings.m_PoolSize",
        "UserSettings.m_SpatialAudioSettings.m_MaxSoundPropagationDepth",
        "UserSettings.m_SpatialAudioSettings.m_DiffractionFlags",
        "CommsSettings.m_PoolSize",
        "CommsSettings.m_DiscoveryBroadcastPort",
        "CommsSettings.m_CommandPort",
        "CommsSettings.m_NotificationPort",
        "CommsSettings.m_InitializeSystemComms",
        "CommsSettings.m_NetworkName",
        "AdvancedSettings.m_IOMemorySize",
        "AdvancedSettings.m_TargetAutoStreamBufferLengthMs",
        "AdvancedSettings.m_UseStreamCache",
        "AdvancedSettings.m_MaximumPinnedBytesInCache",
        "AdvancedSettings.m_PrepareEventMemoryPoolID",
        "AdvancedSettings.m_EnableGameSyncPreparation",
        "AdvancedSettings.m_ContinuousPlaybackLookAhead",
        "AdvancedSettings.m_MonitorPoolSize",
        "AdvancedSettings.m_MonitorQueuePoolSize",
        "AdvancedSettings.m_MaximumHardwareTimeoutMs",
        "AdvancedSettings.m_SpatialAudioSettings.m_DiffractionShadowAttenuationFactor",
        "AdvancedSettings.m_SpatialAudioSettings.m_DiffractionShadowDegrees",
    };

    public abstract class PlatformSettings : AkPluginSettingsContainerAbstract
    {
        #region Ignore property list management
        [UnityEngine.SerializeField]
        [UnityEngine.HideInInspector]
        private System.Collections.Generic.List<string> IgnorePropertyNameList =
            new System.Collections.Generic.List<string>();

        public void IgnorePropertyValue(string propertyPath)
        {
            if (IsPropertyIgnored(propertyPath))
                return;

            IgnorePropertyNameList.Add(propertyPath);
            SetUseGlobalPropertyValue(propertyPath, false);
        }

        public bool IsPropertyIgnored(string propertyPath)
        {
            return IgnorePropertyNameList.Contains(propertyPath);
        }
        #endregion

        #region Global property list management
        [UnityEngine.SerializeField]
        [UnityEngine.HideInInspector]
        private System.Collections.Generic.List<string> GlobalPropertyNameList =
            new System.Collections.Generic.List<string>();

        protected PlatformSettings()
        {
            SetGlobalPropertyValues(AllGlobalValues);
        }

        public void SetUseGlobalPropertyValue(string propertyPath, bool use)
        {
            if (IsUsingGlobalPropertyValue(propertyPath) == use)
                return;

            if (use)
                GlobalPropertyNameList.Add(propertyPath);
            else
                GlobalPropertyNameList.Remove(propertyPath);

            _GlobalPropertyHashSet = null;
        }

        public void SetGlobalPropertyValues(System.Collections.IEnumerable enumerable)
        {
            foreach (var item in enumerable)
            {
                string propertyPath = item as string;
                if (!IsUsingGlobalPropertyValue(propertyPath))
                    GlobalPropertyNameList.Add(propertyPath);
            }
        }

        private bool IsUsingGlobalPropertyValue(string propertyPath)
        {
            return GlobalPropertyNameList.Contains(propertyPath);
        }

        private System.Collections.Generic.HashSet<string> _GlobalPropertyHashSet = null;

        public System.Collections.Generic.HashSet<string> GlobalPropertyHashSet
        {
            get
            {
                if (_GlobalPropertyHashSet == null)
                    _GlobalPropertyHashSet = new System.Collections.Generic.HashSet<string>(GlobalPropertyNameList);
                return _GlobalPropertyHashSet;
            }
            set { _GlobalPropertyHashSet = value; }
        }
        #endregion

#if UNITY_EDITOR
        protected static void RegisterPlatformSettingsClass<T>(string platformName) where T : PlatformSettings
        {
            string className = typeof(T).Name;
            string currentClassName;
            if (m_PlatformSettingsClassNames.TryGetValue(platformName, out currentClassName) && currentClassName == className)
            {
                UnityEngine.Debug.LogWarning("WwiseUnity: The class <" + currentClassName + "> is being replaced by <" + className + "> for the reference platform: " + platformName);
                return;
            }

            m_PlatformSettingsClassNames[platformName] = className;
        }
#endif
    }

    public class CommonPlatformSettings : PlatformSettings
    {
        protected override AkCommonUserSettings GetUserSettings()
        {
            return UserSettings;
        }

        protected override AkCommonAdvancedSettings GetAdvancedSettings()
        {
            return AdvancedSettings;
        }

        protected override AkCommonCommSettings GetCommsSettings()
        {
            return CommsSettings;
        }

        [UnityEngine.HideInInspector]
        public AkCommonUserSettings UserSettings;
        [UnityEngine.HideInInspector]
        public AkCommonAdvancedSettings AdvancedSettings;
        [UnityEngine.HideInInspector]
        public AkCommonCommSettings CommsSettings;
    }

    #region Singleton management
    private static AkPluginSettingsContainer m_Instance = null;
    private static IPluginSettingProperty m_ActivePlatformSettings = null;

    public static AkPluginSettingsContainer Instance
    {
        get
        {
            if (m_Instance == null)
            {
#if UNITY_EDITOR
                var name = typeof(AkPluginSettingsContainer).Name;
                m_Instance = AkUtilities.GetOrCreateWiseResAsset<AkPluginSettingsContainer>(name, name);
#else
				m_Instance = CreateInstance<AkPluginSettingsContainer>();
				UnityEngine.Debug.LogWarning("WwiseUnity: No platform specific settings were created. Default initialization settings will be used.");
#endif
            }
            return m_Instance;
        }
    }

    private static IPluginSettingProperty GetPlatformSettings(string platformName)
    {
        var instance = Instance;
        if (!instance.IsValid)
            return instance;

        for (var i = 0; i < instance.Count; ++i)
        {
            var platformSettings = instance.PlatformSettingsList[i];
            if (platformSettings != null && (string.Compare(platformName, instance.PlatformSettingsNameList[i], true) == 0))
                return platformSettings;
        }

        UnityEngine.Debug.LogWarning("WwiseUnity: Platform specific settings cannot be found for <" + platformName + ">. Using global settings.");
        return instance;
    }

    public static IPluginSettingProperty ActivePlatformSettings
    {
        get
        {
            if (m_ActivePlatformSettings == null)
                m_ActivePlatformSettings = GetPlatformSettings(AkBasePathGetter.GetPlatformName());

            return m_ActivePlatformSettings;
        }
    }

    private void OnEnable()
    {
        if (m_Instance == null)
            m_Instance = this;
        else if (m_Instance != this)
            UnityEngine.Debug.LogWarning("WwiseUnity: There are multiple AkPluginSettingsContainer objects instantiated; only one will be used.");
    }
    #endregion
   


#if UNITY_EDITOR
    [UnityEditor.MenuItem("Edit/Project Settings/Wwise Initialization Settings")]
    private static void WwiseInitializationSettingsMenuItem()
    {
        UnityEditor.Selection.activeObject = Instance;
    }


    private static System.Collections.Generic.Dictionary<string, string> m_PlatformSettingsClassNames
        = new System.Collections.Generic.Dictionary<string, string>();

    public const System.Reflection.BindingFlags BindingFlags
        = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic;

  
    [UnityEngine.HideInInspector]
    public bool ActiveSettingsHaveChanged = true;

    [UnityEngine.HideInInspector]
    public string ActiveSettingsHash;

    public static void UpdatePlatforms()
    {
        if (!AkUtilities.IsWwiseProjectAvailable)
            return;

        var customPlatformSettingsMap = new System.Collections.Generic.Dictionary<string, PlatformSettings>();
        var instance = Instance;
        if (instance.IsValid)
        {
            for (var i = 0; i < instance.Count; ++i)
            {
                var settings = instance.PlatformSettingsList[i];
                var name = instance.PlatformSettingsNameList[i];
                if (settings && !string.IsNullOrEmpty(name))
                    customPlatformSettingsMap.Add(name, settings);
            }
        }

        var updated = false;

        var allCustomPlatforms = new System.Collections.Generic.List<string>();
        foreach (var pair in AkUtilities.PlatformMapping)
        {
            var referencePlatform = pair.Key;
            var customPlatformList = pair.Value;

            string className;
            if (!m_PlatformSettingsClassNames.TryGetValue(referencePlatform, out className))
            {
                if (!instance.InvalidReferencePlatforms.Contains(referencePlatform))
                {
                    instance.InvalidReferencePlatforms.Add(referencePlatform);
                    UnityEngine.Debug.LogError("WwiseUnity: A class has not been registered for the reference platform: " + referencePlatform);
                }
                continue;
            }

            foreach (var customWwisePlatform in customPlatformList)
            {
                allCustomPlatforms.Add(customWwisePlatform);
                if (customPlatformSettingsMap.ContainsKey(customWwisePlatform))
                    continue;

                var settings = AkUtilities.GetOrCreateWiseResAsset<PlatformSettings>(className, customWwisePlatform);
                customPlatformSettingsMap.Add(customWwisePlatform, settings);
                updated = true;
            }
        }

        var customPlatformSettingsToRemoveMap = new System.Collections.Generic.Dictionary<string, PlatformSettings>();
        foreach (var pair in customPlatformSettingsMap)
        {
            var instantiatedCustomPlatform = pair.Key;
            if (!allCustomPlatforms.Contains(instantiatedCustomPlatform))
                customPlatformSettingsToRemoveMap.Add(instantiatedCustomPlatform, pair.Value);
        }

        foreach (var pair in customPlatformSettingsToRemoveMap)
        {
            var instantiatedCustomPlatform = pair.Key;
            customPlatformSettingsMap.Remove(instantiatedCustomPlatform);
            UnityEditor.AssetDatabase.DeleteAsset(AkUtilities.Asset__WiseResSubFolder__FileExtPath_Relative(instantiatedCustomPlatform));
            updated = true;
        }

        if (updated)
        {
            instance.PlatformSettingsNameList.Clear();
            instance.PlatformSettingsList.Clear();

            AkUtilities.RepaintInspector();

            var keys = System.Linq.Enumerable.ToList(customPlatformSettingsMap.Keys);
            keys.Sort();

            foreach (var key in keys)
            {
                instance.PlatformSettingsNameList.Add(key);
                instance.PlatformSettingsList.Add(customPlatformSettingsMap[key]);
            }

            UnityEditor.EditorUtility.SetDirty(instance);
            UnityEditor.AssetDatabase.SaveAssets();
            AkUtilities.RepaintInspector();
        }
    }

    
#endif
}
