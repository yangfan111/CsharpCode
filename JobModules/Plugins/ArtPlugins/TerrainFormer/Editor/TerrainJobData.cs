using System.Threading;

namespace JesseStiller.TerrainFormerExtension {
    internal struct TerrainJobData {
        public int yStart, yEnd;
        public bool coverEntireTerrainGrid;
        public ManualResetEvent manualResetEvent;
        public TerrainJobData(int yStart, int yEnd, bool coverEntireTerrainGrid, ManualResetEvent manualResetEvent) {
            this.yStart = yStart;
            this.yEnd = yEnd;
            this.coverEntireTerrainGrid = coverEntireTerrainGrid;
            this.manualResetEvent = manualResetEvent;
        }
    }
}
