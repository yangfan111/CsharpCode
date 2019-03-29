using App.Shared.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.Singleton;
using UnityEngine.Profiling;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace App.Client.Tools
{
    public class MinRendererSetSampler : MonoBehaviour
    {
#if UNITY_EDITOR
        [MenuItem("CONTEXT/MinRendererSetSampler/ViewRenders")]
        private static void ViewRenders(MenuCommand command)
        {
            if (command == null || command.context == null)
            {
                Debug.LogError("command or command.context is null");
                return;
            }

            MinRendererSetSampler sampler = command.context as MinRendererSetSampler;
            if (sampler == null)
            {
                Debug.LogError("sampler is null");
                return;
            }

            sampler.ViewRenders();
        }

        [MenuItem("CONTEXT/MinRendererSetSampler/ToggleTerrainsAndTreeDetails")]
        private static void ToggleTerrains(MenuCommand command)
        {
            if (command == null || command.context == null)
            {
                Debug.LogError("command or command.context is null");
                return;
            }

            MinRendererSetSampler sampler = command.context as MinRendererSetSampler;
            if (sampler == null)
            {
                Debug.LogError("sampler is null");
                return;
            }

            sampler.ToggleTerrainsAndTreeDetails();
        }
#endif

        /// <summary>
        /// 用于记录颜色集合
        /// </summary>
        private class ColorSet
        {
            private Dictionary<int, Color32> colors = new Dictionary<int, Color32>();

            public Color this[int index]
            {
                get
                {
                    Color32 color;
                    if (!colors.TryGetValue(index, out color))
                    {
                        if (index < 0) index = -index;

                        if (index <= 255)
                        {
                            color = new Color32(0, 0, (byte)index, 255);
                        }
                        else if (index > 255 && index <= 255 * 255)
                        {
                            int num1 = index / 255, count1 = index % 255;
                            if (count1 == 0) { num1--; count1 = 255; }
                            color = new Color32(0, (byte)num1, (byte)count1, 255);
                        }
                        else
                        {
                            int C = 255 * 255;
                            int num1 = index / C, count1 = index % C;
                            if (count1 == 0) { num1--; count1 = 255; }
                            int num2 = count1 / 255, count2 = count1 % 255;
                            if (count2 == 0) { num2--; count2 = 255; }
                            color = new Color32((byte)num1, (byte)num2, (byte)count2, 255);
                        }

                        colors.Add(index, color);
                    }
                    return color;
                }
            }
        }

        public PlayerContext playerContext;

        /// <summary>
        /// 记录渲染场景的相机
        /// </summary>
        private Camera _cam;
        private Camera cam
        {
            get
            {
                if (_cam == null)
                {
                    _cam = Camera.main;
                }
                return _cam;
            }
        }

        private Shader _shader;
        private Shader shader
        {
            get
            {
                if (_shader == null)
                {
                    _shader = Shader.Find("Unlit/Color");

                    if (_shader == null)
                    {
                        Debug.LogErrorFormat("MinRenderSet error, can't get shader");
                    }
                }
                return _shader;
            }
        }

        private ColorSet _colorSet;
        private ColorSet colorSet
        {
            get
            {
                if (_colorSet == null)
                {
                    _colorSet = new ColorSet();
                }
                return _colorSet;
            }
        }

        private List<MeshRenderer> lastCollects = new List<MeshRenderer>();

        /// <summary>
        /// 恢复上次禁用的渲染体
        /// </summary>
        public void RecoveryLastCollects()
        {
            foreach (MeshRenderer mr in lastCollects)
            {
                if (mr != null)
                {
                    mr.enabled = true;
                }
            }
        }

        public bool DisableInvisibleRenderers()
        {
            // 确保产生玩家
            if (playerContext == null || playerContext.flagSelfEntity == null)
            {
                Debug.LogErrorFormat("MinRendererSetSampler.DisableInvisibleRenderers error, playerContext.flagSelfEntity is null");
                return false;
            }

            // 确保地形加载完毕
            if (!SingletonManager.Get<DynamicScenesController>().IsAllVisibleTerrianLoaded(playerContext.flagSelfEntity.position.Value))
            {
                Debug.LogErrorFormat("MinRendererSetSampler.DisableInvisibleRenderers error, terrains were not loaded fully");
                return false;
            }

            // 首先恢复上次禁用的渲染体
            RecoveryLastCollects();
            Debug.LogFormat("DisableInvisibleRenderers call, Recovery last collects, time:{0}", System.DateTime.Now);

            // 遍历所有MeshRenderer集合
            MeshRenderer[] mrs = FindObjectsOfType<MeshRenderer>();
            Debug.LogFormat("DisableInvisibleRenderers call, find all meshrenderer count:{0} time:{1}", mrs.Length, System.DateTime.Now);

            // 查找所有潜在可视集合
            List<MeshRenderer> visibleSet = new List<MeshRenderer>();
            for (int i = 0; i < mrs.Length; i++)
            {
                MeshRenderer mr = mrs[i];
                if (mr == null || !mr.gameObject.activeSelf || !mr.enabled || mr.sharedMaterials.Length <= 0) continue;

                MeshFilter mf = mr.GetComponent<MeshFilter>();
                if (mf == null || mf.sharedMesh == null) continue;

                if (mr.isVisible) visibleSet.Add(mr);
            }
            Debug.LogFormat("DisableInvisibleRenderers call, find potential meshrenderer count:{0} time:{1}", visibleSet.Count, System.DateTime.Now);

            // 替换为纯色shader
            Dictionary<MeshRenderer, Material[]> oldRenderMats = new Dictionary<MeshRenderer, Material[]>();
            Dictionary<string, MeshRenderer> renderColors = new Dictionary<string, MeshRenderer>();
            //System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < visibleSet.Count; i++)
            {
                MeshRenderer mr = visibleSet[i];
                if (mr == null || mr.sharedMaterials.Length <= 0) continue;

                oldRenderMats.Add(mr, mr.sharedMaterials);

                Material mat = new Material(shader);
                Color32 color = colorSet[i];
                mat.SetColor("_Color", color);

                Material[] mats = new Material[mr.sharedMaterials.Length];
                for (int k = 0; k < mats.Length; k++) mats[k] = mat;
                //foreach(var mat in mats)
                mr.sharedMaterials = mats;

                string key = string.Format("{0}_{1}_{2}", color.r, color.g, color.b);
                renderColors.Add(key, mr);

                //sb.AppendLine(color.ToString());
            }
            Debug.LogFormat("DisableInvisibleRenderers call, replace color shader, count:{0} time:{1}", oldRenderMats.Count, System.DateTime.Now);
            //System.IO.File.WriteAllText(@"C:\Users\Myself\Desktop\writePixels.txt", sb.ToString());

            // 关闭相机的可能的后效组件影响
            Dictionary<Component, bool> coms = new Dictionary<Component, bool>();
            var cs = cam.GetComponents<Component>();
            foreach (Component c in cs)
            {
                if (c != null)
                {
                    Transform tr = c as Transform;
                    Camera ca = c as Camera;
                    Behaviour beh = c as Behaviour;
                    if (tr == null && ca == null && beh != null)
                    {
                        coms.Add(c, beh.enabled);
                        beh.enabled = false;
                    }
                }
            }

            // 屏幕纹理捕捉
            Texture2D texture = CaputureScreenTexture();
            Debug.LogFormat("DisableInvisibleRenderers call, caputure screen texture, width:{0} height:{1} time:{2}", texture.width, texture.height, System.DateTime.Now);

            //{
            //    byte[] bs = texture.EncodeToPNG();
            //    System.IO.File.WriteAllBytes(@"C:\Users\Myself\Desktop\pixels.png", bs);
            //}

            // 恢复相机上可能的后效组件
            foreach (var pair in coms)
            {
                Behaviour beh = pair.Key as Behaviour;
                if (beh != null) beh.enabled = pair.Value;
            }

            // 获取实际渲染的物体
            Color32[] pixels = texture.GetPixels32(0);
            Debug.LogFormat("pixelsCount:{0} width:{1} height:{2} w*h:{3}", pixels.Length, texture.width, texture.height, texture.width * texture.height);
            HashSet<MeshRenderer> foundRenderers = new HashSet<MeshRenderer>();
            int step = 1, notFoundKeysCount = 0;
            //sb.Length = 0;
            for (int i = 0; i < texture.height; i += step)
            {
                for (int j = 0; j < texture.width; j += step)
                {
                    Color32 color = pixels[i * texture.width + j];
                    //sb.AppendLine(color.ToString());

                    string key = string.Format("{0}_{1}_{2}", color.r, color.g, color.b);
                    MeshRenderer mr = null;
                    if (!renderColors.TryGetValue(key, out mr))
                    {
                        notFoundKeysCount++;
                        continue;
                    }
                    if (mr != null && !foundRenderers.Contains(mr))
                    {
                        foundRenderers.Add(mr);
                    }
                }
            }
            Debug.LogFormat("DisableInvisibleRenderers call, get real renderers, foundcount:{0} notfoundCount:{1} time:{2}", foundRenderers.Count, notFoundKeysCount, System.DateTime.Now);
            //System.IO.File.WriteAllText(@"C:\Users\Myself\Desktop\readPixels.txt", sb.ToString());

            // 材质复原
            for (int i = 0; i < visibleSet.Count; i++)
            {
                MeshRenderer mr = visibleSet[i];
                if (mr == null || mr.sharedMaterials.Length <= 0) continue;

                Material[] mats;
                if (oldRenderMats.TryGetValue(mr, out mats))
                {
                    mr.sharedMaterials = mats;
                }
            }
            Debug.LogFormat("DisableInvisibleRenderers call, recovery materials, time:{0}", System.DateTime.Now);

            // Disable掉实际不可见的物体
            for (int i = 0; i < visibleSet.Count; i++)
            {
                MeshRenderer mr = visibleSet[i];
                if (mr == null || mr.sharedMaterials.Length <= 0) continue;

                if (!foundRenderers.Contains(mr))
                {
                    mr.enabled = false;
                    lastCollects.Add(mr);
                }
            }
            Debug.LogFormat("DisableInvisibleRenderers call, disable all not real renderers, time:{0}", System.DateTime.Now);

            if (texture != null) DestroyImmediate(texture);

            return true;
        }

        private Texture2D CaputureScreenTexture()
        {
            int width = Screen.width, height = Screen.height;
            RenderTexture oldTarget = cam.targetTexture;
            RenderTexture oldActive = RenderTexture.active;

            RenderTexture newTarget = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            RenderTexture.active = newTarget;
            cam.targetTexture = newTarget;
            cam.Render();

            Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            texture.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
            texture.Apply();

            cam.targetTexture = oldTarget;
            RenderTexture.active = oldActive;
            DestroyImmediate(newTarget);
            newTarget = null;

            return texture;
        }

        public void ViewRenders()
        {
            Debug.LogFormat("====================start view renders, time:{0}======================", System.DateTime.Now.ToString());
            int count = 0;
            var rs = FindObjectsOfType<MeshRenderer>();
            for (int i = 0; i < rs.Length; i++)
            {
                MeshRenderer mr = rs[i];
                if (mr != null && mr.isVisible)
                {
                    count++;
                    string path = GetCompletePath(mr.transform);
                    Debug.LogFormat(mr.gameObject, path);
                }
            }
            Debug.LogFormat("====================finish view renders, count:{0} time:{1}======================", count, System.DateTime.Now.ToString());
        }

        private string GetCompletePath(Transform tr)
        {
            if (tr == null)
            {
                Debug.LogError("GetCompletePath error, tr is null");
                return string.Empty;
            }

            string path = tr.name;
            Transform parent = tr.parent;
            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }
            path = tr.gameObject.scene.name + "/" + path;

            return path;
        }

        public void ToggleTerrainsAndTreeDetails()
        {
            var terrains = FindObjectsOfType<Terrain>();
            foreach (Terrain terrain in terrains)
            {
                if (terrain != null)
                {
                    terrain.drawHeightmap = !terrain.drawHeightmap;
                    terrain.drawTreesAndFoliage = !terrain.drawTreesAndFoliage;
                }
            }
        }

        public static string RecordVisibleRenderers()
        {
            Camera cam = Camera.main;
            if (cam == null)
            {
                return "Can't find main camera";
            }

            VisibleRecord record = new VisibleRecord();
            record.fps = 1f / Time.deltaTime;
            record.batches = RuntimeStats.batches;
            record.setPassCalls = RuntimeStats.setPassCalls;
            record.tris = RuntimeStats.triangles;
            record.verts = RuntimeStats.vertices;
            record.camPos = cam.transform.position;
            record.camDir = cam.transform.forward;
            record.camUp = cam.transform.up;
            record.camFov = cam.fieldOfView;
            record.camNear = cam.nearClipPlane;
            record.camFar = cam.farClipPlane;
            record.camOc = cam.useOcclusionCulling ? 1 : 0;
            record.camHdr = cam.allowHDR ? 1 : 0;
            record.camMsaa = cam.allowMSAA ? 1 : 0;

            record.renders = new List<string>();
            var mrs = FindObjectsOfType<MeshRenderer>();
            foreach (MeshRenderer mr in mrs)
            {
                if (mr != null && mr.isVisible)
                {
                    string path = mr.name;
                    Transform parentTr = mr.transform.parent;
                    while (parentTr != null)
                    {
                        path = parentTr.name + "/" + path;
                        parentTr = parentTr.parent;
                    }
                    path = mr.gameObject.scene.name + "/" + path;

                    int vert = 0, tris = 0;
                    MeshFilter mf = mr.GetComponent<MeshFilter>();
                    if (mf != null && mf.sharedMesh != null)
                    {
                        vert = mf.sharedMesh.vertexCount;
                        tris = mf.sharedMesh.triangles.Length;
                    }
                    Vector3 pos = mr.transform.position;

                    path += string.Format("=>vert:{0},tris:{1},pos:{2}", vert, tris, pos);

                    record.renders.Add(path);
                }
            }

            string json = JsonUtility.ToJson(record);
            var now = System.DateTime.Now;
            System.IO.File.WriteAllText(string.Format("visible-{0}-{1}-{2}-{3}.json", now.Month, now.Day, now.Hour, now.Minute), json);

            return "OK";
        }

        [System.Serializable]
        private class VisibleRecord
        {
            public float fps;
            public float batches;
            public float setPassCalls;
            public int tris;
            public int verts;
            public Vector3 camPos;
            public Vector3 camDir;
            public Vector3 camUp;
            public float camFov;
            public float camNear;
            public float camFar;
            public int camOc;
            public int camHdr;
            public int camMsaa;
            public List<string> renders;
        }
    }
}
