using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArtPlugins  {
    [Serializable]
    public class TerrainDetailRemoveParams {
        public bool enabled = true;

        public Texture2D texture;
        public Texture2D textureMask;
        [Range(0.1f, 1.0f)]
        public float removeAlpha=0.5f;
    }
    public class TerrainDetailRemove : MonoBehaviour
    {
        public int showCount = 1;
        public TerrainDetailRemoveParams[] maskParams;

    }

}
