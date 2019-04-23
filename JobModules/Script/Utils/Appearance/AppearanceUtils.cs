using Core.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;

namespace Utils.Appearance
{
    public static class AppearanceUtils
    {
        private static LoggerAdapter Logger = new LoggerAdapter(typeof(AppearanceUtils));

        private static List<SkinnedMeshRenderer> _skinnedMeshRenderers = new List<SkinnedMeshRenderer>(128);
        private static List<MeshRenderer> _meshRenderers = new List<MeshRenderer>(128);
        public static void DisableRender(GameObject obj)
        {
            if (obj != null)
            {
                
                Logger.DebugFormat("Disable Render: {0}", obj);
                _skinnedMeshRenderers.Clear();
                obj.GetComponentsInChildren(_skinnedMeshRenderers);
                foreach (var renderer in _skinnedMeshRenderers)
                {
                    renderer.enabled = false;
                }
                _meshRenderers.Clear();
                obj.GetComponentsInChildren(_meshRenderers);
                foreach (var renderer in _meshRenderers)
                {
                    renderer.enabled = false;
                }
            }
        }

        public static void DisableShadow(GameObject obj)
        {
            if (obj != null)
            {
                Logger.DebugFormat("Disable shadow: {0}", obj);
                _skinnedMeshRenderers.Clear();
                obj.GetComponentsInChildren(_skinnedMeshRenderers);
                foreach (var renderer in _skinnedMeshRenderers)
                {
                    renderer.shadowCastingMode = ShadowCastingMode.Off;
                }  
                _meshRenderers.Clear();
                obj.GetComponentsInChildren(_meshRenderers);
                foreach (var renderer in _meshRenderers)
                {
                    renderer.shadowCastingMode = ShadowCastingMode.Off;
                }
            }
        }

        public static void EnableRender(GameObject obj)
        {
            if (obj != null)
            {

                Logger.DebugFormat("Enable Render: {0}", obj);
                _skinnedMeshRenderers.Clear();
                obj.GetComponentsInChildren(_skinnedMeshRenderers);
                foreach (var renderer in _skinnedMeshRenderers)
                {
                    renderer.enabled = true;
                }
                _meshRenderers.Clear();
                obj.GetComponentsInChildren(_meshRenderers);
                foreach (var renderer in _meshRenderers)
                {
                    renderer.enabled = true;
                }
            }
        }
        
        public static void EnableShadow(GameObject obj)
        {
            if (obj != null)
            {
                Logger.DebugFormat("Enable shadow: {0}", obj);
                _skinnedMeshRenderers.Clear();
                obj.GetComponentsInChildren(_skinnedMeshRenderers);
                foreach (var renderer in _skinnedMeshRenderers)
                {
                    renderer.shadowCastingMode = ShadowCastingMode.On;
                }  
                _meshRenderers.Clear();
                obj.GetComponentsInChildren(_meshRenderers);
                foreach (var renderer in _meshRenderers)
                {
                    renderer.shadowCastingMode = ShadowCastingMode.On;
                }
            }
        }

        public static void ActiveGameobject(GameObject go)
        {
            if (null != go)
            {
                go.SetActive(true);
                EnableRender(go);
            }
        }

        public static void UnactiveGameobject(GameObject go)
        {
            if (null != go)
            {
                go.SetActive(false);
                DisableRender(go);
            }
        }
    }
}
