
set(Root_Dir "../${Root_Dir}")
set(Build_Dir "../${Export_Dir}/${Dll_Export}")
set(Proj_Name "AssetBundleManager.Editor")
set(Output_Name "AssetBundleManager.Editor")
set(RootNamespace "AssetBundleManager.Editor")

include("../../cmake_common_lib.txt")

target_link_libraries(${Proj_Name} AssetBundleManager)

set_property(TARGET ${Proj_Name} PROPERTY VS_DOTNET_REFERENCES
	"System"
	"System.Core"
	"System.Data"
	"System.Data.DataSetExtensions"
	"System.Xml"
	"System.Xml.Linq"
	"${Root_Dir}../UnityPackages/CommonLibs/log4net.dll"
	"${Root_Dir}../UnityPackages/ThirdParty/Unity/${UNITY_VERSION}/UnityEngine.dll"
	"${Root_Dir}../UnityPackages/ThirdParty/Unity/${UNITY_VERSION}/UnityEditor.dll")
