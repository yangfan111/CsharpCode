using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace JesseStiller.TerrainFormerExtension {
    internal static class TerrainMismatchManager {
        private static List<TerrainInformation> cachedTerrainInformations;
        private static bool heightmapResolutionsAreIdentical;
        private static int heightmapResolution = 1025;
        
        private static int[] terrainIndexesWithSplatPrototypes;
        private static string[] terrainNamesWithSplatPrototypes;
        private static bool splatPrototypesAreIdentical;
        private static int splatPrototypesIndex = -1;
        private static Texture2D[] splatPrototypePreviews;

        internal static bool IsInitialized { get; private set; }
        internal static bool IsMismatched { get; private set; }
        
        internal static void Initialize(List<TerrainInformation> terrainInformations) {
            if(IsMismatched) return;

            cachedTerrainInformations = terrainInformations;
            IsMismatched = false;
            splatPrototypesAreIdentical = true;
            heightmapResolutionsAreIdentical = true;
            
            TerrainData firstTerrainData = cachedTerrainInformations[0].terrainData;
            heightmapResolution = firstTerrainData.heightmapResolution;
            SplatPrototype[] otherSplatPrototypes = firstTerrainData.splatPrototypes;
            
            for(int i = 1; i < cachedTerrainInformations.Count; i++) {
                firstTerrainData = cachedTerrainInformations[i].terrainData;
                // Heightmap Resolution check
                if(heightmapResolutionsAreIdentical && heightmapResolution != firstTerrainData.heightmapResolution) {
                    SetMismatch(ref heightmapResolutionsAreIdentical);
                }
                
                // Splat Prototypes check
                if(splatPrototypesAreIdentical) {
                    if(otherSplatPrototypes.Length != firstTerrainData.splatPrototypes.Length) {
                        SetMismatch(ref splatPrototypesAreIdentical);
                    } else {
                        // TODO: Mismatches with splat prototype parameters should have its own dialog
                        for(int s = 0; s < otherSplatPrototypes.Length; s++) {
                            if(otherSplatPrototypes[s].metallic != firstTerrainData.splatPrototypes[s].metallic || otherSplatPrototypes[s].normalMap != firstTerrainData.splatPrototypes[s].normalMap ||
                                otherSplatPrototypes[s].specular != firstTerrainData.splatPrototypes[s].specular || otherSplatPrototypes[s].texture != firstTerrainData.splatPrototypes[s].texture || 
                                otherSplatPrototypes[s].tileOffset != firstTerrainData.splatPrototypes[s].tileOffset || otherSplatPrototypes[s].tileSize != firstTerrainData.splatPrototypes[s].tileSize
                                || otherSplatPrototypes[s].smoothness != firstTerrainData.splatPrototypes[s].smoothness) {
                                SetMismatch(ref splatPrototypesAreIdentical);
                                break;
                            }
                        }
                    }
                }
            }
            
            List<string> terrainNamesWithSplatPrototypesList = new List<string>();
            List<int> terrainIndexesWithSplatPrototypesList = new List<int>();
            for(int i = 0; i < cachedTerrainInformations.Count; i++) {
                string terrainName = cachedTerrainInformations[i].terrain.name;
                firstTerrainData = cachedTerrainInformations[i].terrainData;
                if(firstTerrainData.splatPrototypes.Length != 0) {
                    terrainIndexesWithSplatPrototypesList.Add(i);
                    terrainNamesWithSplatPrototypesList.Add(terrainName);

                    if(splatPrototypesIndex == -1) {
                        splatPrototypesIndex = i;
                    }
                }
            }

            terrainNamesWithSplatPrototypes = terrainNamesWithSplatPrototypesList.ToArray();
            terrainIndexesWithSplatPrototypes = terrainIndexesWithSplatPrototypesList.ToArray();
            
            UpdateSplatPrototypesPreviews();

            IsInitialized = true;
        }

        private static void SetMismatch(ref bool paramater) {
            paramater = false;
            IsMismatched = true;
        }

        private static void UpdateSplatPrototypesPreviews() {
            if(splatPrototypesIndex == -1) return;

            splatPrototypePreviews = new Texture2D[cachedTerrainInformations[splatPrototypesIndex].terrainData.splatPrototypes.Length];
            Texture2D splatTexture;
            for(int i = 0; i < splatPrototypePreviews.Length; i++) {
                splatTexture = cachedTerrainInformations[splatPrototypesIndex].terrainData.splatPrototypes[i].texture;
                splatPrototypePreviews[i] = AssetPreview.GetAssetPreview(splatTexture) ?? splatTexture;
            }
        }
        
        internal static void Draw() {
            if(IsMismatched == false) return;
            GUIUtilities.ActionableHelpBox("There are differences between the terrains in the current terrain grid which must be fixed before sculpting and painting is allowed.", MessageType.Warning, 
                () => {
                    EditorGUILayout.LabelField("Terrain Grid Settings", EditorStyles.boldLabel);
                    if(heightmapResolutionsAreIdentical == false) {
                        heightmapResolution = EditorGUILayout.IntPopup(TerrainSettings.heightmapResolutionContent, heightmapResolution, TerrainSettings.heightmapResolutionsContents, TerrainSettings.heightmapResolutions);
                    }
                    
                    if(splatPrototypesAreIdentical == false) {
                        int newIndex = EditorGUILayout.IntPopup("Splat Prototypes", splatPrototypesIndex, terrainNamesWithSplatPrototypes, terrainIndexesWithSplatPrototypes);
                        if(newIndex != splatPrototypesIndex) {
                            splatPrototypesIndex = newIndex;
                            UpdateSplatPrototypesPreviews();
                        }
                        DrawPreviewGrid(splatPrototypePreviews);
                    }
                    EditorGUILayout.Space();
                    if(GUILayout.Button("Apply to Terrain Grid")) {
                        Apply();
                    }
                }
            );
        }
        
        private static void Apply() {
            List<TerrainData> allModifiedTerrainDatas = new List<TerrainData>();
            for(int i = 0; i < cachedTerrainInformations.Count; i++) {
                if(cachedTerrainInformations[i].terrainData.heightmapResolution == heightmapResolution && 
                    splatPrototypesAreIdentical) continue;
                allModifiedTerrainDatas.Add(cachedTerrainInformations[i].terrainData);
            }
            Undo.RegisterCompleteObjectUndo(allModifiedTerrainDatas.ToArray(), "Fixed terrain grid settings mismatch");

            Vector3 originalSize = cachedTerrainInformations[0].terrainData.size;

            for(int i = 0; i < allModifiedTerrainDatas.Count; i++) {
                if(heightmapResolutionsAreIdentical == false) {
                    allModifiedTerrainDatas[i].heightmapResolution = heightmapResolution;
                    allModifiedTerrainDatas[i].size = originalSize; // Unity changes the size if the heightmapResolution has changed
                }
                if(splatPrototypesAreIdentical == false) {
                    allModifiedTerrainDatas[i].splatPrototypes = allModifiedTerrainDatas[splatPrototypesIndex].splatPrototypes;
                }
            }
            
            IsMismatched = false;
            splatPrototypesAreIdentical = true;
            heightmapResolutionsAreIdentical = true;

            TerrainFormerEditor.Instance.OnEnable();
        }

        private static void DrawPreviewGrid(Texture2D[] previews) {
            float size = 70f;
            int columnsPerRow = Mathf.FloorToInt((Screen.width - 30f) / size);
            int rows = Math.Max(Mathf.CeilToInt((float)previews.Length / columnsPerRow), 1);
            int currentRow = 0;
            int currentColumn = 0;
            GUI.BeginGroup(GUILayoutUtility.GetRect(Screen.width - 42f, rows * size), GUI.skin.box);
            for(int i = 0; i < previews.Length; i++) {
                GUI.DrawTexture(new Rect(currentColumn * 67f + 3f, currentRow * 64f + 3f, 64f, 64f), previews[i]);
                if(++currentColumn >= columnsPerRow) {
                    currentColumn = 0;
                    currentRow++;
                }
            }
            GUI.EndGroup();
        }
    }
}
