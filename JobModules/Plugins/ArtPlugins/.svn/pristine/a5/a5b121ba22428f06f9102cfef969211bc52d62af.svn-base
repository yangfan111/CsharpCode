using UnityEngine;
using System.Collections;

namespace ArtPlugins
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class DepthOfFieldPostEffect : MonoBehaviour
    {

         private Material Mat;
        [Range(0, 2)]
        public float BlurSize = 10;
        [Range(0, 10)]
        public int interator = 2;
        [Range(0, 1)]
        public float FocusDistance = 0f;
        [Range(0, 0.5f)]
        public float FocusRange = 0.1f;
        [Range(0.001f, 1f)]
        public float FocusFar = 0.2f;

        public Texture mask;
        private Camera _camera;

        void OnEnable()
        {
            if(null != _camera)
            {
                _camera.depthTextureMode |= DepthTextureMode.Depth;
            }
        }
        void OnDisable()
        {
            _camera.depthTextureMode &= ~DepthTextureMode.Depth;
        }
        // Use this for initialization
        void Awake()
        {
            _camera = GetComponent<Camera>();
            Mat = new Material(Shader.Find("DepthOfFiled"));
        }
        private void OnDestroy()
        {
            Destroy(Mat);
            
        }

        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            //	Graphics.Blit (src, dest,Mat);
            var w = src.width;
            var h = src.height;
            var tmp1 = RenderTexture.GetTemporary(w, h);
            var tmp2 = RenderTexture.GetTemporary(w, h);
            Mat.SetFloat("_BlurSize", BlurSize);
            Mat.SetFloat("_FocusDistance", FocusDistance);
            Mat.SetFloat("_FocusRange", FocusRange);
            Mat.SetFloat("_FocusFar", FocusFar);
            Mat.SetTexture("_MaskTex", mask);
            Graphics.Blit(src, tmp1);

            for (int i = 0; i < interator; i++)
            {
                Graphics.Blit(tmp1, tmp2, Mat, 0);
                Graphics.Blit(tmp2, tmp1, Mat, 1);
            }

            Mat.SetTexture("_BlurTex", tmp1);

            Graphics.Blit(src, dest, Mat, 2);

            RenderTexture.ReleaseTemporary(tmp1);
            RenderTexture.ReleaseTemporary(tmp2);

        }
    }
}