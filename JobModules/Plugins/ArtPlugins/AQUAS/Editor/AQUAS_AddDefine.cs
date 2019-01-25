using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AQUAS
{
	[InitializeOnLoad]
	public class AQUAS_AddDefine : Editor {

		static AQUAS_AddDefine()
		{
			var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

			if (!symbols.Contains("AQUAS_PRESENT"))
			{
				symbols += ";" + "AQUAS_PRESENT";
				PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
			}
		}
	}
}
