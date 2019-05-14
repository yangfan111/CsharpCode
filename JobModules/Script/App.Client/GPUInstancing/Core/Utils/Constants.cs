using UnityEngine;

namespace App.Client.GPUInstancing.Core.Utils
{
    static class Constants
    {
        public const int DefaultDetailNodeSize = 32;
        // in meters
        public const float DetailDisableBufferLength = 50;

        public const int StrideSizeFloat = 4;
        public const int StrideSizeFloat4 = StrideSizeFloat * 4;
        public const int StrideSizeInt = 4;
        public const int StrideSizeUint = 4;
        public const int StrideSizeInt4 = StrideSizeInt * 4;
        public const int StrideSizeMatrix4x4 = 64;
        public const int StrideSizeFloat3 = StrideSizeFloat * 3;

        public const int DetailInstantiationThreadCount = 32;
        public const int MergeThreadCount = 1024;
        public const int VisibilityThreadCount = 1024;

        public static class CsPath
        {
            public const string DetailInstantiationInResource = "Compute/DetailInstantiation";
            public const string MergeBufferInResource = "Compute/MergeBuffer";
            public const string VisibilityDeterminationInResource = "Compute/VisibilityDetermination";
        }

        public static class CsKernel
        {
            public const string Common = "CSMain";
        }

        public static class MergeVariable
        {
            public const string MergeFloat4x4 = "MergeFloat4x4";
            public static readonly int InputFloat4x4 = Shader.PropertyToID("InputFloat4x4");
            public static readonly int OutputFloat4x4 = Shader.PropertyToID("OutputFloat4x4");

            public const string MergeFloat3 = "MergeFloat3";
            public static readonly int InputFloat3 = Shader.PropertyToID("InputFloat3");
            public static readonly int OutputFloat3 = Shader.PropertyToID("OutputFloat3");
        }

        public static class ShaderVariable
        {
            public static readonly int InputData = Shader.PropertyToID("InputData");
            public static readonly int InputDataCount = Shader.PropertyToID("InputDataCount");
            public static readonly int OutputData = Shader.PropertyToID("OutputData");
            public static readonly int OutputDataOffset = Shader.PropertyToID("OutputDataOffset");
            public static readonly int CounterData = Shader.PropertyToID("CounterData");

            public static readonly int BlockCount = Shader.PropertyToID("BlockCount");
            public static readonly int BlockSize = Shader.PropertyToID("BlockSize");
            public static readonly int RealCountInBlockData = Shader.PropertyToID("RealCountInBlockData");

            public static readonly int CullingDistance = Shader.PropertyToID("CullingDistance");
            public static readonly int WorldShift = Shader.PropertyToID("WorldShift");
        }

        public static class CameraVariable
        {
            public static readonly int TanHalfFov = Shader.PropertyToID("TanHalfFov");
            public static readonly int CameraWorldPosition = Shader.PropertyToID("CameraWorldPosition");
            public static readonly int CamLeftPlaneWorld = Shader.PropertyToID("CamLeftPlaneWorld");
            public static readonly int CamRightPlaneWorld = Shader.PropertyToID("CamRightPlaneWorld");
            public static readonly int CamTopPlaneWorld = Shader.PropertyToID("CamTopPlaneWorld");
            public static readonly int CamBottomPlaneWorld = Shader.PropertyToID("CamBottomPlaneWorld");
            public static readonly int CamFarPlaneWorld = Shader.PropertyToID("CamFarPlaneWorld");
            public static readonly int CamFarClipDistance = Shader.PropertyToID("CamFarClipDistance");
        }

        public static class MeshVariable
        {
            public static readonly int LodRatios = Shader.PropertyToID("LodRatios");
            public static readonly int LodSize = Shader.PropertyToID("LodSize");
            public static readonly int SphereCenter = Shader.PropertyToID("SphereCenter");
            public static readonly int SphereRadius = Shader.PropertyToID("SphereRadius");

            public static readonly int DrawInstanceData = Shader.PropertyToID("DrawInstanceData");
            public static readonly int DrawInstanceDataLod0 = Shader.PropertyToID("DrawInstanceDataLod0");
            public static readonly int DrawInstanceDataLod1 = Shader.PropertyToID("DrawInstanceDataLod1");
            public static readonly int DrawInstanceDataLod2 = Shader.PropertyToID("DrawInstanceDataLod2");
            public static readonly int DrawInstanceDataLod3 = Shader.PropertyToID("DrawInstanceDataLod3");
        }

        public static class TerrainVariable
        {
            public static readonly int TerrainSize = Shader.PropertyToID("TerrainSize");
            public static readonly int HeightMapData = Shader.PropertyToID("HeightMapData");
            public static readonly int HeightMapResolution = Shader.PropertyToID("HeightMapResolution");

            // UnityTerrain CBuffer 
            public static readonly int WavingTint = Shader.PropertyToID("_WavingTint");
            public static readonly int WaveAndDistance = Shader.PropertyToID("_WaveAndDistance");
            public static readonly int CameraPosition = Shader.PropertyToID("_CameraPosition");
            public static readonly int CameraRight = Shader.PropertyToID("_CameraRight");
            public static readonly int CameraUp = Shader.PropertyToID("_CameraUp");
            public static readonly int TreeInstanceColor = Shader.PropertyToID("_TreeInstanceColor");
            public static readonly int TreeInstanceScale = Shader.PropertyToID("_TreeInstanceScale");
            public static readonly int TerrainEngineBendTree = Shader.PropertyToID("_TerrainEngineBendTree");
            public static readonly int SquashPlaneNormal = Shader.PropertyToID("_SquashPlaneNormal");
            public static readonly int SquashAmount = Shader.PropertyToID("_SquashAmount");
            public static readonly int TreeBillboardCameraRight = Shader.PropertyToID("_TreeBillboardCameraRight");
            public static readonly int TreeBillboardCameraUp = Shader.PropertyToID("_TreeBillboardCameraUp");
            public static readonly int TreeBillboardCameraFront = Shader.PropertyToID("_TreeBillboardCameraFront");
            public static readonly int TreeBillboardCameraPos = Shader.PropertyToID("_TreeBillboardCameraPos");
            public static readonly int TreeBillboardDistances = Shader.PropertyToID("_TreeBillboardDistances");
        }

        public static class DetailVariable
        {
            public static readonly int DetailResolution = Shader.PropertyToID("DetailResolution");
            public static readonly int DetailResolutionPerPatch = Shader.PropertyToID("DetailResolutionPerPatch");
            public static readonly int DividedDetailData = Shader.PropertyToID("DividedDetailData");
            public static readonly int DividedDetailResolution = Shader.PropertyToID("DividedDetailResolution");
            public static readonly int DividedDetailIndex = Shader.PropertyToID("DividedDetailIndex");
            public static readonly int DetailDensity = Shader.PropertyToID("DetailDensity");
            public static readonly int DetailScale = Shader.PropertyToID("DetailScale");
            public static readonly int BasePosition = Shader.PropertyToID("BasePosition");
            public static readonly int DetailLayer = Shader.PropertyToID("DetailLayer");
            public static readonly int NoiseSpread = Shader.PropertyToID("NoiseSpread");
            public static readonly int HealthyColor = Shader.PropertyToID("HealthyColor");
            public static readonly int DryColor = Shader.PropertyToID("DryColor");

            public static readonly int TransformData = Shader.PropertyToID("TransformData");
            public static readonly int NormalData = Shader.PropertyToID("NormalData");
            public static readonly int ColorData = Shader.PropertyToID("ColorData");
            public static readonly int Cutoff = Shader.PropertyToID("_Cutoff");
        }

        public static class RandVariable
        {
            public static readonly int PerlinNoiseConstants = Shader.PropertyToID("PerlinNoiseConstants");
        }

        public static class Rendering
        {
            static Rendering()
            {
                QuadMesh = new Mesh
                {
                    vertices = new []
                    {
                        new Vector3(-0.5f, 0, 0),
                        new Vector3(-0.5f, 1, 0),
                        new Vector3(0.5f, 1, 0),
                        new Vector3(0.5f, 0, 0)
                    },
                    uv = new []
                    {
                        new Vector2(0, 0), 
                        new Vector2(0, 1), 
                        new Vector2(1, 1), 
                        new Vector2(1, 0), 
                    },
                    triangles = new []
                    {
                        0, 1, 2, 2, 3, 0
                    },
                    colors32 = new []
                    {
                        new Color32(160, 160, 160, 0), 
                        new Color32(255, 255, 255, 255), 
                        new Color32(255, 255, 255, 255), 
                        new Color32(160, 160, 160, 0), 
                    }
                };
                QuadMesh.RecalculateBounds();

                GrassMaterial = Resources.Load<Material>(GrassMaterialInResource);
            }

            public static readonly Mesh QuadMesh;
            private static readonly Material GrassMaterial;

            public static Material GetGrassMaterial()
            {
                return new Material(GrassMaterial);
            }

            public const string GrassMaterialInResource = "Materials/DefaultGrass";
            public static readonly int GrassMaterialMainTex = Shader.PropertyToID("_MainTex");
        }

        public static class Random
        {
            public static readonly int[] PerlinNoise =
            {
                151, 160, 137, 91, 90, 15,
                131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23,
                190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33,
                88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166,
                77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244,
                102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200, 196,
                135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123,
                5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42,
                223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9,
                129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228,
                251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107,
                49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254,
                138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180,
                151, 160, 137, 91, 90, 15,
                131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23,
                190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33,
                88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166,
                77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244,
                102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200, 196,
                135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123,
                5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42,
                223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9,
                129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228,
                251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107,
                49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254,
                138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180
            };

            public static ComputeBuffer GetPerlinNoiseBuffer()
            {
                var buffer = new ComputeBuffer(PerlinNoise.Length, StrideSizeInt);
                buffer.SetData(PerlinNoise);

                return buffer;
            }
        }
    }
}
