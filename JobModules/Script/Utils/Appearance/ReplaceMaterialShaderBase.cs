using System.Collections.Generic;
using UnityEngine;
using Utils.AssetManager;

namespace Utils.Appearance
{
    public abstract class ReplaceMaterialShaderBase
    {
        private static Shader _topLayerShader;
        private static Shader _defaultShader;
        private readonly List<AbstractLoadRequest> _loadRequestBatch = new List<AbstractLoadRequest>();
        private readonly ReplaceShaderHandler _topSideShaderHandlers;
        private readonly ReplaceShaderHandler _defaultShaderHandlers;
        
        private static readonly Dictionary<int, int> RenderQueues = new Dictionary<int, int>();

        protected ReplaceMaterialShaderBase()
        {
            _topSideShaderHandlers = new ReplaceShaderHandler(ShaderType.TopSideShader);
            _defaultShaderHandlers = new ReplaceShaderHandler(ShaderType.DefaultShader);
            LoadResource();
        }

        public List<AbstractLoadRequest> GetLoadRequests()
        {
            return _loadRequestBatch;
        }

        public void ClearRequests()
        {
            _loadRequestBatch.Clear();
        }

        private void LoadResource()
        {
            _loadRequestBatch.Add(CreateLoadRequest(new AssetInfo("shaders", "MSAO_FP"), _topSideShaderHandlers));
            _loadRequestBatch.Add(CreateLoadRequest(new AssetInfo("shaders", "MSAO"), _defaultShaderHandlers));
        }

        public static void ChangeShader(GameObject obj)
        {
            if (null == obj || null == _topLayerShader) return;
            foreach (var value in obj.GetComponentsInChildren<Renderer>())
            {
                var shader = value.material.shader;
                if (null != shader && shader.name.Contains("MSAO"))
                {
                    var q = RenderQueues[value.material.GetInstanceID()] = value.material.renderQueue;
                    value.material.shader = _topLayerShader;
                    value.material.renderQueue = q;
                }
            }
        }

        public static void ResetShader(GameObject obj)
        {
            if (null == obj || null == _defaultShader || null == _topLayerShader) return;
            foreach (var value in obj.GetComponentsInChildren<Renderer>())
            {
                if (value.material.shader.name.Equals(_topLayerShader.name))
                {
                    value.material.shader = _defaultShader;
                    if (RenderQueues.ContainsKey(value.material.GetInstanceID()))
                        value.material.renderQueue = RenderQueues[value.material.GetInstanceID()];
                }
            }
        }

        protected abstract AbstractLoadRequest CreateLoadRequest(AssetInfo assetInfo, ILoadedHandler mountHandler);

        private class ReplaceShaderHandler : ILoadedHandler
        {
            private readonly ShaderType _shaderType;

            public ReplaceShaderHandler(ShaderType shaderType)
            {
                _shaderType = shaderType;
            }

            public void OnLoadSucc<T>(T source, UnityObject obj)
            {
                if (null == obj) return;
                var m = obj.AsObject as Material;
                if (null == m) return;
                switch (_shaderType)
                {
                    case ShaderType.DefaultShader:
                        _defaultShader = m.shader;
                        break;
                    case ShaderType.TopSideShader:
                        _topLayerShader = m.shader;
                        break;
                }
            }
        }

        private enum ShaderType
        {
            DefaultShader,
            TopSideShader
        }
    }
}