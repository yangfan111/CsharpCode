
public interface IPluginSettingProperty
{

    AkInitializationSettings AkInitializationSettings { get; }

    AkSpatialAudioInitSettings AkSpatialAudioInitSettings { get; }

    AkCallbackManager.InitializationSettings CallbackManagerInitializationSettings { get; }

    AkCommunicationSettings AkCommunicationSettings { get; }

    string InitialLanguage { get; }

 //   string SoundbankPath { get; }

}

public static class WwiseInfluence
{
    public static readonly string WiseResSubFolder_Relative = System.IO.Path.Combine("Wwise", "Resources");
    public static readonly string Asset__WiseResSubFolder_Relative = System.IO.Path.Combine("Assets", WiseResSubFolder_Relative);
    public static readonly string WiseBankRelativePath_Editor = "Assets/Sound/WiseBank";
}