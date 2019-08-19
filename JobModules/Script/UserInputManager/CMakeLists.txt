
set(Build_Dir "${Export_Dir}/${Dll_Export}")
set(Proj_Name "UserInputManager")
set(Output_Name "UserInputManager")
set(RootNamespace "UserInputManager")

include("../cmake_common_lib.txt")

set_property(TARGET ${Proj_Name} PROPERTY VS_DOTNET_REFERENCES
	"System"
	"System.Core"
	"System.Data"
	"System.Drawing"
	"System.Runtime.Serialization"
	"System.Xml"
	"System.Xml.Linq"
	"${Root_Dir}ThirdParty/Unity/${UNITY_VERSION}/UnityEditor.dll"
	"${Root_Dir}ThirdParty/Unity/${UNITY_VERSION}/UnityEngine.dll")
