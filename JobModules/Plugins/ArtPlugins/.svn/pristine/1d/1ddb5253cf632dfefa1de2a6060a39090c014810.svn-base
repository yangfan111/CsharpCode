using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

namespace ArtPlugins
{
    public class SceneCheck
    {

        [Test(Description ="地型数量")]
        public void  TerrianCount()
        {
            // Use the Assert class to test conditions.
           int count= GameObject.FindObjectsOfType<Terrain>().Length;
            Assert.IsTrue(count<=1,"地型数量:"+ count);
        }
        [Test(Description ="存在后效")]
        public void LightingBox()
        {
            // Use the Assert class to test conditions.
           var  help= GameObject.FindObjectOfType<LB_LightingBoxHelper>();
             var volume = GameObject.FindObjectOfType<UnityEngine.Rendering.PostProcessing.PostProcessVolume>();
            Assert.IsTrue(help == null&& volume==null, "存在后效LB_LightingBoxHelper 或 PostProcessVolume");
        }
        [Test(Description = "存在相机")]
        public void ExistCamera()
        {
            // Use the Assert class to test conditions.
            var cmr = GameObject.FindObjectOfType<Camera>();
            Assert.IsTrue(cmr == null);
 
        }
        [Test(Description = "存在平行光")]
        public void ExistLight()
        {
            // Use the Assert class to test conditions.
            foreach (var item in  GameObject.FindObjectsOfType<Light>())
            {
                if (item.type == LightType.Directional) {
                    Assert.IsTrue(false);
                }
            } ;
            Assert.IsTrue(true);

        }

    }

}