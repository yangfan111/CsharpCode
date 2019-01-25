using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace JesseStiller.TerrainFormerExtension {
    internal static class BrushCollection {
        internal const string defaultFalloffBrushId = "_DefaultFalloffBrushName";
        internal const string defaultPerlinNoiseBrushId = "_DefaultPerlinNoiseBrushName";

        internal static string absoluteCustomBrushPath;
        internal static string localCustomBrushPath;
        
        public static SortedDictionary<string, TerrainBrush> brushes;
        
        public static Dictionary<string, Type> terrainBrushTypes;

        private static bool initialized = false;

        private class TerrainBrushTypesInfo {
            internal int sortOrder;
            internal string prettyTypeName;
            internal Type type;

            internal TerrainBrushTypesInfo(int sortOrder, string prettyTypeName, Type type) {
                this.sortOrder = sortOrder;
                this.prettyTypeName = prettyTypeName;
                this.type = type;
            }
        }

        public static void Initilize() {
            if(initialized == true) return;
            
            absoluteCustomBrushPath = Path.Combine(Utilities.GetAbsolutePathFromLocalPath(TerrainFormerEditor.settings.mainDirectory), "Textures/Brushes");
            localCustomBrushPath = Utilities.GetLocalPathFromAbsolutePath(absoluteCustomBrushPath);
            
            brushes = new SortedDictionary<string, TerrainBrush>();
            
            brushes.Add(defaultFalloffBrushId, new FalloffBrush("Falloff Brush", defaultFalloffBrushId));
            brushes.Add(defaultPerlinNoiseBrushId, new PerlinNoiseBrush("Perlin Noise Brush", defaultPerlinNoiseBrushId));

            RefreshCustomBrushes();

            terrainBrushTypes = new Dictionary<string, Type>();
            terrainBrushTypes.Add("All", null);

            List<TerrainBrushTypesInfo> terrainBrushTypesInfo = new List<TerrainBrushTypesInfo>();

            Type[] allAssemblyTypes = typeof(TerrainFormerEditor).Assembly.GetTypes();
            // Gather all classes that derrive from TerrainBrush
            foreach(Type type in allAssemblyTypes) {
                if(type.IsSubclassOf(typeof(TerrainBrush)) == false) continue;

                BindingFlags nonPublicStaticBindingFlags = BindingFlags.NonPublic | BindingFlags.Static;
                FieldInfo prettyTypeNameFieldInfo = type.GetField("prettyTypeName", nonPublicStaticBindingFlags);
                string prettyTypeName = prettyTypeNameFieldInfo == null ? type.Name : (string)prettyTypeNameFieldInfo.GetValue(null);

                FieldInfo typeSortOrderFieldInfo = type.GetField("typeSortOrder", nonPublicStaticBindingFlags);
                int typeSortOrder = typeSortOrderFieldInfo == null ? 10 : (int)typeSortOrderFieldInfo.GetValue(null);

                terrainBrushTypesInfo.Add(new TerrainBrushTypesInfo(typeSortOrder, prettyTypeName, type));
            }

            terrainBrushTypesInfo.Sort(delegate (TerrainBrushTypesInfo x, TerrainBrushTypesInfo y) {
                if(x.sortOrder < y.sortOrder) return x.sortOrder;
                else return y.sortOrder;
            });

            foreach(TerrainBrushTypesInfo t in terrainBrushTypesInfo) {
                terrainBrushTypes.Add(t.prettyTypeName, t.type);
            }

            initialized = true;
        }
        
        // The parameter UpdatedBrushes requires local Unity assets paths
        internal static void RefreshCustomBrushes(string[] updatedBrushes = null) {
            // If there is no data on which brushes need to be updated, assume every brush must be updated
            if(updatedBrushes == null) {
                updatedBrushes = Directory.GetFiles(absoluteCustomBrushPath, "*", SearchOption.AllDirectories);

                for(int i = 0; i < updatedBrushes.Length; i++) {
                    updatedBrushes[i] = Utilities.GetLocalPathFromAbsolutePath(updatedBrushes[i]);
                }
            }
            
            // Get the custom brush textures
            foreach(string path in updatedBrushes) {
                if(path.EndsWith(".meta")) continue;
                
                Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if(tex == null) continue;
                
                TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(path);

#if UNITY_5_5_OR_NEWER
                if(textureImporter.isReadable == false || textureImporter.wrapMode != TextureWrapMode.Clamp || 
                    textureImporter.textureCompression != TextureImporterCompression.Uncompressed) {
#else
                if(textureImporter.textureType != TextureImporterType.Advanced || textureImporter.isReadable == false ||
                    textureImporter.wrapMode != TextureWrapMode.Clamp || textureImporter.textureFormat != TextureImporterFormat.AutomaticTruecolor) {
                    textureImporter.textureType = TextureImporterType.Advanced;
#endif

                    textureImporter.isReadable = true;
                    textureImporter.wrapMode = TextureWrapMode.Clamp;

#if UNITY_5_5_OR_NEWER
                    textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
#else
                    textureImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;
#endif

                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);

                    // Reload the texture with the updated settings
#if UNITY_5_3_OR_NEWER && !UNITY_5_3_0 && !UNITY_5_3_1 && !UNITY_5_3_2 && !UNITY_5_3_3 && !UNITY_5_3_4 // Unity 5.3.5 or newer 
                    tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
#else
                    tex = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
#endif
                }

                if(tex.width != tex.height) continue;
                
                string imageBasedTextureGUID = AssetDatabase.AssetPathToGUID(path);

                if(brushes.ContainsKey(imageBasedTextureGUID)) {
                    ImageBrush customBrush = brushes[imageBasedTextureGUID] as ImageBrush;
                    if(customBrush == null) continue;
                    customBrush.sourceTexture = tex;
                } else {
                    brushes.Add(imageBasedTextureGUID, new ImageBrush(tex.name, imageBasedTextureGUID, tex));
                }
            }
        }
        
        internal static void UpdatePreviewTextures() {
            foreach(TerrainBrush terrainBrush in brushes.Values) { 
                terrainBrush.CreatePreviewTexture();
            }
        }

        internal static void RemoveDeletedBrushes(string[] deletedBrushes) {
            foreach(string deletedBrush in deletedBrushes) {
                string deletedBrushId = AssetDatabase.AssetPathToGUID(deletedBrush);
                brushes.Remove(deletedBrushId);
            }
        }
    }
}
