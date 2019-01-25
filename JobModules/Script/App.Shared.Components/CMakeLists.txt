#project("App.Shared.Components" CSharp)

##########################
# App.Shared.Components.EntitasCodeGen.csproj
##########################

if(${INCLUDE_CODE_GEN} MATCHES True)
	set(Build_Dir "${Export_Dir}/${Dll_Temp}")
	set(Proj_Name "App.Shared.Components.EntitasCodeGen")
	set(Output_Name "App.Shared.Components.EntitasCodeGen")
	set(RootNamespace "App.Shared")

	list(APPEND Ignore_List "Generated")

	include("../cmake_common_lib.txt")

	set(SCRIPT_Dir ${CMAKE_CURRENT_SOURCE_DIR})

	set_property(TARGET ${Proj_Name} PROPERTY VS_GLOBAL_PostBuildEvent "mkdir \"${SCRIPT_Dir}/Generated/\" \n mkdir \"${SCRIPT_Dir}/Generated/Serializer\" \n mkdir \"${SCRIPT_Dir}/Generated/Entitas\" \n\n call ${SCRIPT_Dir}/generate_components.bat\necho =============================\ncall ${SCRIPT_Dir}/generate_serializer.bat\n")
	SET_TARGET_PROPERTIES(${Proj_Name} PROPERTIES VS_GLOBAL_RunPostBuildEvent "OnOutputUpdated")


	target_link_libraries(${Proj_Name} 
		Core
		Utils
		UserInputManager
		XmlConfig
		App.Shared.Entity)

	set_property(TARGET ${Proj_Name} PROPERTY VS_DOTNET_REFERENCES
		"System"
		"System.Core"
		"System.Data"
		"System.Data.DataSetExtensions"
		"System.Xml"
		"System.Xml.Linq"
		"${Root_Dir}CommonLibs/Entitas.dll"
		"${Root_Dir}CommonLibs/Entitas.CodeGeneration.Attributes.dll"
		"${Root_Dir}CommonLibs/Entitas.Utils.dll"
		"${Root_Dir}CommonLibs/Entitas.VisualDebugging.Unity.dll"
		"${Root_Dir}ThirdParty/Unity/${UNITY_VERSION}/UnityEngine.dll")
endif()	
##########################
# App.Shared.Components.csproj
##########################

set(Build_Dir "${Export_Dir}/${Dll_Export}")
set(Proj_Name "App.Shared.Components")
set(Output_Name "App.Shared.Components")
set(RootNamespace "App.Shared")

list(REMOVE_ITEM Ignore_List "Generated")

include("../cmake_common_lib.txt")

target_link_libraries(${Proj_Name} 
	App.ProtoBuf
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
	"${Root_Dir}CommonLibs/Entitas.VisualDebugging.Unity.dll"
	"${Root_Dir}ThirdParty/Unity/${UNITY_VERSION}/UnityEngine.dll")
