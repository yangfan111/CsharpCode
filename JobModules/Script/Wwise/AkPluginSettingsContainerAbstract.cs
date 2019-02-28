public abstract class AkPluginSettingsContainerAbstract : UnityEngine.ScriptableObject, IPluginSettingProperty
{
    protected abstract AkCommonUserSettings GetUserSettings();

    protected abstract AkCommonAdvancedSettings GetAdvancedSettings();

    protected abstract AkCommonCommSettings GetCommsSettings();

    /// <summary>
    /// AkCommonUserSettings=>AkInitializationSettings
    /// </summary>
	public AkInitializationSettings AkInitializationSettings
    {
        get
        {
            var settings = new AkInitializationSettings();
            AkCommonUserSettings userSettings = GetUserSettings();
            userSettings.CopyTo(settings.memSettings);
            userSettings.CopyTo(settings.deviceSettings);
            userSettings.CopyTo(settings.streamMgrSettings);
            userSettings.CopyTo(settings.initSettings);
            userSettings.CopyTo(settings.platformSettings);
            userSettings.CopyTo(settings.musicSettings);
            userSettings.CopyTo(settings.unityPlatformSpecificSettings);
            settings.preparePoolSize = userSettings.m_PreparePoolSize;

            var advancedSettings = GetAdvancedSettings();
            advancedSettings.CopyTo(settings.deviceSettings);
            advancedSettings.CopyTo(settings.initSettings);
            advancedSettings.CopyTo(settings.platformSettings);
            advancedSettings.CopyTo(settings.unityPlatformSpecificSettings);
            return settings;
        }
    }

    public AkSpatialAudioInitSettings AkSpatialAudioInitSettings
    {
        get
        {
            var settings = new AkSpatialAudioInitSettings();
            GetUserSettings().CopyTo(settings);
            GetAdvancedSettings().CopyTo(settings);
            return settings;
        }
    }

    public AkCallbackManager.InitializationSettings CallbackManagerInitializationSettings
    {
        get
        {
            var userSettings = GetUserSettings();
            return new AkCallbackManager.InitializationSettings { BufferSize = userSettings.m_CallbackManagerBufferSize, IsLoggingEnabled = userSettings.m_EngineLogging };
        }
    }

    public string InitialLanguage
    {
        get
        {
            return GetUserSettings().m_StartupLanguage;
        }
    }

    //public string SoundbankPath
    //{
    //    get
    //    {
    //        return GetUserSettings().m_EditorBasePath;
    //    }
    //}

    public AkCommunicationSettings AkCommunicationSettings
    {
        get
        {
            var settings = new AkCommunicationSettings();
            GetCommsSettings().CopyTo(settings);
            return settings;
        }
    }

    #region parameter validation
#if UNITY_EDITOR
    void OnValidate()
    {
        GetUserSettings().Validate();
        GetAdvancedSettings().Validate();
        GetCommsSettings().Validate();
    }
#endif
    #endregion
}

