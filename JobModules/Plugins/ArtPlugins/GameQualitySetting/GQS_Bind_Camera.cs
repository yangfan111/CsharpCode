using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ArtPlugins
{
    [RequireComponent(typeof(Camera))]
    public class GQS_Bind_Camera : MonoBehaviour
    {

        private void Start()
        {
            var cmr = GetComponent<Camera>();
            cmr.gameObject.AddComponent<GQS_LightDetail_Camera>();
        }

    }

}
 