


[UnityEngine.RequireComponent(typeof(AkTerminator))]
[UnityEngine.DisallowMultipleComponent]
public class AkInitializerExt : UnityEngine.MonoBehaviour
{
    #region Public Data Members

    ///Path for the soundbanks. This must contain one sub folder per platform, with the same as in the Wwise project.
    public string basePath = AkSoundEngineControllerExt.s_DefaultBasePath;

    /// Language sub-folder.
    public string language = AkSoundEngineControllerExt.s_Language;

    ///Default Pool size.  This contains the meta data for your audio project.  Default size is 4 MB, but you should adjust for your needs.
    public int defaultPoolSize = AkSoundEngineControllerExt.s_DefaultPoolSize;

    ///Lower Pool size.  This contains the audio processing buffers and DSP data.  Default size is 2 MB, but you should adjust for your needs.
    public int lowerPoolSize = AkSoundEngineControllerExt.s_LowerPoolSize;

    ///Streaming Pool size.  This contains the streaming buffers.  Default size is 1 MB, but you should adjust for your needs.
    public int streamingPoolSize = AkSoundEngineControllerExt.s_StreamingPoolSize;

    ///Prepare Pool size.  This contains the banks loaded using PrepareBank (Banks decoded on load use this).  Default size is 0 MB, but you should adjust for your needs.
    public int preparePoolSize = AkSoundEngineControllerExt.s_PreparePoolSize;

    ///This setting will trigger the killing of sounds when the memory is reaching 95% of capacity.  Lowest priority sounds are killed.
    public float memoryCutoffThreshold = AkSoundEngineControllerExt.s_MemoryCutoffThreshold;

    ///Monitor Pool size.  Size of the monitoring pool, in bytes. This parameter is not used in Release build.
    public int monitorPoolSize = AkSoundEngineControllerExt.s_MonitorPoolSize;

    ///Monitor Queue Pool size.  Size of the monitoring queue pool, in bytes. This parameter is not used in Release build.
    public int monitorQueuePoolSize = AkSoundEngineControllerExt.s_MonitorQueuePoolSize;

    ///CallbackManager buffer size.  The size of the buffer used per-frame to transfer callback data. Default size is 4 KB, but you should increase this, if required.
    public int callbackManagerBufferSize = AkSoundEngineControllerExt.s_CallbackManagerBufferSize;

    ///Spatial Audio Lower Pool size.  Default size is 4 MB, but you should adjust for your needs.
    public int spatialAudioPoolSize = AkSoundEngineControllerExt.s_SpatialAudioPoolSize;

    [UnityEngine.Range(0, AkSoundEngine.AK_MAX_SOUND_PROPAGATION_DEPTH)]
    /// Spatial Audio Max Sound Propagation Depth. Maximum number of rooms that sound can propagate through; must be less than or equal to AK_MAX_SOUND_PROPAGATION_DEPTH.
    public uint maxSoundPropagationDepth = AkSoundEngine.AK_MAX_SOUND_PROPAGATION_DEPTH;

    [UnityEngine.Tooltip("Default Diffraction Flags combine all the diffraction flags")]
    /// Enable or disable specific diffraction features. See AkDiffractionFlags.
    public AkDiffractionFlags diffractionFlags = AkDiffractionFlags.DefaultDiffractionFlags;

    ///Enable Wwise engine logging. Option to turn on/off the logging of the Wwise engine.
    public bool engineLogging = AkSoundEngineControllerExt.s_EngineLogging;

    #endregion
    public static AkInitializerExt SoleInitiializer { get; private set; }

    public static string GetBasePath()
    {
#if UNITY_EDITOR
        return WwiseSettings.LoadSettings().SoundbankPath;
#else
		return AkSoundEngineController.Instance.basePath;
#endif
    }

    public static string GetCurrentLanguage()
    {
        return AkSoundEngineControllerExt.Instance.language;
    }

    private void Awake()
    {

        UnityEngine.Debug.Assert(SoleInitiializer == null, "SoleInitiializer ERRRRRRRRRROR!!!!!!!!!!!");
        SoleInitiializer = this;

#if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
            return;
#endif

        DontDestroyOnLoad(this);
    }

    private void OnEnable()
    {
         AkSoundEngineControllerExt.Instance.Init(this);
    }

    private void OnDisable()
    {
       AkSoundEngineControllerExt.Instance.OnDisable();
    }

    private void OnDestroy()
    {
        SoleInitiializer = null;
    }

    private void OnApplicationPause(bool pauseStatus)
    {
            AkSoundEngineControllerExt.Instance.OnApplicationPause(pauseStatus);
    }

    private void OnApplicationFocus(bool focus)
    {
            AkSoundEngineControllerExt.Instance.OnApplicationFocus(focus);
    }

    private void OnApplicationQuit()
    {
            AkSoundEngineControllerExt.Instance.Terminate();
    }

    //Use LateUpdate instead of Update() to ensure all gameobjects positions, listener positions, environements, RTPC, etc are set before finishing the audio frame.
    private void LateUpdate()
    {
            AkSoundEngineControllerExt.Instance.LateUpdate();
    }
}
