using UnityEditor;

namespace AQUAS
{
	public class AQUAS_RemoveDefine : UnityEditor.AssetModificationProcessor {

		static string symbols;

		public static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions rao)
		{

			if (assetPath.Contains ("AQUAS")) 
			{
				symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
			}
           if (symbols == null) return AssetDeleteResult.DidNotDelete;

			if (symbols.Contains("AQUAS_PRESENT"))
			{
				symbols = symbols.Replace("AQUAS_PRESENT;", "");
				symbols = symbols.Replace("AQUAS_PRESENT", "");
				PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
			}

			return AssetDeleteResult.DidNotDelete;
		}
	}
}
