using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPropFlash : MonoBehaviour {
    class FlashMaterial {
        public Texture _EmissionMap;
        public bool _EMISSION;
        public Color _EmissionColor;
        public Material material;
    };
    private  List<FlashMaterial> materials;
    public Color color;
    public AnimationCurve curve;
    private float lastUpdateTime;
    // Use this for initialization
    void Active() {
       var  renderers =transform.parent.GetComponentsInChildren<Renderer>();
      
        if (null == renderers || renderers.Length < 1)
        {
            Debug.LogErrorFormat("no render exist in {0}", gameObject.name);
            return;
        }
        materials = new List<FlashMaterial>();
        foreach (var r in renderers)

        {
            if (r == null||r.material==null) continue;
            FlashMaterial mat = new FlashMaterial();
            mat._EmissionMap = r.material.GetTexture("_EmissionMap");
            mat._EMISSION = r.material.IsKeywordEnabled("_EMISSION");
            mat._EmissionColor = r.material.GetColor("_EmissionColor");
            mat.material = r.material;
            materials.Add(mat);
             r.material.SetTexture("_EmissionMap",null);
            r.material.SetColor("_EmissionColor", Color.clear);
            r.material.EnableKeyword("_EMISSION");
           
        }
        lastUpdateTime = Time.time + Random.Range(0.0f, 0.2f);
    }

    private void EnableFlash(bool enable)
    {
        if(enable)
        {
            Active();
        }
        else
        {
            OnDisable();
        }
    }

    private void OnDisable()
    {
        OnDestroy(); 
    }

    private void OnDestroy()
    {
        if(null != materials)
        {
            foreach (var r in materials)
            {
                
                    r.material.SetColor("_EmissionColor", r._EmissionColor);
                    r.material.SetTexture("_EmissionMap", r._EmissionMap);
                if(r._EMISSION)
                    r.material.EnableKeyword("_EMISSION");
                else
                    r.material.DisableKeyword("_EMISSION");
 
            }
        }
    }
 
    // Update is called once per frame
    void Update () {

        if (lastUpdateTime > Time.time - 0.2f)
        {
            return;
        }
        if (Camera.main == null || (Camera.main.transform.position - transform.parent.position).sqrMagnitude > 100) return;
        lastUpdateTime = Time.time;
        if (materials == null) return;
        foreach (var r in materials)
        {
            
 
            r.material.SetColor("_EmissionColor",Color.Lerp( Color.clear, color, curve.Evaluate(Time.time)));
        }

    }
}
