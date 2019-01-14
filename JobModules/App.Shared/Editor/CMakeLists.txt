
set(Root_Dir "../${Root_Dir}")
set(Build_Dir "../${Export_Dir}/${Dll_Export}")
set(Proj_Name "App.Shared.Editor")
set(Output_Name "App.Shared.Editor")
set(RootNamespace "App.Shared.Editor")

include("../../cmake_common_lib.txt")

target_link_libraries(${Proj_Name} 
	App.ProtoBuf
	App.Shared.Components
	AssetBundleManager
	Core
	Utils
	EVP5
	UserInputManager
	XmlConfig)

set_property(TARGET ${Proj_Name} PROPERTY VS_DOTNET_REFERENCES
	"System"
	"System.Core"
	"System.Data"
	"System.Data.DataSetExtensions"
	"System.Runtime.Serialization"
	"System.Xml"
	"System.Xml.Linq"
	"${Root_Dir}CommonLibs/Entitas.dll"
	"${Root_Dir}CommonLibs/Entitas.CodeGeneration.Attributes.dll"
	"${Root_Dir}CommonLibs/Entitas.Utils.dll"
	"${Root_Dir}CommonLibs/log4net.dll"
	"${Root_Dir}CommonLibs/protobuf3.dll"
	"${Root_Dir}ThirdParty/Unity/${UNITY_VERSION}/UnityEngine.dll"
	"${Root_Dir}ThirdParty/Unity/${UNITY_VERSION}/UnityEditor.dll"
	"${Root_Dir}CommonLibs/Entitas.VisualDebugging.Unity.dll")
