
using System.IO;
using UnityEngine;

public partial class AkUtilities
{
#if UNITY_EDITOR
    public static string GetHashOfActiveSettingsField(string name, object obj)
    {
        string ret = string.Empty;

        var type = obj.GetType();
        if (type.IsPrimitive || type == typeof(string))
        {
            ret += name + ": " + obj.ToString() + "\n";
        }
        else
        {
            foreach (var subFieldInfo in type.GetFields(AkPluginSettingsContainer.BindingFlags))
            {
                var subObject = subFieldInfo.GetValue(obj);
                var subType = subFieldInfo.FieldType;
                var fields = subType.GetFields(AkPluginSettingsContainer.BindingFlags);

                if (fields.Length == 0)
                    ret += subFieldInfo.Name + ": " + subObject.ToString() + "\n";
                else
                    ret += GetHashOfActiveSettingsField(subFieldInfo.Name, subObject);
            }
        }

        return ret;
    }

    public static string GetHashOfActiveSettings()
    {
        try
        {
            return GetHashOfActiveSettingsField(string.Empty, AkPluginSettingsContainer.ActivePlatformSettings);
        }
        catch
        {
            return string.Empty;
        }
    }
#endif

    public static string Asset__WiseResSubFolder__FileExtPath_Relative(string fileName)
    {
        return System.IO.Path.Combine(WwiseInfluence.Asset__WiseResSubFolder_Relative, fileName + ".asset");
    }
#if UNITY_EDITOR
    public static T GetOrCreateWiseResAsset<T>(string className, string fileName) where T : AkPluginSettingsContainerAbstract
    {
        var path = AkUtilities.Asset__WiseResSubFolder__FileExtPath_Relative(fileName);
        var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
        if (!asset)
        {
            var fullPath = System.IO.Path.Combine(UnityEngine.Application.dataPath, WwiseInfluence.WiseResSubFolder_Relative);
            if (!System.IO.Directory.Exists(fullPath))
                System.IO.Directory.CreateDirectory(fullPath);

            asset = ScriptableObject.CreateInstance(className) as T;
            UnityEditor.AssetDatabase.CreateAsset(asset, path);
        }

        return asset;
    }
#endif
    public static string GetWiseBankFolder_Full()
    {
#if UNITY_EDITOR

        string path_unityEditor = System.IO.Path.Combine(Application.dataPath, WwiseInfluence.WiseBankRelativePath_Editor);
        FixSlashes(ref path_unityEditor);
        return path_unityEditor;
#else
                return Application.streamingAssetsPath;
          //return Application.dataPath + "/StreamingAssets";
#endif
    }
    public static string GetWiseBankFolder_Full_Decode()
    {
        return Path.Combine(GetWiseBankFolder_Full(), "Decode");
    }
    public static void FixSlashes(ref string path, char separatorChar, char badChar, bool addTrailingSlash)
    {
        if (string.IsNullOrEmpty(path))
            return;

        path = path.Trim().Replace(badChar, separatorChar).TrimStart('\\');

        // Append a trailing slash to play nicely with Wwise
        if (addTrailingSlash && !path.EndsWith(separatorChar.ToString()))
            path += separatorChar;
    }

    public static void FixSlashes(ref string path)
    {
#if UNITY_WSA
		var separatorChar = '\\';
#else
        var separatorChar = System.IO.Path.DirectorySeparatorChar;
#endif // UNITY_WSA
        var badChar = separatorChar == '\\' ? '/' : '\\';
        FixSlashes(ref path, separatorChar, badChar, true);
    }
    #if UNITY_EDITOR
	/// <summary>
	///     Determines the platform base path for use within the Editor.
	/// </summary>
	/// <param name="platformName">The platform name.</param>
	/// <returns>The full path to the sound banks for use within the Editor.</returns>
	public static string GetPlatformBasePathEditor(string platformName)
    {
        var Settings = WwiseSettings.LoadSettings();
        var WwiseProjectFullPath = AkUtilities.GetFullPath(UnityEngine.Application.dataPath, Settings.WwiseProjectPath);
        var SoundBankDest = AkUtilities.GetWwiseSoundBankDestinationFolder(platformName, WwiseProjectFullPath);

        try
        {
            if (System.IO.Path.GetPathRoot(SoundBankDest) == "")
            {
                // Path is relative, make it full
                SoundBankDest = AkUtilities.GetFullPath(System.IO.Path.GetDirectoryName(WwiseProjectFullPath), SoundBankDest);
            }
        }
        catch
        {
            SoundBankDest = string.Empty;
        }

        if (string.IsNullOrEmpty(SoundBankDest))
        {
            UnityEngine.Debug.LogWarning("WwiseUnity: The platform SoundBank subfolder within the Wwise project could not be found.");
        }
        else
        {
            try
            {
                // Verify if there are banks in there
                var di = new System.IO.DirectoryInfo(SoundBankDest);
                var foundBanks = di.GetFiles("*.bnk", System.IO.SearchOption.AllDirectories);
                if (foundBanks.Length == 0)
                    SoundBankDest = string.Empty;
                else if (!SoundBankDest.Contains(platformName))
                {
                    UnityEngine.Debug.LogWarning(
                        "WwiseUnity: The platform SoundBank subfolder does not match your platform name. You will need to create a custom platform name getter for your game. See section \"Using Wwise Custom Platforms in Unity\" of the Wwise Unity integration documentation for more information");
                }
            }
            catch
            {
                SoundBankDest = string.Empty;
            }
        }

        return SoundBankDest;
    }
#endif
}
