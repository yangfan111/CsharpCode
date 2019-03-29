
set(Build_Dir "${Export_Dir}/${Dll_Export}")

#set(Share_Dir1 "${Root_Dir}../UnityPackages/Dll.Shared")
#set(Share_Dir2 "${Root_Dir}../HallUnityPackages/Dll.Hall.Shared")
#set(Share_Dir3 "${Root_Dir}../HallUnityPackages/Dll.Hall.Shared.Release")
set(Proj_Name "Wwise")
set(Output_Name "Wwise") 
set(RootNamespace "Wwise")

if(${UNITY_EDITOR} MATCHES True)
else()
	list(APPEND Ignore_List "/Editor/")
endif(${UNITY_EDITOR} MATCHES True)

include("../cmake_common_lib.txt")

set(Script_Dir ${CMAKE_CURRENT_SOURCE_DIR}  )
#add_custom_command(TARGET  ${Proj_Name}  POST_BUILD   COMMAND ${CMAKE_COMMAND} -E copy ${Script_Dir}/Deployment/Plugins/Windows/x86_64/Release/AKSoundEngine.dll.meta   ${Build_Dir}/Plugins/AKSoundEngine.dll.meta  COMMENT  "copy dll" )
#add_custom_command(TARGET  ${Proj_Name}  POST_BUILD   COMMAND ${CMAKE_COMMAND} -E copy ${Script_Dir}/Deployment/Plugins/Windows/x86_64/Release/AKSoundEngine.dll  ${Build_Dir}/Plugins/AKSoundEngine.dll COMMENT  "copy dll" )

#add_custom_command(TARGET  ${Proj_Name}  POST_BUILD   COMMAND ${CMAKE_COMMAND} -E copy ${Script_Dir}/Deployment/Plugins/Windows/x86_64/Release/AKSoundEngine.dll ${Share_Dir1}/AKSoundEngine.dll  COMMENT  "copy dll" )
#add_custom_command(TARGET  ${Proj_Name}  POST_BUILD   COMMAND ${CMAKE_COMMAND} -E copy ${Script_Dir}/Deployment/Plugins/Windows/x86_64/Release/AKSoundEngine.dll.meta  ${Share_Dir1}/AKSoundEngine.dll.meta  COMMENT  "copy dll" )

#add_custom_command(TARGET  ${Proj_Name}  POST_BUILD   COMMAND ${CMAKE_COMMAND} -E copy ${Script_Dir}/Deployment/Plugins/Windows/x86_64/Release/AKSoundEngine.dll ${Share_Dir2}/AKSoundEngine.dll  COMMENT  "copy dll" )
#add_custom_command(TARGET  ${Proj_Name}  POST_BUILD   COMMAND ${CMAKE_COMMAND} -E copy ${Script_Dir}/Deployment/Plugins/Windows/x86_64/Release/AKSoundEngine.dll.meta  ${Share_Dir2}/AKSoundEngine.dll.meta  COMMENT  "copy dll" )

#add_custom_command(TARGET  ${Proj_Name}  POST_BUILD   COMMAND ${CMAKE_COMMAND} -E copy ${Script_Dir}/Deployment/Plugins/Windows/x86_64/Release/AKSoundEngine.dll ${Share_Dir3}/AKSoundEngine.dll  COMMENT  "copy dll" )
#add_custom_command(TARGET  ${Proj_Name}  POST_BUILD   COMMAND ${CMAKE_COMMAND} -E copy ${Script_Dir}/Deployment/Plugins/Windows/x86_64/Release/AKSoundEngine.dll.meta  ${Share_Dir3}/AKSoundEngine.dll.meta  COMMENT  "copy dll" )

set_property(TARGET ${Proj_Name} PROPERTY VS_DOTNET_REFERENCES
	"System"
	"System.Core"
	"System.Data"
	"System.Data.DataSetExtensions"
	"System.Xml"
	"System.Xml.Linq"
	"${Root_Dir}../UnityPackages/ThirdParty/Unity/${UNITY_VERSION}/UnityEngine.dll"
	"${Root_Dir}../UnityPackages/ThirdParty/Unity/${UNITY_VERSION}/UnityEditor.dll")

