echo Running Components Serializer  code generator
echo %~dp0
cd %~dp0
del Generated\Serializer /f /q
mkdir Generated\Serializer
..\Dll.Temp\ComponentSerializerCodeGenerator ..\Dll.Temp\Core.dll 1 ..\Dll.Temp\App.Shared.Components.EntitasCodeGen.dll .\Tools\ComponentSerializerGenerator\codeTemplate\Serializer.stg App.Shared.Components.Serializer .\Generated\Serializer
