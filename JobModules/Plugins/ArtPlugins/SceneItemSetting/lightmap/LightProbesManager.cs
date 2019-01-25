using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace ArtPlugins
{
 //   [ExecuteInEditMode]
    public class LightProbesManager : MonoBehaviour
    {
        public LightProbes lightProbes; 

        // Use this for initialization
        void Start()
        {
             SceneManager.sceneLoaded += onSceneLoad;

        }
        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= onSceneLoad;
        }
        private void onSceneLoad(Scene arg0, LoadSceneMode arg1)
        {
            setData();
        }

        [ContextMenu("print")]
        void printdata()
        {
            print(LightmapSettings.lightProbes.positions.Length);
            

        }
        [ContextMenu("makeGlobalLightProbes")]
          void  updateData() {
                  List<SphericalHarmonicsL2> sphericals=new List<SphericalHarmonicsL2>();
            List<Vector3> positions=new List<Vector3>();
             Dictionary<Vector3, SphericalHarmonicsL2> probeDic = new Dictionary<Vector3, SphericalHarmonicsL2>();
            foreach (var item in GetComponentsInChildren<LightProbesData>())
            {
              
                 positions.AddRange( item.posList);

                for (int j = 0; j < item.prebesList.Count; )
                {
                   
                    SphericalHarmonicsL2 s = new SphericalHarmonicsL2();

                    int index = j / 27;
                    for (int i = 0; i < 3; i++)
                    {
                        for (int k = 0; k < 9; k++)
                            s[i, k] = item.prebesList[j++] ;
                        
                   
                    }
                   
                     //  sphericals.Add(s);
                    probeDic.Add(item.posList[index], s);
                }
            }

            //foreach (var item in positions)
            //{
            //    print(Array.IndexOf(LightmapSettings.lightProbes.positions, item));
            //}
           


            lightProbes = Instantiate( LightmapSettings.lightProbes);
            print(LightmapSettings.lightProbes.positions.Length);
            //    List<SphericalHarmonicsL2> sphericals = new List<SphericalHarmonicsL2>();
            int missProbeCount = 0;
                           for (int j = 0; j < lightProbes.positions.Length;j++)
                            {

                var pos = lightProbes.positions[j];
                //lightProbes.positions[j].x  = pos.x;
                //   lightProbes.positions[j].y = pos.y;
                //   lightProbes.positions[j].z = pos.z;


                SphericalHarmonicsL2 s
; if (probeDic.TryGetValue(pos, out s))
                {
                    sphericals.Add(s);
                     
                }
                else
                {
                    missProbeCount++;
                    sphericals.Add(LightmapSettings.lightProbes.bakedProbes[j]);
                }
            }
            Debug.Log("missProbeCount:" + missProbeCount);
             //  LightmapSettings.lightProbes.bakedProbes = sphericals.ToArray();
            lightProbes.bakedProbes = sphericals.ToArray();
            // lightProbes = LightmapSettings.lightProbes;
        }
        
        [ContextMenu("useGlobalLightProbes")]
        private void setData()
        {
            if (lightProbes == null) return;
                LightmapSettings.lightProbes = lightProbes;
             
        }


    }
}