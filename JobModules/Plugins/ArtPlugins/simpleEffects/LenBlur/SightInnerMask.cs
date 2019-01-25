using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArtPlugins
{
    public class SightInnerMask : MonoBehaviour
    {
        public Renderer[] sightRenders;


        private void OnEnable()
        {
            if (sightRenders == null)return;
foreach( var item in sightRenders)
                item.material.renderQueue = 3001;
        }
        private void OnDisable()
        {
                        if (sightRenders == null)return;
foreach( var item in sightRenders)
                item.material.renderQueue = item.material.shader.renderQueue;
        
                
        }
    }

}