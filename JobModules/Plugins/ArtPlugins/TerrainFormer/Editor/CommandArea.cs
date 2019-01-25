namespace JesseStiller.TerrainFormerExtension {
    public class CommandArea {
        /**
        * Clipped left/bottom refers to how many units have been clipped on a given side, and clipped width/height are the spans taking into account clipping from 
        * the brush hanging off edge(s) of the terrain.
        */
        public int leftOffset, bottomOffset, clippedLeft, clippedBottom, width, height;

        public CommandArea() { }

        public CommandArea(int leftOffset, int bottomOffset, int clippedLeft, int clippedBottom, int width, int height) {
            this.leftOffset = leftOffset;
            this.bottomOffset = bottomOffset;
            this.clippedLeft = clippedLeft;
            this.clippedBottom = clippedBottom;
            this.width = width;
            this.height = height;
        }

        public override string ToString() {
            return string.Format("Clipped (left: {0}, bottom: {1}, width: {2}, height: {3}), offset (left: {4}, bottom: {5})", clippedLeft, clippedBottom,
                width, height, leftOffset, bottomOffset);
        }
    }
}
