using UnityEngine;

namespace JesseStiller.TerrainFormerExtension {
    internal class PaintTextureCommand : TerrainCommand {
        internal override string GetName() {
            return "Paint Texture";
        }

        protected override bool GetUsesShift() {
            return false;
        }

        protected override bool GetUsesControl() {
            return false;
        }

        internal PaintTextureCommand(float[,] brushSamples) : base(brushSamples) { }

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

                    int selectedTextureIndex = TerrainFormerEditor.settings.selectedTextureIndex;
                    int layerCount = TerrainFormerEditor.allTextureSamples.GetLength(2);

                    // Apply paint to currently selected texture type
                    if(TerrainFormerEditor.settings.targetOpacity > brushSample) {
                        TerrainFormerEditor.allTextureSamples[globalTerrainY, globalTerrainX, selectedTextureIndex] =
                            Mathf.Min(TerrainFormerEditor.allTextureSamples[globalTerrainY, globalTerrainX, selectedTextureIndex] + brushSample, TerrainFormerEditor.settings.targetOpacity);
                    } else {
                        TerrainFormerEditor.allTextureSamples[globalTerrainY, globalTerrainX, selectedTextureIndex] =
                            Mathf.Max(TerrainFormerEditor.allTextureSamples[globalTerrainY, globalTerrainX, selectedTextureIndex] - brushSample, TerrainFormerEditor.settings.targetOpacity);
                    }

                    float sum = 0f;
                    for(int l = 0; l < layerCount; l++) {
                        if(l != selectedTextureIndex) sum += TerrainFormerEditor.allTextureSamples[globalTerrainY, globalTerrainX, l];
                    }

                    if(sum > 0.01f) {
                        float sumCoefficient = (1f - TerrainFormerEditor.allTextureSamples[globalTerrainY, globalTerrainX, selectedTextureIndex]) / sum;
                        for(int l = 0; l < layerCount; ++l) {
                            if(l != selectedTextureIndex) TerrainFormerEditor.allTextureSamples[globalTerrainY, globalTerrainX, l] *= sumCoefficient;
                        }
                    } else {
                        for(int l = 0; l < layerCount; ++l) {
                            TerrainFormerEditor.allTextureSamples[globalTerrainY, globalTerrainX, l] = l != selectedTextureIndex ? 0f : 1f;
                        }
                    }
                }
            }

            terrainJobData.manualResetEvent.Set();
        }

        protected override void OnControlClick(object data) { }

        protected override void OnShiftClick(object data) { }

        protected override void OnShiftClickDown() { }
    }
}
