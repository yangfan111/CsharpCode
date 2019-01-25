using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu("AQUAS/Render Queue Controller")]
public class AQUAS_RenderQueueEditor : MonoBehaviour
{
    private Renderer _renderer;
    private new Renderer renderer
    {
        get
        {
            if (_renderer == null) { _renderer = GetComponent<Renderer>(); }
            return _renderer;
        }
    }

    public int renderQueueIndex = -1;
    void Update()
    {
        //gameObject.GetComponent<Renderer>().sharedMaterial.renderQueue = renderQueueIndex;
        if (renderer != null && renderer.sharedMaterial != null)
        {
            renderer.sharedMaterial.renderQueue = renderQueueIndex;
        }
    }
}

