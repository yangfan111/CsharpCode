using System;

namespace JesseStiller.TerrainFormerExtension {
    internal class SmoothCommand : TerrainCommand {
        private readonly int smoothRadius, totalTerrainWidth, totalTerrainHeight;
        
        internal override string GetName() {
            return "Smooth";
        }

        protected override bool GetUsesShift() {
            return false;
        }

        protected override bool GetUsesControl() {
            return false;
        }

        internal SmoothCommand(float[,] brushSamples, int smoothRadius, int totalTerrainWidth, int totalTerrainHeight) : base(brushSamples) {
            this.smoothRadius = smoothRadius;
            this.totalTerrainWidth = totalTerrainWidth;
            this.totalTerrainHeight = totalTerrainHeight;
        }
        
        internal override void OnClick(object data) {
            TerrainJobData terrainJobData = (TerrainJobData)data;

            float heightSum, neighbourCount; // Values which are inherently integers are saved as floats to avoid conversion
            int globalTerrainX, globalTerrainY, endX, endY;
            float brushSample;
            for(int y = terrainJobData.yStart; y < terrainJobData.yEnd; y++) {
                for(int x = 0; x < commandArea.width; x++) {
                    brushSample = terrainJobData.coverEntireTerrainGrid ? 1f : brushSamples[x + commandArea.clippedLeft, y + commandArea.clippedBottom];
                    if(brushSample == 0f) continue;

                    heightSum = 0f;
                    neighbourCount = 0f;

                    globalTerrainX = x + commandArea.leftOffset;
                    globalTerrainY = y + commandArea.bottomOffset;

                    endX = Math.Min(globalTerrainX + smoothRadius + 1, totalTerrainWidth);
                    endY = Math.Min(globalTerrainY + smoothRadius + 1, totalTerrainHeight);
                    for(int x2 = Math.Max(globalTerrainX - smoothRadius, 0); x2 < endX; x2++) {
                        for(int y2 = Math.Max(globalTerrainY - smoothRadius, 0); y2 < endY; y2++) {
                            heightSum += TerrainFormerEditor.allUnmodifiedTerrainHeights[y2, x2];
                            neighbourCount++;
                        }
                    }

                    // Finally apply the smoothed result.
                    TerrainFormerEditor.allTerrainHeights[globalTerrainY, globalTerrainX] = Utilities.LerpUnclamped(TerrainFormerEditor.allUnmodifiedTerrainHeights[globalTerrainY, globalTerrainX], heightSum / neighbourCount, brushSample);
                }
            }
            terrainJobData.manualResetEvent.Set();
        }
        
        protected override void OnControlClick(object data) { }

        protected override void OnShiftClickDown() { }

        protected override void OnShiftClick(object data) { }
    }
}
