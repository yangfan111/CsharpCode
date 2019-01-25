using UnityEngine;

namespace JesseStiller.TerrainFormerExtension {
    internal class SetHeightCommand : TerrainCommand {
        internal float normalizedHeight;
        
        internal override string GetName() {
            return "Set Height";
        }

        protected override bool GetUsesShift() {
            return false;
        }

        protected override bool GetUsesControl() {
            return true;
        }

        internal SetHeightCommand(float[,] brushSamples) : base(brushSamples) {
            normalizedHeight = TerrainFormerEditor.settings.setHeight / TerrainFormerEditor.Instance.terrainSize.y;
        }

        internal override void OnClick(object data) {
            TerrainJobData terrainJobData = (TerrainJobData)data;

            int globalTerrainX, globalTerrainY;
            float brushSample;
            for(int x = 0; x < commandArea.width; x++) {
                for(int y = terrainJobData.yStart; y < terrainJobData.yEnd; y++) {
                    brushSample = brushSamples[x + commandArea.clippedLeft, y + commandArea.clippedBottom];
                    if(brushSample == 0f) continue;

                    globalTerrainX = x + commandArea.leftOffset;
                    globalTerrainY = y + commandArea.bottomOffset;
                    
                    TerrainFormerEditor.allTerrainHeights[globalTerrainY, globalTerrainX] += 
                        (normalizedHeight - TerrainFormerEditor.allTerrainHeights[globalTerrainY, globalTerrainX]) * brushSample * 0.5f;
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
                        Mathf.Clamp01(Mathf.Lerp(TerrainFormerEditor.allUnmodifiedTerrainHeights[globalTerrainY, globalTerrainX], normalizedHeight, 
                        -TerrainFormerEditor.Instance.currentTotalMouseDelta * brushSample * 0.02f));
                }
            }

            terrainJobData.manualResetEvent.Set();
        }

        protected override void OnShiftClick(object data) { }
        
        protected override void OnShiftClickDown() {
            TerrainFormerEditor.Instance.UpdateSetHeightAtMousePosition();
        }
    }
}
