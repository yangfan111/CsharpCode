//Command Buffer测试
//by: puppet_master
//2017.5.26
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
namespace ArtPlugins
{
    public class DepthOfFieldPostEffectMask : MonoBehaviour
    {
        [SerializeField] public float _distance;
        [SerializeField] public float _range;
        [SerializeField] public float _far;
        [SerializeField] public float _blueSize;
        [SerializeField] public int _interator;

        private CommandBuffer commandBuffer = null;
        private RenderTexture renderTexture = null;
      //  private Renderer[] targetRenderer = null;
       // public GameObject targetObject = null;
        private Material replaceMaterial = null;
        public Color bgColor = Color.white;
        public Color maskColor = Color.black;
        public DepthOfFieldPostEffect depthOfFieldPostEffect;
        [ContextMenu("testApply")]
        public void Apply()
        {
            replaceMaterial = new Material(Shader.Find("Unlit/Color"));
            replaceMaterial.color = maskColor;
            Camera.main.RemoveCommandBuffer(CameraEvent.AfterImageEffects, commandBuffer);
            //初始颜色设置为灰色
            commandBuffer.ClearRenderTarget(true, true, bgColor);
            var render = GetComponent<Renderer>();
            //绘制目标对象，如果没有替换材质，就用自己的材质
            //   foreach (var render in targetRenderer)
            {
                if (render.gameObject.activeInHierarchy)
                {
                    Material mat = replaceMaterial == null ? render.sharedMaterial : replaceMaterial;
                    commandBuffer.DrawRenderer(render, mat);
                }
            }
            //直接加入相机的CommandBuffer事件队列中
            Camera.main.AddCommandBuffer(CameraEvent.AfterImageEffects, commandBuffer);
        }

        void EnableEffect(bool enalbe)
        {
            if(enalbe)
            {
                var cam = Camera.main;
                var effect = cam.GetComponent<DepthOfFieldPostEffect>();
                if(null == effect)
                {
                    effect = cam.gameObject.AddComponent<DepthOfFieldPostEffect>();
                }
                effect.FocusDistance = _distance;
                effect.FocusRange = _range;
                effect.FocusFar = _far;
                effect.BlurSize = _blueSize;
                effect.interator = _interator;
                if(null == renderTexture || !renderTexture.IsCreated())
                {
                    OnEnableEffect();
                    Apply();
                    effect.enabled = true;
                }
            }
            else
            {
                if(null != renderTexture)
                {
                    OnDisableEffect();
                }
            }
        }

        void OnEnableEffect()
        {
            //申请RT
            renderTexture = RenderTexture.GetTemporary(512, 512, 16, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 4);
            commandBuffer = new CommandBuffer();
            //设置Command Buffer渲染目标为申请的RT
            commandBuffer.SetRenderTarget(renderTexture);

            Camera.main.AddCommandBuffer(CameraEvent.AfterImageEffects, commandBuffer);

            //然后接受物体的材质使用这张RT作为主纹理
            //this.GetComponent<Renderer>().sharedMaterial.mainTexture = renderTexture;
            depthOfFieldPostEffect = Camera.main.GetComponent<DepthOfFieldPostEffect>();
            depthOfFieldPostEffect.mask = renderTexture;
            Apply();
        }

        void OnDisableEffect()
        {
            //移除事件，清理资源
            Camera.main.RemoveCommandBuffer(CameraEvent.AfterImageEffects, commandBuffer);
            commandBuffer.Clear();
            renderTexture.Release();
        }

        private void OnDisable()
        {
            EnableEffect(false);
        }

    }

}