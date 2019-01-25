using UnityEngine;

namespace JesseStiller.TerrainFormerExtension {
    internal class FlattenCommand : TerrainCommand {
        private FlattenMode mode;
        private float flattenHeight;
        
        internal override string GetName() {
            return "Flatten";
        }
        
        protected override bool GetUsesShift() {
            const bool usesShift = false;
            return usesShift;
        }
        
        protected override bool GetUsesControl() {
            const bool usesControl = true;
            return usesControl;
        }

        internal FlattenCommand(float[,] brushSamples, float flattenHeight) : base(brushSamples) {
            mode = TerrainFormerEditor.settings.flattenMode;
            this.flattenHeight = flattenHeight;
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

                    if((mode == FlattenMode.Flatten && TerrainFormerEditor.allTerrainHeights[globalTerrainY, globalTerrainX] <= flattenHeight) ||
                        (mode == FlattenMode.Extend && TerrainFormerEditor.allTerrainHeights[globalTerrainY, globalTerrainX] >= flattenHeight)) continue;

                    TerrainFormerEditor.allTerrainHeights[globalTerrainY, globalTerrainX] = 
                        TerrainFormerEditor.allTerrainHeights[globalTerrainY, globalTerrainX] + 
                        (flattenHeight - TerrainFormerEditor.allTerrainHeights[globalTerrainY, globalTerrainX]) * brushSample * 0.5f;
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

                    if((mode == FlattenMode.Flatten && TerrainFormerEditor.allTerrainHeights[globalTerrainY, globalTerrainX] <= flattenHeight) ||
                        (mode == FlattenMode.Extend && TerrainFormerEditor.allTerrainHeights[globalTerrainY, globalTerrainX] >= flattenHeight)) continue;

                    TerrainFormerEditor.allTerrainHeights[globalTerrainY, globalTerrainX] = 
                        Mathf.Clamp01(Mathf.Lerp(TerrainFormerEditor.allUnmodifiedTerrainHeights[globalTerrainY, globalTerrainX], flattenHeight, 
                            -TerrainFormerEditor.Instance.currentTotalMouseDelta * brushSample * 0.03f));
                }
            }

            terrainJobData.manualResetEvent.Set();
        }

        protected override void OnShiftClick(object data) { }

        protected override void OnShiftClickDown() { }
    }
}
