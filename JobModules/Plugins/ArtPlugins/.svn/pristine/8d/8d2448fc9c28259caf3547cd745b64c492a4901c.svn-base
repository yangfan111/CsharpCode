using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArtPlugins
{
    public class ChangeModeTest : MonoBehaviour
    {
        public Terrain terrain;
        public GameObject newMode;
        // Use this for initialization
       

        private void OnGUI()
        {
            if (terrain == null) {
                terrain = FindObjectOfType<Terrain>();
            }
            if (GUILayout.Button("改变草树模式")) {
                newMode.SetActive(!newMode.activeSelf);
                terrain.drawTreesAndFoliage = !newMode.activeSelf;
            }
        }
    }
}