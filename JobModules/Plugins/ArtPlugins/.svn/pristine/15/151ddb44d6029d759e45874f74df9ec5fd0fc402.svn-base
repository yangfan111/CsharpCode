using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Renderer))]
public class SSSObject : MonoBehaviour {
    private List<Vector4> kernel = new List<Vector4>(16);
    public SkinData data;
	// Use this for initialization

	private void Awake () {
        
        data.renderer = GetComponent<Renderer>();
        SSSFunction.SetKernel(ref data, kernel);
    }
    private void OnEnable()
    {
            
    }
    private void OnWillRenderObject()
    {
        if (enabled == false) return;
        if(SSSCamera.currentSkr.RenderCamera!=null)
        SSSFunction.UpdateSubsurface(ref SSSCamera.currentSkr, ref data, kernel);
    }
}
