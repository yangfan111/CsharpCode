using System.Collections.Generic;	
namespace YF
{
   
 

    [System.Serializable]
    public class WwiseSettings
    {
        public const string WwiseSettingsFilename = "WwiseSettings.xml";

        //private static WwiseSettings s_Instance;
        public bool CopySoundBanksAsPreBuildStep = true;
        public bool CreatedPicker = false;
        public bool CreateWwiseGlobal = true;
        public bool CreateWwiseListener = true;
        public bool GenerateSoundBanksAsPreBuildStep = false;
        public bool ShowMissingRigidBodyWarning = true;
        public string SoundbankPath;
        public string WwiseInstallationPathMac;
        public string WwiseInstallationPathWindows;
        public string WwiseProjectPath;

        // Save the WwiseSettings structure to a serialized XML file
        public static void SaveSettings(WwiseSettings Settings) { }


        // Load the WwiseSettings structure from a serialized XML file
        public static WwiseSettings LoadSettings(bool ForceLoad = false)
        { return null; }

    }
}
