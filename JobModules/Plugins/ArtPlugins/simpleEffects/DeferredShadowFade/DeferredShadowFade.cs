using UnityEngine;
using UnityEngine.Rendering;

public class DeferredShadowFade : MonoBehaviour
{
	private CommandBuffer _commandBuffer;
    // Use this for initialization
    private void Awake()
    {
        _commandBuffer = new CommandBuffer();
        _commandBuffer.name = "Deferred Shadow Fade";
    }
    private void OnEnable()
    {
        _commandBuffer.Clear();
        int buffer0 = Shader.PropertyToID("buffer0");
        int buffer4 = Shader.PropertyToID("buffer4");
        _commandBuffer.ReleaseTemporaryRT(buffer0);
        _commandBuffer.ReleaseTemporaryRT(buffer4);
        _commandBuffer.GetTemporaryRT(buffer0, Screen.width/4, Screen.height/4, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, QualitySettings.antiAliasing);
        _commandBuffer.GetTemporaryRT(buffer4, Screen.width/4, Screen.height/4, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, QualitySettings.antiAliasing);
        _commandBuffer.Blit(BuiltinRenderTextureType.GBuffer0, buffer0);
        _commandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, buffer4);
       GetComponent<Camera>().AddCommandBuffer(CameraEvent.AfterFinalPass, _commandBuffer);
    }
    private void OnDisable()
    {
        GetComponent<Camera>().RemoveCommandBuffer(CameraEvent.AfterFinalPass, _commandBuffer);

    }
 
 
}
