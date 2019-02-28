public class AkMacSettings : AkPluginSettingsContainer.CommonPlatformSettings
{
#if UNITY_EDITOR
	[UnityEditor.InitializeOnLoadMethod]
	private static void AutomaticPlatformRegistration()
	{
		RegisterPlatformSettingsClass<AkMacSettings>("Mac");
	}
#endif // UNITY_EDITOR
}
