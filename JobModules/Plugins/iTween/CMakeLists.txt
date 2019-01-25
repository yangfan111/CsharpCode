
set(Build_Dir "${Export_Dir}/${Dll_Export}")
set(Proj_Name "iTween")
set(Output_Name "iTween")
set(RootNamespace "iTween")

include("../cmake_common_lib.txt")

set_property(TARGET ${Proj_Name} PROPERTY VS_DOTNET_REFERENCES
	"System"
	"System.Core"
	"System.Data"
	"System.Data.DataSetExtensions"
	"System.Xml"
	"System.Xml.Linq"
	"${Root_Dir}../UnityPackages/ThirdParty/Unity/${UNITY_VERSION}/UnityEngine.dll")
