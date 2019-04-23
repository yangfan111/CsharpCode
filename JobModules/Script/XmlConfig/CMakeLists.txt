
set(Build_Dir "${Export_Dir}/${Dll_Export}")
set(Proj_Name "XmlConfig")
set(Output_Name "XmlConfig")
set(RootNamespace "XmlConfig")

include("../cmake_common_lib.txt")

target_link_libraries(${Proj_Name} 
	Shared.Scripts)
	
set_property(TARGET ${Proj_Name} PROPERTY VS_GLOBAL_PostBuildEvent "mkdir Weapon\nmkdir Input\ncall xsd $(TargetPath) /type:WeaponConfigNs.WeaponConfigs /outputdir:Weapon\ncall xsd $(TargetPath) /type:WeaponConfigNs.InputConfig /outputdir:Input\nexit 0")

set_property(TARGET ${Proj_Name} PROPERTY VS_DOTNET_REFERENCES
	"System"
	"System.Core"
	"System.Data"
	"System.Data.DataSetExtensions"
	"System.Runtime.Serialization"
	"System.Xml"
	"System.Xml.Linq"
	"${Root_Dir}/../UnityPackages/ThirdParty/Unity/${UNITY_VERSION}/UnityEngine.dll")
