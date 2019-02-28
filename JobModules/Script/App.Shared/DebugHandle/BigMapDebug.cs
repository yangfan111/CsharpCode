using com.cpkf.yyjd.tools.util.collection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace App.Shared.DebugHandle
{
    public class BigMapDebug
    {
        private static Dictionary<string, TreePrototype[]> treeDic = new Dictionary<string, TreePrototype[]>();
        private static Dictionary<string, DetailPrototype[]> grassDic = new Dictionary<string, DetailPrototype[]>();

        private static int type;
        private static int count;

        public static int SmoothFactor = 2;

        private static Dictionary<string, int> countDic = new Dictionary<string, int>();

        private static string Smooth(string ls, string factor)
        {
            if (ls == "sf")
            {
                if (factor != null)
                {
                    SmoothFactor = int.Parse(factor);

                }
                return "Smooth Factor " + SmoothFactor;
            }

            return null;
        }

        private static string ShowComponent(string ls, string name, string field)
        {
            if (ls == "get")
            {
                var item = GameObject.Find(name);
                if (item == null) return "not found";
                List<string> list = new List<string>();
                foreach (var b in item.GetComponents<MonoBehaviour>())
                {
                    list.Add(b.ToString());
                    if (string.IsNullOrEmpty(field) == false)
                    {
                        var f = b.GetType().GetField(field);
                        if (f != null)
                            list.Add(field + "=" + f.GetValue(b).ToString());
                    }
                }

                return string.Join("\n", list.ToArray());
            }
            else if (ls == "getm")
            {
                List<string> list = new List<string>();
                var item = GameObject.Find(name);
                if (item == null) return "not found";
                foreach (var r in item.GetComponentsInChildren<Renderer>())
                {
                    foreach (var m in r.materials)
                    {
                        list.Add(m.name + ":" + m.shader.name);
                        if (string.IsNullOrEmpty(field) == false)
                        {

                            list.Add(m.GetTexture(field).ToString());
                        }
                    }
                }

                return string.Join("\n", list.ToArray());
            }

            return null;
        }

        private static string Culling(string ls, string factor)
        {
            if (ls == "ci")
            {
                if (factor != null)
                {
                    SharedConfig.CullingInterval = int.Parse(factor);

                }
                return "CullingInterval=" + factor;
            }

            if (ls == "coff")
            {
                if (factor != null)
                {
                    SharedConfig.CullingOn = factor != "1";

                }
                return "CullingOff=" + factor;
            }

            if (ls == "cr")
            {
                if (factor != null)
                {
                    SharedConfig.CullingRange = int.Parse(factor);

                }
                return "CullingRange=" + factor;
            }

            if (ls == "crs")
            {
                if (factor != null)
                {
                    SharedConfig.CullingRangeSmall = int.Parse(factor);

                }
                return "CullingRangeSmall=" + factor;
            }

            if (ls == "crm")
            {
                if (factor != null)
                {
                    SharedConfig.CullingRangeMid = int.Parse(factor);
                }
                return "CullingRangeMid=" + factor;
            }

            return null;
        }

        private static string Layer(PlayerEntity player, string ls, string layer, string dis)
        {
            if (ls == "layer")
            {
                float[] fs = player.cameraObj.MainCamera.layerCullDistances;
                int d = int.Parse(dis);
                fs[LayerMask.NameToLayer(layer)] = d;
                float[] newFs = new float[fs.Length];
                for (int i = 0; i < fs.Length; i++)
                {
                    newFs[i] = fs[i];
                }
                player.cameraObj.MainCamera.layerCullDistances = newFs;

                return "layer  " + layer + "=" + dis;
            }

            return null;
        }

        private static string Scripts(string ls)
        {
            if (ls == "sp")
            {
                Accumulator<string> acc = new Accumulator<string>();
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    Scene scene = SceneManager.GetSceneAt(i);

                    foreach (GameObject obj in scene.GetRootGameObjects())
                    {
                        Transform[] trans = obj.GetComponentsInChildren<Transform>();
                        foreach (Transform tran in trans)
                        {
                            Component[] monos = tran.gameObject.GetComponents<Component>();
                            foreach (Component mono in monos)
                            {
                                if (mono != null)
                                {
                                    if (HasEnabled(mono))
                                    {
                                        acc.AddKey(mono.GetType().Name);
                                    }
                                }
                            }
                        }
                    }
                }

                List<string> list = new List<string>();
                foreach (string name in acc.KeysSortedByValue())
                {
                    list.Add(name + "=" + acc.GetCount(name));
                }

                return string.Join("\n", list.ToArray());
            }

            return null;
        }

        private static int EnableComponent(Scene scene, GameObject obj, string name, bool enable, bool all)
        {
            int count = 0;

            if (scene.name.Contains("002") || all)
            {
                Transform[] ts = obj.GetComponentsInChildren<Transform>();
                string objName = null;
                string comName = null;
                if (!string.IsNullOrEmpty(name))
                {
                    if (name.Contains("."))
                    {
                        string[] ss = name.Split('.');
                        if (ss.Length == 2)
                        {
                            objName = ss[0].Trim();
                            comName = ss[1].Trim();
                        }
                    }
                    else
                    {
                        comName = name.Trim();
                    }
                }


                foreach (Transform t in ts)
                {
                    if (string.IsNullOrEmpty(objName) || t.gameObject.name.Contains(objName))
                    {
                        foreach (Component com in t.gameObject.GetComponents<Component>())
                        {
                            if (com != null)
                            {
                                if (string.IsNullOrEmpty(comName) || com.GetType().Name.Contains(comName))
                                {
                                    if (HasEnabled(com))
                                    {
                                        PropertyInfo p = proDic[com.GetType().Name];
                                        p.SetValue(com, enable, null);
                                        count++;
                                    }
                                }
                            }

                        }
                    }
                }

            }

            return count;
        }

        private static Dictionary<string, bool> enableDic = new Dictionary<string, bool>();
        private static Dictionary<string, PropertyInfo> proDic = new Dictionary<string, PropertyInfo>();

        private static bool HasEnabled(Component com)
        {
            string key = com.GetType().Name;
            if (!enableDic.ContainsKey(key))
            {
                foreach (PropertyInfo f in com.GetType().GetProperties())
                {
                    if (f.Name == "enabled")
                    {
                        enableDic.Add(key, true);
                        proDic.Add(key, f);
                        break;
                    }
                }

                if (!enableDic.ContainsKey(key))
                {
                    enableDic.Add(key, false);
                }
            }

            return enableDic[com.GetType().Name];
        }

        private static string HandleEffect(string cmd)
        {
            Scene scene = SceneManager.GetActiveScene();
            foreach (GameObject obj in scene.GetRootGameObjects())
            {
                if (obj.name == "Main Camera")
                {
                    Component[] cs = obj.GetComponents<Component>();
                    foreach (Component c in cs)
                    {

                    }
                }
            }
            return null;
        }

        private static string HandleNear(PlayerEntity player, string cmd)
        {
            Scene scene = GetScene(player);
            if (scene != null && scene.isLoaded)
            {
                List<Transform> buds = new List<Transform>();
                List<TreeInstance> trees = new List<TreeInstance>();
                Terrain t = null;
                foreach (GameObject obj in scene.GetRootGameObjects())
                {
                    if (obj.GetComponent<Terrain>() != null)
                    {
                        t = obj.GetComponent<Terrain>();
                        if (t != null && !treeDic.ContainsKey(scene.name))
                        {
                            treeDic.Add(scene.name, t.terrainData.treePrototypes);
                            grassDic.Add(scene.name, t.terrainData.detailPrototypes);
                        }
                        trees.AddRange(t.terrainData.treeInstances);
                    }
                    else
                    {
                        foreach (Transform tf in obj.GetComponentsInChildren<Transform>(true))
                        {
                            if (GetBudName(tf).Contains("Bud"))
                            {
                                buds.Add(tf);
                            }
                        }

                    }
                }

                buds.Sort(new BudComparer(player.position.Value));
                trees.Sort(new TreeComparer(player.position.Value));

                if (cmd == "tr0")
                {
                    HandleTree(scene, t, treeDic[scene.name][trees[0].prototypeIndex].prefab.name, true);

                    return "hide " + treeDic[scene.name][trees[0].prototypeIndex].prefab.name;
                }
                else if (cmd == "tr1")
                {
                    HandleTree(scene, t, treeDic[scene.name][trees[0].prototypeIndex].prefab.name, false);

                    return "show " + treeDic[scene.name][trees[0].prototypeIndex].prefab.name;
                }
                else if (cmd == "bud0")
                {
                    foreach (Transform tf in buds)
                    {
                        if (GetBudName(tf).Contains(GetBudName(buds[0])))
                        {
                            tf.gameObject.SetActive(false);
                        }
                    }
                    return "hide " + GetBudName(buds[0]);
                }
                else if (cmd == "bud1")
                {
                    foreach (Transform tf in buds)
                    {
                        if (GetBudName(tf).Contains(GetBudName(buds[0])))
                        {
                            tf.gameObject.SetActive(true);
                        }
                    }
                    return "show " + GetBudName(buds[0]);
                }

            }


            return null;
        }

        private static string GetBudName(Transform tf)
        {
            Transform p = tf.parent;
            while (p != null)
            {
                if (p.gameObject.name.Contains("Bud"))
                {
                    return "";
                }
                p = p.parent;
            }
            if (tf.gameObject.name.Contains("("))
            {
                return tf.gameObject.name.Substring(0, tf.gameObject.name.IndexOf('('));
            }
            return tf.gameObject.name;
        }


        private static Scene GetScene(PlayerEntity player)
        {
            return SceneManager.GetSceneByName("002 " + (int)(player.position.Value.x + 4000) / 1000 + "x" + (int)(player.position.Value.z + 4000) / 1000);
        }

        private static string SaveTerrainData()
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);

                foreach (GameObject obj in scene.GetRootGameObjects())
                {
                    Terrain t = obj.GetComponent<Terrain>();
                    if (t != null)
                    {
                        TerrainData data = t.terrainData;
                        for (int j = 0; j < data.detailPrototypes.Length; i++)
                        {
                            data.GetDetailLayer(0, 0, data.detailWidth, data.detailHeight, j);
                        }
                    }
                }
            }

            return "ok";
        }

        public static string HandleCommand(PlayerEntity player, string[] args)
        {
            countDic.Clear();
            string ls = Smooth(args[0], args.Length > 1 ? args[1] : null);
            if (ls != null)
            {
                return ls;
            }
            if (args.Length > 0)
            {
                if (args[0] == "savedata")
                {
                    ls = SaveTerrainData();
                }
            }
            if (args.Length > 2)
            {
                ls = ShowComponent(args[0], args[1], args[2]);
            }
            else if (args.Length == 2)
            {
                ls = ShowComponent(args[0], args[1], null);
            }
            if (ls != null)
            {
                return ls;
            }

            ls = Scripts(args[0]);
            if (ls != null)
            {
                return ls;
            }

            ls = HandleNear(player, args[0]);
            if (ls != null)
            {
                return ls;
            }

            if (args.Length > 2)
            {
                ls = Layer(player, args[0], args[1], args[2]);
                if (ls != null)
                {
                    return ls;
                }
            }
            if (args.Length > 1)
            {
                ls = Culling(args[0], args[1]);
                if (ls != null)
                {
                    return ls;
                }
            }



            bool open = args[0].Trim() == "1";
            string scope = args[1].Trim();
            string name = "";
            if (args.Length > 2)
            {
                name = args[2].Trim();
            }

            type = 0;
            count = 0;

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);


                foreach (GameObject obj in scene.GetRootGameObjects())
                {
                    Terrain t = obj.GetComponent<Terrain>();

                    if (t != null && !treeDic.ContainsKey(scene.name))
                    {
                        treeDic.Add(scene.name, t.terrainData.treePrototypes);
                        grassDic.Add(scene.name, t.terrainData.detailPrototypes);
                    }

                    switch (scope)
                    {
                        case "te":
                            if (string.IsNullOrEmpty(name))
                            {
                                if (t != null && scene.name != "ClientScene")
                                {
                                    t.enabled = open;
                                }
                            }
                            else if ("b" == name)
                            {
                                if (t != null && scene.name != "ClientScene")
                                {
                                    t.drawHeightmap = open;
                                }
                            }

                            break;
                        case "tr":
                            if (t != null && scene.name != "ClientScene")
                            {
                                HandleTree(scene, t, name, !open);
                                count += t.terrainData.treeInstances.Length;
                            }
                            break;
                        case "de":
                            if (t != null && scene.name != "ClientScene")
                            {
                                foreach (DetailPrototype ti in grassDic[scene.name])
                                {
                                    if (ti != null && ti.prototypeTexture != null && ti.prototypeTexture.name.Contains(name))
                                    {
                                        AddCount("grass - " + ti.prototypeTexture.name);
                                    }
                                }
                                if (string.IsNullOrEmpty(name))
                                {
                                    if (open)
                                    {
                                        t.terrainData.detailPrototypes = grassDic[scene.name];
                                    }
                                    else
                                    {
                                        DetailPrototype[] dps = new DetailPrototype[grassDic[scene.name].Length];
                                        for (int j = 0; j < dps.Length; j++)
                                        {
                                            dps[j] = new DetailPrototype();
                                        }
                                        t.terrainData.detailPrototypes = dps;
                                    }
                                }
                                else
                                {
                                    HandleGrass(scene, t, name, !open);
                                }

                            }
                            break;
                        case "bud":
                            if (t == null && scene.name.Contains("x"))
                            {
                                HandleBuild(scene, obj, name, !open);
                                count += scene.rootCount;
                            }
                            break;
                        case "sp":
                            count += EnableComponent(scene, obj, name, open, false);
                            break;
                        case "spa":
                            count += EnableComponent(scene, obj, name, open, true);
                            break;
                        case "game":
                            if (t == null && scene.name.Contains("ClientScene"))
                            {
                                if (obj.name.Contains("GameCon"))
                                {
                                    MonoBehaviour[] bs = obj.GetComponents<MonoBehaviour>();
                                    foreach (MonoBehaviour b in bs)
                                    {
                                        if (b != null && b.GetType().Name.Contains("ClientGame"))
                                        {
                                            b.enabled = open;
                                            count++;
                                        }
                                    }
                                }
                            }
                            break;
                        case "entity":
                            if (t == null && scene.name.Contains("ClientScene"))
                            {
                                if (obj.name.Contains("DefaultGo"))
                                {
                                    obj.SetActive(open);
                                    count++;
                                }
                            }
                            break;
                        case "effect":
                            if (scene == SceneManager.GetActiveScene())
                            {
                                if (obj.name == "Main Camera")
                                {
                                    MonoBehaviour[] cs = obj.GetComponents<MonoBehaviour>();
                                    foreach (MonoBehaviour c in cs)
                                    {
                                        if (c != null && c.GetType().Name.Contains(name))
                                        {
                                            c.enabled = open;
                                        }
                                    }
                                }
                                else
                                {
                                    HandleBuild(scene, obj, name, !open);
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            List<string> list = new List<string>();
            foreach (string key in countDic.Keys)
            {
                list.Add(key + "=" + countDic[key]);
            }

            return count + "个改动\n" + string.Join("\n", list.ToArray());
        }

        private static void AddCount(string key)
        {
            if (!countDic.ContainsKey(key))
            {
                countDic.Add(key, 0);
            }
            countDic[key] = countDic[key] + 1;
        }

        private static void HandleBuild(Scene scene, GameObject obj, string tree, bool close)
        {
            if (obj.name.Contains(tree))
            {
                obj.SetActive(!close);
                AddCount(obj.name);
            }
        }

        private static void HandleGrass(Scene scene, Terrain t, string tree, bool close)
        {
            DetailPrototype[] old = t.terrainData.detailPrototypes;
            DetailPrototype[] ts = new DetailPrototype[old.Length];

            for (int i = 0; i < ts.Length; i++)
            {
                ts[i] = new DetailPrototype();
                ts[i].bendFactor = old[i].bendFactor;
                if (grassDic[scene.name][i].prototypeTexture != null && grassDic[scene.name][i].prototypeTexture.name.Contains(tree))
                {
                    if (close)
                    {
                        ts[i].prototypeTexture = null;
                    }
                    else
                    {
                        ts[i].prototypeTexture = grassDic[scene.name][i].prototypeTexture;
                    }
                }
                else
                {
                    ts[i].prototypeTexture = old[i].prototypeTexture;
                }
            }

            t.terrainData.detailPrototypes = ts;
        }

        private static void HandleTree(Scene scene, Terrain t, string tree, bool close)
        {
            TreePrototype[] old = t.terrainData.treePrototypes;
            TreePrototype[] ts = new TreePrototype[old.Length];

            foreach (TreeInstance ti in t.terrainData.treeInstances)
            {
                if (treeDic[scene.name][ti.prototypeIndex] != null && treeDic[scene.name][ti.prototypeIndex].prefab != null)
                {
                    if (treeDic[scene.name][ti.prototypeIndex].prefab.name.Contains(tree))
                    {
                        AddCount("tree - " + treeDic[scene.name][ti.prototypeIndex].prefab.name);
                    }
                }
            }

            for (int i = 0; i < ts.Length; i++)
            {
                ts[i] = new TreePrototype();
                ts[i].bendFactor = old[i].bendFactor;
                if (treeDic[scene.name][i].prefab != null && treeDic[scene.name][i].prefab.name.Contains(tree))
                {
                    if (close)
                    {
                        ts[i].prefab = new GameObject();
                    }
                    else
                    {
                        ts[i].prefab = treeDic[scene.name][i].prefab;
                    }
                }
                else
                {
                    ts[i].prefab = old[i].prefab;
                }
            }

            t.terrainData.treePrototypes = ts;
        }
    }

    class BudComparer : IComparer<Transform>
    {
        private Vector3 pos;

        public BudComparer(Vector3 pos)
        {
            this.pos = pos;
        }

        public int Compare(Transform x, Transform y)
        {
            return Distance(pos, x.position) - Distance(pos, y.position);
        }

        private static int Distance(Vector3 v1, Vector3 v2)
        {
            return (int)((v1.x - v2.x) * (v1.x - v2.x) + (v1.z - v2.z) * (v1.z - v2.z));
        }
    }

    class TreeComparer : IComparer<TreeInstance>
    {
        private Vector3 pos;

        public TreeComparer(Vector3 pos)
        {
            this.pos = pos;
        }

        public int Compare(TreeInstance x, TreeInstance y)
        {
            Vector2 v = new Vector2(pos.x, pos.z);
            while (v.x < 0)
            {
                v.x = v.x + 1000;
            }
            while (v.x > 1000)
            {
                v.x = v.x - 1000;
            }
            while (v.y < 0)
            {
                v.y = v.y + 1000;
            }
            while (v.y > 1000)
            {
                v.y = v.y - 1000;
            }

            return Distance(v, x.position) - Distance(v, y.position);
        }

        private static int Distance(Vector2 v1, Vector3 v2)
        {
            return (int)((v1.x - v2.x * 1000) * (v1.x - v2.x * 1000) + (v1.y - v2.z * 1000) * (v1.y - v2.z * 1000));
        }

    }
}
