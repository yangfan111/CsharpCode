using System;
using System.Collections.Generic;
using System.Reflection;
using Core.SessionState;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace App.Client.Tools
{
    /// <summary>
    /// 用于木屋冲突的分析系统
    /// </summary>
    public class WoodConflictSystem : AbstractStepExecuteSystem
    {
        private class DetailLocalST
        {
            public int x;
            public int y;
        }

        private static TreePrototype[] treeProtos;
        private static DetailPrototype[] detailProtos;

        private static TreeInstance[] trees;
        private static Dictionary<int, int[,]> details = null;

        private static GameObject collisionGo, waterGo, mapGo;
        private static Terrain terrain;

        public WoodConflictSystem(Contexts context)
        {
            Init();
        }

        private void Init()
        {
            Scene scene = SceneManager.GetActiveScene();
            if (!scene.name.Equals("S003")) return;
            var roots = scene.GetRootGameObjects();

            foreach (var root in roots)
            {
                if (root != null)
                {
                    if (collisionGo == null && root.name.Equals("collision"))
                    {
                        collisionGo = root;
                    }

                    if (waterGo == null && root.name.Equals("AQUAS Waterplane"))
                    {
                        waterGo = root;
                    }

                    if (mapGo == null && root.name.Equals("map"))
                    {
                        mapGo = root;
                    }

                    if (terrain == null)
                    {
                        terrain = root.GetComponent<Terrain>();
                    }
                }
            }

            treeProtos = terrain.terrainData.treePrototypes;
            detailProtos = terrain.terrainData.detailPrototypes;
            trees = terrain.terrainData.treeInstances;
            details = new Dictionary<int, int[,]>();
            for (int i = 0; i < detailProtos.Length; i++)
            {
                var layer = terrain.terrainData.GetDetailLayer(0, 0, terrain.terrainData.detailWidth,
                    terrain.terrainData.detailHeight, i);
                details.Add(i, layer);
            }
        }

        /// <summary>
        /// 每帧执行
        /// </summary>
        protected override void InternalExecute()
        {
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        /// <param name="key">0:collision 1:water 2:map 3:post-fx 4:trees 5:details</param>
        public static string ToggleStatus(int key, bool status)
        {
            switch (key)
            {
                case 0: // collision
                {
                    if (collisionGo == null) return "can't find collision gamobject";
                    collisionGo.SetActive(status);

                    break;
                }
                case 1: // water
                {
                    if (waterGo == null) return "can't find water gameobject";
                    waterGo.SetActive(status);

                    break;
                }
                case 2: // map
                {
                    if (mapGo == null) return "can't find map gameobject";
                    mapGo.SetActive(status);

                    break;
                }
                case 3: // post-fx
                {
                    if (Camera.main == null) return "can't find main camera";

                    Type postProcessLayerTp = null, sunShaftsTp = null, globalFogTp = null;
                    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    foreach (var assembly in assemblies)
                    {
                        if (postProcessLayerTp == null)
                        {
                            postProcessLayerTp =
                                assembly.GetType("UnityEngine.Rendering.PostProcessing.PostProcessLayer");
                        }

                        if (sunShaftsTp == null)
                        {
                            sunShaftsTp = assembly.GetType("UnityStandardAssets.ImageEffects.SunShafts");
                        }

                        if (globalFogTp == null)
                        {
                            globalFogTp = assembly.GetType("LightingBox.Effects.GlobalFog");
                        }
                    }

                    Behaviour postBh = Camera.main.GetComponent(postProcessLayerTp) as Behaviour;
                    Behaviour sunBh = Camera.main.GetComponent(sunShaftsTp) as Behaviour;
                    Behaviour fogBh = Camera.main.GetComponent(globalFogTp) as Behaviour;

                    postBh.enabled = status;
                    sunBh.enabled = status;
                    fogBh.enabled = status;

                    break;
                }
                case 4: // trees
                {
                    if (terrain == null) return "can't find terrain";

                    if (status)
                    {
                        terrain.terrainData.treePrototypes = treeProtos;
                    }
                    else
                    {
                        var protos = new TreePrototype[terrain.terrainData.treePrototypes.Length];
                        for (int i = 0; i < protos.Length; i++)
                        {
                            protos[i] = new TreePrototype();
                            protos[i].prefab = null;
                        }

                        terrain.terrainData.treePrototypes = protos;
                    }

                    break;
                }
                case 5: // details
                {
                    if (terrain == null) return "can't find terrain";

                    if (status)
                    {
                        terrain.terrainData.detailPrototypes = detailProtos;
                    }
                    else
                    {
                        var protos = new DetailPrototype[terrain.terrainData.detailPrototypes.Length];
                        for (int i = 0; i < protos.Length; i++)
                        {
                            protos[i] = new DetailPrototype();
                            protos[i].prototype = null;
                            protos[i].prototypeTexture = null;
                        }

                        terrain.terrainData.detailPrototypes = protos;
                    }

                    break;
                }
                default:
                {
                    return string.Format("error key:{0}", key.ToString());
                }
            }

            return "OK";
        }

        public static string GetTrees()
        {
            if (terrain == null) return "can't find terrain";

            Dictionary<int, int> dict = new Dictionary<int, int>();
            var instances = terrain.terrainData.treeInstances;
            for (int i = 0; i < instances.Length; i++)
            {
                TreeInstance instance = instances[i];
                int key = instance.prototypeIndex;
                if (!dict.ContainsKey(key)) dict.Add(key, 0);
                dict[key]++;
            }

            string value = string.Empty;
            foreach (var pair in dict)
            {
                value += string.Format(" {0}:{1}", pair.Key.ToString(), pair.Value.ToString());
            }

            return string.Format("total:{0} {1}", terrain.terrainData.treeInstanceCount.ToString(), value);
        }

        /// <summary>
        /// 剔除指定数量的树木
        /// </summary>
        public static string DecreaseTrees(int add)
        {
            if (terrain == null) return "can't find terrain";

            if (add <= 0) return "OK";

            var instances = terrain.terrainData.treeInstances;
            List<TreeInstance> list = new List<TreeInstance>(instances);
            while (add > 0 && list.Count > 0)
            {
                int index = Random.Range(0, list.Count);
                list.RemoveAt(index);
                --add;
            }

            terrain.terrainData.treeInstances = list.ToArray();

            return "OK";
        }

        /// <summary>
        /// 剔除指定比例的草
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string DecreaseDetails(int count)
        {
            if (terrain == null) return "can't find terrain";

            if (count <= 0) return "OK";

            terrain.detailObjectDensity = (100 - count) / 100f;

            // // 记录每层的有效数据
            // Dictionary<int, List<DetailLocalST>> layers = new Dictionary<int, List<DetailLocalST>>();
            // for (int y = 0; y < terrain.terrainData.detailHeight; y++)
            // {
            //     for (int x = 0; x < terrain.terrainData.detailWidth; x++)
            //     {
            //         for (int i = 0; i < detailProtos.Length; i++) // each layer
            //         {
            //             var map = terrain.terrainData.GetDetailLayer(0, 0, terrain.terrainData.detailWidth,
            //                 terrain.terrainData.detailHeight, i);
            //             if (map[x, y] > 0)
            //             {
            //                 if (!layers.ContainsKey(i)) layers.Add(i, new List<DetailLocalST>());
            //                 layers[i].Add(new DetailLocalST {x = x, y = y});
            //             }
            //         }
            //     }
            // }
            //
            // // 每层根据移除比例剔除
            // foreach (var pair in layers)
            // {
            //     var map = terrain.terrainData.GetDetailLayer(0, 0, terrain.terrainData.detailWidth,
            //         terrain.terrainData.detailHeight, pair.Key);
            //
            //     int count = (int) (pair.Value.Count * (add / 100f));
            //     while (count > 0)
            //     {
            //         int index = Random.Range(0, pair.Value.Count);
            //         map[pair.Value[index].x, pair.Value[index].y] = 0;
            //         pair.Value.RemoveAt(index);
            //         --count;
            //     }
            //
            //     terrain.terrainData.SetDetailLayer(0, 0, pair.Key, map);
            // }

            return "OK";
        }

        public static string ResetTrees()
        {
            if (terrain == null) return "can't find terrain";

            terrain.terrainData.treePrototypes = treeProtos;
            terrain.terrainData.treeInstances = trees;

            return "OK";
        }

        public static string ResetDetails()
        {
            if (terrain == null) return "can't find terrain";

            // terrain.terrainData.detailPrototypes = detailProtos;
            // for (int i = 0; i < detailProtos.Length; i++)
            // {
            //     terrain.terrainData.SetDetailLayer(0, 0, i, details[i]);
            // }

            terrain.detailObjectDensity = 1f;

            return "OK";
        }

        public static string GetGlobalVolumeProfile()
        {
            Type postProfileTp = null, postVolumeTp = null;
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (postProfileTp == null)
                {
                    postProfileTp = assembly.GetType("UnityEngine.Rendering.PostProcessing.PostProcessProfile");
                }

                if (postVolumeTp == null)
                {
                    postVolumeTp = assembly.GetType("UnityEngine.Rendering.PostProcessing.PostProcessVolume");
                }
            }

            Object volumeObj = Object.FindObjectOfType(postVolumeTp);
            FieldInfo sharedFieldInfo =
                postVolumeTp.GetField("sharedProfile", BindingFlags.Public | BindingFlags.Instance);
            Object profileObj = sharedFieldInfo.GetValue(volumeObj) as Object;
            return profileObj.name;
        }
    }
}