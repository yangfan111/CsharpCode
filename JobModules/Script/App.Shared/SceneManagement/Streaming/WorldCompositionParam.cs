using System.Collections.Generic;
using UnityEngine;

namespace App.Shared.SceneManagement.Streaming
{
    public class WorldCompositionParam
    {
        public Vector3 TerrainMin;
        public int TerrainSize;
        public int TerrainDimension;
        public string TerrainNamePattern;

        public string AssetBundleName;
        public List<string> FixedScenes;

        public float LoadRadiusInGrid;
        public float UnloadRadiusInGrid;
    }
}