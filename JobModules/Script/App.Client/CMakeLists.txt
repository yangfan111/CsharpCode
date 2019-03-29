
set(Build_Dir "${Export_Dir}/${Dll_Export}")
set(Proj_Name "App.Client")
set(Output_Name "App.Client")
set(RootNamespace "App.Client")

include("../cmake_common_lib.txt")

target_link_libraries(${Proj_Name} 
	App.ProtoBuf
	AssetBundleManager
	App.Shared
	App.Shared.Components
	App.Localization
	Core
	iTween
	Tools
	UIComponent
	UiFramework
	UITween
	UserInputManager
	XmlConfig
	I2Localization
	Fracturing)

set_property(TARGET ${Proj_Name} PROPERTY VS_DOTNET_REFERENCES
	"System"
	"System.Core"
	"System.Data"
	"System.Data.DataSetExtensions"
	"System.Xml"
	"System.Xml.Linq"
	"${Root_Dir}CommonLibs/protobuf3.dll"
	"${Root_Dir}ThirdParty/Unity/${UNITY_VERSION}/UnityEngine.dll"
	"${Root_Dir}ThirdParty/Unity/${UNITY_VERSION}/UnityEngine.UI.dll"
	"${Root_Dir}ThirdParty/Unity/${UNITY_VERSION}/UnityEditor.dll"
	"${Root_Dir}CommonLibs/DOTween/DOTween.dll"
	"${Root_Dir}/../UnityPackages/BehaviorDesigner/Runtime/BehaviorDesignerRuntime.dll"
	"${Root_Dir}UiFramework/Libs/Loxodon/Loxodon.Framework.dll"
	"${Root_Dir}UiFramework/Libs/Loxodon/Loxodon.Framework.Binding.dll"
	"${Root_Dir}UiFramework/Libs/Loxodon/Loxodon.Framework.Localizations.dll"
	"${Root_Dir}UiFramework/Libs/Loxodon/Loxodon.Log.dll"
	"${Root_Dir}CommonLibs/Entitas.dll"
	"${Root_Dir}CommonLibs/Entitas.CodeGeneration.Attributes.dll"
	"${Root_Dir}CommonLibs/Entitas.Utils.dll"
	"${Root_Dir}CommonLibs/log4net.dll"
	"${Root_Dir}CommonLibs/Entitas.VisualDebugging.Unity.dll")
