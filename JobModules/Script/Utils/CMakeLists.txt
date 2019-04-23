
set(Build_Dir "${Export_Dir}/${Dll_Export}")
set(Proj_Name "Utils")
set(Output_Name "Utils")
set(RootNamespace "Utils")

include("../cmake_common_lib.txt")

SET_TARGET_PROPERTIES(${Proj_Name} PROPERTIES 
							   VS_GLOBAL_AllowUnsafeBlocks ON)

target_link_libraries(${Proj_Name} LiteNetLib Common Shared.Scripts XmlConfig AssetBundleManager)

set_property(TARGET ${Proj_Name} PROPERTY VS_DOTNET_REFERENCES
	"System"
	"System.Core"
	"System.Data"
	"System.Data.DataSetExtensions"
	"System.Runtime.Serialization"
	"System.Runtime.Serialization.Formatters.Soap"
	"System.Xml"
	"System.Xml.Linq"
	"${Root_Dir}../UnityPackages/CommonLibs/Entitas.dll"
	"${Root_Dir}../UnityPackages/CommonLibs/log4net.dll"
	"${Root_Dir}../UnityPackages/ThirdParty/Unity/${UNITY_VERSION}/UnityEditor.dll"
	"${Root_Dir}../UnityPackages/ThirdParty/Unity/${UNITY_VERSION}/UnityEngine.dll")
