using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;



public static class AKProcessProvider
{


    #region Sound Engine Initialization
    //AksoundEngine -> Init
    public static bool InitializeSoundEngine()
    {
        
#if UNITY_EDITOR
        AkPluginSettingsContainer.Instance.ActiveSettingsHash = AkUtilities.GetHashOfActiveSettings();
        AkPluginSettingsContainer.Instance.ActiveSettingsHaveChanged = true;
#endif

        if (AkSoundEngine.Init(AkPluginSettingsContainer.ActivePlatformSettings.AkInitializationSettings) != AKRESULT.AK_Success)
        {
            UnityEngine.Debug.LogError("WwiseUnity: Failed to initialize the sound engine. Abort.");
            AkSoundEngine.Term();
            return false;
        }

        if (AkSoundEngine.InitSpatialAudio(AkPluginSettingsContainer.ActivePlatformSettings.AkSpatialAudioInitSettings) != AKRESULT.AK_Success)
        {
            UnityEngine.Debug.LogWarning("WwiseUnity: Failed to initialize spatial audio.");
        }

        AkSoundEngine.InitCommunication(AkPluginSettingsContainer.ActivePlatformSettings.AkCommunicationSettings);

        var basePathToSet = AkUtilities.GetWiseBankFolder_Full();
        var basePathToSet_decode = AkUtilities.GetWiseBankFolder_Full_Decode();
        if (string.IsNullOrEmpty(basePathToSet))
        {
            UnityEngine.Debug.LogError("WwiseUnity: Couldn't find soundbanks base path. Terminating sound engine.");
            AkSoundEngine.Term();
            return false;
        }

        if (AkSoundEngine.SetBasePath(basePathToSet) != AKRESULT.AK_Success)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogError("WwiseUnity: Failed to set soundbanks base path to <" + basePathToSet + ">. Make sure soundbank path is correctly set under Edit > Wwise Setting... > Asset Management.");
#else
			UnityEngine.Debug.LogError("WwiseUnity: Failed to set soundbanks base path to <" + basePathToSet + ">. Make sure soundbank path is correctly set under Edit > Project Settings > Wwise Initialization Settings.");
#endif
            AkSoundEngine.Term();
            return false;
        }

        if (!string.IsNullOrEmpty(basePathToSet_decode))
        {
            // AkSoundEngine.SetDecodedBankPath creates the folders for writing to (if they don't exist)
            AkSoundEngine.SetDecodedBankPath(basePathToSet_decode);
        }

        AkSoundEngine.SetCurrentLanguage(AkPluginSettingsContainer.ActivePlatformSettings.InitialLanguage);

#if !UNITY_SWITCH
        // Calling Application.persistentDataPath crashes Switch
        AkSoundEngine.AddBasePath(UnityEngine.Application.persistentDataPath + System.IO.Path.DirectorySeparatorChar);
#endif

        if (!string.IsNullOrEmpty(basePathToSet_decode))
        {
            // Adding decoded bank path last to ensure that it is the first one used when writing decoded banks.
            AkSoundEngine.AddBasePath(basePathToSet_decode);
        }

        AkCallbackManager.Init(AkPluginSettingsContainer.ActivePlatformSettings.CallbackManagerInitializationSettings);
        UnityEngine.Debug.Log("WwiseUnity: Sound engine initialized successfully.");
        LoadInitBank();
        return true;
    }

    public static bool ResetSoundEngine(bool isPlaying)
    {
        if (isPlaying)
        {
            AkSoundEngine.ClearBanks();
            LoadInitBank();
        }

        AkCallbackManager.Init(AkPluginSettingsContainer.ActivePlatformSettings.CallbackManagerInitializationSettings);
        return true;
    }

    private static void LoadInitBank()
    {
        AkBankManager.Reset();

        uint BankID;
        var result = AkSoundEngine.LoadBank("Init.bnk", AkSoundEngine.AK_DEFAULT_POOL_ID, out BankID);
        if (result != AKRESULT.AK_Success)
            UnityEngine.Debug.LogError("WwiseUnity: Failed load Init.bnk with result: " + result);
    }

    public static void TerminateSoundEngine()
    {
        if (!AkSoundEngine.IsInitialized())
            return;

        // Stop everything, and make sure the callback buffer is empty. We try emptying as much as possible, and wait 10 ms before retrying.
        // Callbacks can take a long time to be posted after the call to RenderAudio().
        AkSoundEngine.StopAll();
        AkSoundEngine.ClearBanks();
        AkSoundEngine.RenderAudio();

        for (var retry = 0; retry < 5;)
        {
            if (AkCallbackManager.PostCallbacks() == 0)
            {
                SleepForMilliseconds(10);
                ++retry;
            }

            SleepForMilliseconds(1);
        }

        AkSoundEngine.Term();

        // Make sure we have no callbacks left after Term. Some might be posted during termination.
        AkCallbackManager.PostCallbacks();
        AkCallbackManager.Term();
        AkBankManager.Reset();

#if UNITY_EDITOR
        AkPluginSettingsContainer.Instance.ActiveSettingsHash = string.Empty;
        AkPluginSettingsContainer.Instance.ActiveSettingsHaveChanged = true;
#endif
    }

    private static void SleepForMilliseconds(double milliseconds)
    {
        using (var tmpEvent = new System.Threading.ManualResetEvent(false))
            tmpEvent.WaitOne(System.TimeSpan.FromMilliseconds(milliseconds));
    }
    #endregion



}

