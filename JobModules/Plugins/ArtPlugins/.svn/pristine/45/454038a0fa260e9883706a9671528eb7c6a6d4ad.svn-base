using UnityEditor;
using UnityEngine;

namespace JesseStiller.TerrainFormerExtension {
    public class TerrainInformation {
        public Transform transform;
        public Terrain terrain;
        public TerrainData terrainData;
        public TerrainCollider collider;
        public string terrainAssetPath;
        public CommandArea commandArea;
        public int gridXCoordinate, gridYCoordinate; // Co-ordinates of the terrain in respect to a terrain grid
        public int heightmapXOffset, heightmapYOffset;
        public int alphamapsXOffset, alphamapsYOffset;
        public int toolCentricXOffset, toolCentricYOffset; // The samples' offsets based on the current tool selected
        public bool hasChangedSinceLastSetHeights = false;
        public bool hasChangedSinceLastSave = false;
        public bool ignoreOnAssetsImported = false;
        
        public TerrainInformation(Terrain terrain) {
            this.terrain = terrain;
            transform = terrain.transform;
            collider = transform.GetComponent<TerrainCollider>();
            terrainData = terrain.terrainData;

            terrainAssetPath = AssetDatabase.GetAssetPath(terrainData);
        }
    }
}
