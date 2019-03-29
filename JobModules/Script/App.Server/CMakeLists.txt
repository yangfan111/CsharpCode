
set(Build_Dir "${Export_Dir}/${Dll_Export}")
set(Proj_Name "App.Server")
set(Output_Name "App.Server")
set(RootNamespace "App.Server")

include("../cmake_common_lib.txt")

SET_TARGET_PROPERTIES(${Proj_Name} PROPERTIES 
                 VS_GLOBAL_AllowUnsafeBlocks ON)

target_link_libraries(${Proj_Name} 
	App.ProtoBuf
	App.Hall.ProtoBuf
	AssetBundleManager
	App.Shared
	App.Shared.Components
	Core
	Enetcs
	Tools
	XmlConfig
	RpcNetwork
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
	"${Root_Dir}ThirdParty/Unity/${UNITY_VERSION}/UnityEditor.dll"
	"${Root_Dir}CommonLibs/Entitas.dll"
	"${Root_Dir}CommonLibs/Entitas.CodeGeneration.Attributes.dll"
	"${Root_Dir}CommonLibs/Entitas.Utils.dll"
	"${Root_Dir}CommonLibs/log4net.dll"
    "${Root_Dir}/../UnityPackages/BehaviorDesigner/Runtime/BehaviorDesignerRuntime.dll"
        "${Root_Dir}CommonLibs/commons.dll"
        "${Root_Dir}CommonLibs/I18N.dll"
        "${Root_Dir}CommonLibs/I18N.West.dll"
	"${Root_Dir}CommonLibs/Mysql.Data.dll"
        "${Root_Dir}CommonLibs/System.Data.dll"
	"${Root_Dir}CommonLibs/Entitas.VisualDebugging.Unity.dll"
	"${Root_Dir}CommonLibs/protobuf3.dll")
