
set(Build_Dir "${Export_Dir}/${Dll_Export}")
set(Proj_Name "AssetBundleManager")
set(Output_Name "AssetBundleManager")
set(RootNamespace "AssetBundleManager")

if(${UNITY_EDITOR} MATCHES True)
else()
	list(APPEND Ignore_List "/Editor/")
endif(${UNITY_EDITOR} MATCHES True)

include("../cmake_common_lib.txt")

target_link_libraries(${Proj_Name} 
	Common)


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
