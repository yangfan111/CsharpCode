
set(Build_Dir "${Export_Dir}/${Dll_Export}")
set(Proj_Name "App.Shared")
set(Output_Name "App.Shared")
set(RootNamespace "App.Shared")

if(${UNITY_EDITOR} MATCHES True)
else()
	list(APPEND Ignore_List "/Editor/")
endif(${UNITY_EDITOR} MATCHES True)

include("../cmake_common_lib.txt")

SET_TARGET_PROPERTIES(${Proj_Name} PROPERTIES 
                 VS_GLOBAL_AllowUnsafeBlocks ON)

target_link_libraries(${Proj_Name} 
	App.Hall.ProtoBuf
	App.ProtoBuf
	App.Shared.Components
	AssetBundleManager
	Core
	Utils
	Wise
	EVP5
	DWP
	BehaviorDesigner
	UserInputManager
	XmlConfig
	Shared.Scripts
	RpcNetwork
	Fracturing)

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
	"${Root_Dir}/../UnityPackages/BehaviorDesigner/Runtime/BehaviorDesignerRuntime.dll"
	"${Root_Dir}ThirdParty/Unity/${UNITY_VERSION}/UnityEngine.dll"
	"${Root_Dir}ThirdParty/Unity/${UNITY_VERSION}/UnityEditor.dll"
	"${Root_Dir}CommonLibs/Entitas.VisualDebugging.Unity.dll")
