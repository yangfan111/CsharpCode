using System;
using UnityEngine;

namespace JesseStiller.TerrainFormerExtension {
    internal class RaiseOrLowerCommand : TerrainCommand {
        
        internal override string GetName() {
            return "Raise/Lower";
        }

        protected override bool GetUsesShift() {
            return true;
        }

        protected override bool GetUsesControl() {
            return true;
        }

        internal RaiseOrLowerCommand(float[,] brushSamples) : base(brushSamples) { }

        internal override void OnClick(object data) {
            TerrainJobData terrainJobData = (TerrainJobData)data;
            
            float brushSample;
            for(int x = 0; x < commandArea.width; x++) {
                for(int y = terrainJobData.yStart; y < terrainJobData.yEnd; y++) {
                    brushSample = brushSamples[x + commandArea.clippedLeft, y + commandArea.clippedBottom];
                    if(brushSample == 0f) continue;
                    
                    TerrainFormerEditor.allTerrainHeights[y + commandArea.bottomOffset, x + commandArea.leftOffset] = 
                        Math.Min(TerrainFormerEditor.allTerrainHeights[y + commandArea.bottomOffset, x + commandArea.leftOffset] + brushSample * 0.01f, 1f);
                }
            }

            terrainJobData.manualResetEvent.Set();
        }
        
        protected override void OnControlClick(object data) {
            TerrainJobData terrainJobData = (TerrainJobData)data;

            int globalTerrainX, globalTerrainY;
            float brushSample;
            for(int x = 0; x < commandArea.width; x++) {
                for(int y = terrainJobData.yStart; y < terrainJobData.yEnd; y++) {
                    brushSample = brushSamples[x + commandArea.clippedLeft, y + commandArea.clippedBottom];
                    if(brushSample == 0f) continue;

                    globalTerrainX = x + commandArea.leftOffset;
                    globalTerrainY = y + commandArea.bottomOffset;
                    
                    TerrainFormerEditor.allTerrainHeights[globalTerrainY, globalTerrainX] = 
                        Mathf.Clamp01(TerrainFormerEditor.allUnmodifiedTerrainHeights[globalTerrainY, globalTerrainX] + 
                        brushSample * -TerrainFormerEditor.Instance.currentTotalMouseDelta * 0.003f);
                }
            }

            terrainJobData.manualResetEvent.Set();
        }
        
        protected override void OnShiftClick(object data) {
            TerrainJobData terrainJobData = (TerrainJobData)data;

            float brushSample;
            for(int x = 0; x < commandArea.width; x++) {
                for(int y = terrainJobData.yStart; y < terrainJobData.yEnd; y++) {
                    brushSample = brushSamples[x + commandArea.clippedLeft, y + commandArea.clippedBottom];
                    if(brushSample == 0f) continue;

                    TerrainFormerEditor.allTerrainHeights[y + commandArea.bottomOffset, x + commandArea.leftOffset] = 
                        Math.Max(TerrainFormerEditor.allTerrainHeights[y + commandArea.bottomOffset, x + commandArea.leftOffset] - brushSample * 0.01f, 0f);
                }
            }

            terrainJobData.manualResetEvent.Set();
        }

        protected override void OnShiftClickDown() { }
    }
}
