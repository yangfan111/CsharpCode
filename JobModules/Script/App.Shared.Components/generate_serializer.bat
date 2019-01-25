echo Running Components Serializer  code generator
echo %~dp0
cd %~dp0
del Generated\Serializer\* /f /q
..\Dll.Temp\ComponentSerializerCodeGenerator ..\Dll.Export\Core.dll 1 ..\Dll.Temp\App.Shared.Components.EntitasCodeGen.dll .\Tools\ComponentSerializerGenerator\codeTemplate\Serializer.stg App.Shared.Components.Serializer .\Generated\Serializer
