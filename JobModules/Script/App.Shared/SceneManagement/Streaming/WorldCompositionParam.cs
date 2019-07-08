using System.Collections.Generic;
using UnityEngine;

namespace App.Shared.SceneManagement.Streaming
{
    public class WorldCompositionParam
    {
        public static float LoadRadiusFromGM = 0.0f;
        public static float UnloadRadiusFromGM = 0.0f;

        public Vector3 TerrainMin;
        public int TerrainSize;
        public int TerrainDimension;
        public string TerrainNamePattern;

        public string AssetBundleName;
        public string PreMapName;
        public List<string> FixedScenes;

        public float LoadRadiusInGrid;
        public float UnloadRadiusInGrid;
    }
}