using UnityEngine;
using Math = System.Math;

namespace JesseStiller.TerrainFormerExtension {
    internal class MouldCommand : TerrainCommand {
        private int totalTerrainWidth, totalTerrainHeight;
        private int smoothRadius;
        private Vector3 firstTerrainPosition;
        private float terrainHeightCoefficient;
        private float mouldRaycastOffset;

        internal override string GetName() {
            return "Mould";
        }

        protected override bool GetUsesShift() {
            return false;
        }

        protected override bool GetUsesControl() {
            return false;
        }

        internal MouldCommand(float[,] brushSamples, int smoothRadius, int totalTerrainWidth, int totalTerrainHeight) : base(brushSamples) {
            this.smoothRadius = smoothRadius;
            this.totalTerrainWidth = totalTerrainWidth;
            this.totalTerrainHeight = totalTerrainHeight;

            firstTerrainPosition = TerrainFormerEditor.Instance.firstTerrainTransform.position;
            mouldRaycastOffset = TerrainFormerEditor.settings.mouldToolRaycastOffset;
            terrainHeightCoefficient = 1f / TerrainFormerEditor.Instance.terrainSize.y;
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

                    globalTerrainX = x + commandArea.leftOffset;
                    globalTerrainY = y + commandArea.bottomOffset;

                    if(TerrainFormerEditor.toolScratchArray[x, y] != -1f) {
                        TerrainFormerEditor.allTerrainHeights[globalTerrainY, globalTerrainX] =
                            Mathf.Lerp(TerrainFormerEditor.allTerrainHeights[globalTerrainY, globalTerrainX],
                            (TerrainFormerEditor.toolScratchArray[x, y] - firstTerrainPosition.y - mouldRaycastOffset) * terrainHeightCoefficient, brushSample * 3f);
                        continue;
                    }

                    heightSum = 0f;
                    neighbourCount = 0f;

                    endX = Math.Min(globalTerrainX + smoothRadius + 1, totalTerrainWidth);
                    endY = Math.Min(globalTerrainY + smoothRadius + 1, totalTerrainHeight);
                    // Smooth heights that are not in the raycast map.
                    for(int y2 = Math.Max(globalTerrainY - smoothRadius, 0); y2 < endY; y2++) {
                        for(int x2 = Math.Max(globalTerrainX - smoothRadius, 0); x2 < endX; x2++) {
                            heightSum += TerrainFormerEditor.allUnmodifiedTerrainHeights[y2, x2];
                            neighbourCount++;
                        }
                    }
                    TerrainFormerEditor.allTerrainHeights[globalTerrainY, globalTerrainX] =
                        Utilities.LerpUnclamped(TerrainFormerEditor.allUnmodifiedTerrainHeights[globalTerrainY, globalTerrainX], heightSum / neighbourCount, brushSample);
                }
            }

            terrainJobData.manualResetEvent.Set();
        }

        protected override void OnControlClick(object data) { }

        protected override void OnShiftClick(object data) { }

        protected override void OnShiftClickDown() { }
    }
}
