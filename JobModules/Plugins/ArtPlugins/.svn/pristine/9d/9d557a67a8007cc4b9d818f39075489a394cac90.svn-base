using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
namespace ArtPlugins
{
    public class LightProbesData : MonoBehaviour
    {
        [HideInInspector]
        [SerializeField]
      public  List<float> prebesList;
        [HideInInspector]
        [SerializeField]
        public List<Vector3> posList;
        //public SphericalHarmonicsL2[] sphericals;
        //public List<Vector3> positions;

        // Use this for initialization
        void Start()
        {

        }
        [ContextMenu("save data")]
        private void save()
        {
            prebesList = new List<float>();
            posList = new List<Vector3>();
             for (int j = 0; j < LightmapSettings.lightProbes.bakedProbes.Length; j++)
            {
 
                SphericalHarmonicsL2 s = LightmapSettings.lightProbes.bakedProbes[j];
                for (int i = 0; i < 3; i++)
                {
                    for (int k = 0; k < 9; k++)
                    {
 prebesList.Add(s[i, k]);
                    }
 
                }
                posList.Add(LightmapSettings.lightProbes.positions[j]);
            }
           

            }
      
    }
}