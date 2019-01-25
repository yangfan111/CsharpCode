
set(Build_Dir "${Export_Dir}/${Dll_Export}")
set(Proj_Name "Core")
set(Output_Name "Core")
set(RootNamespace "Core")

include("../cmake_common_lib.txt")

SET_TARGET_PROPERTIES(${Proj_Name} PROPERTIES 
							   VS_GLOBAL_AllowUnsafeBlocks ON)

target_link_libraries(${Proj_Name} XmlConfig Utils Wwise)

set_property(TARGET ${Proj_Name} PROPERTY VS_DOTNET_REFERENCES
	"System"
	"System.Core"
	"System.Data"
	"System.Data.DataSetExtensions"
	"System.Runtime.Serialization"
	"System.Runtime.Serialization.Formatters.Soap"
	"System.Xml"
	"System.Xml.Linq"
	"${Root_Dir}CommonLibs/Entitas.dll"
	"${Root_Dir}CommonLibs/Entitas.CodeGeneration.Attributes.dll"
	"${Root_Dir}CommonLibs/Entitas.VisualDebugging.Unity.dll"
	"${Root_Dir}CommonLibs/Entitas.Utils.dll"
	"${Root_Dir}CommonLibs/log4net.dll"
	"${Root_Dir}ThirdParty/Unity/${UNITY_VERSION}/UnityEditor.dll"
	"${Root_Dir}ThirdParty/Unity/${UNITY_VERSION}/UnityEngine.dll"
	"${Root_Dir}ThirdParty/RefCounterAnalyzer/RefCounterAnalyzer.dll")
