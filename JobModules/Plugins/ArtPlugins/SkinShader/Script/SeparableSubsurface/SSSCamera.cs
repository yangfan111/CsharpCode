using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Camera))]
public class SSSCamera : MonoBehaviour
{
    public static SkinRender currentSkr;
    public SkinRender skinRender;
    private void Awake()
    {
        SSSFunction.InitSkinRender(ref skinRender, GetComponent<Camera>());
    }

    private void OnEnable()
    {
        skinRender.RenderCamera.depthTextureMode |= DepthTextureMode.Depth;
        skinRender.RenderCamera.AddCommandBuffer(UnityEngine.Rendering.CameraEvent.BeforeImageEffectsOpaque, skinRender.SubsurfaceBuffer);
    }

    private void OnDisable()
    { 
        skinRender.RenderCamera.RemoveCommandBuffer(UnityEngine.Rendering.CameraEvent.BeforeImageEffectsOpaque, skinRender.SubsurfaceBuffer);
    }

    private void OnPreCull()
    {
        currentSkr = skinRender;
        SSSFunction.ClearBuffer(ref currentSkr);
    }

    private void OnDestroy()
    {
        SSSFunction.DisposeSkinRender(ref skinRender);
    }
}
