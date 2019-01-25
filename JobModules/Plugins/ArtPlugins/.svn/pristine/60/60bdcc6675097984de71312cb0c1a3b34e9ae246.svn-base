using ArtPlugins;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace ArtPlugins
{
    [ExecuteInEditMode]
    public class FarItemDraw : MonoBehaviour
    {
        public int colMax = 8;
        public int rowMax = 8;

        public int colIndex = 5;
        public int rowIndex = 5;
        MaterialPropertyBlock prop = null;
        private Terrain terrain;
        private void Start()
        {
            ApplyData();
        }

        [ContextMenu("flush")]
        void ApplyData()
        {
            if (prop == null) prop = new MaterialPropertyBlock();
            if (terrain == null) terrain = GetComponent<Terrain>();
            // GetComponent<Terrain>().materialTemplate.SetVector("FarUV", new Vector4( colIndex  , rowIndex,colMax,rowMax));
            terrain.GetSplatMaterialPropertyBlock(prop);
            prop.SetVector("FarUV", new Vector4(colIndex, rowIndex, colMax, rowMax));
            terrain.SetSplatMaterialPropertyBlock(prop);
        }
     


    }

}