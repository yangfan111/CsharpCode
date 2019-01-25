﻿using System;
using System.Collections;
using System.Collections.Generic;
using UltimateFracturing;
using UnityEngine;

namespace ArtPlugins
{
    /// <summary>
    /// 该脚本用于选择并记录建筑内待合并的物件网格
    /// </summary>
    public class MeshCombineBoundsTool : MonoBehaviour
    {
        [Serializable]
        public class BoundST
        {
            public Vector3 center = Vector3.zero;
            public Vector3 size = Vector3.one;
            public Vector3 rotation = Vector3.zero;

            public bool goesFold = true;

            private Dictionary<GameObject, bool> _checkBoundDuplicateDic = null;
            private Dictionary<GameObject, bool> checkBoundDuplicateDic
            {
                get
                {
                    if (_checkBoundDuplicateDic == null)
                    {
                        _checkBoundDuplicateDic = new Dictionary<GameObject, bool>();
                    }
                    return _checkBoundDuplicateDic;
                }
            }

            /// <summary>
            /// 记录包围盒框选住的待合并游戏对象列表
            /// </summary>
            [SerializeField]
            private List<GameObject> goes = new List<GameObject>();

            /// <summary>
            /// 记录包围盒框选住的待合并游戏对象相对于建筑节点的路径列表
            /// </summary>
            [SerializeField]
            private List<string> paths = new List<string>();

            /// <summary>
            /// 记录作为小物件挂载的建筑节点（用于路径查找子物体使用）
            /// </summary>
            [SerializeField]
            private Transform parent = null;

            /// <summary>
            /// 获取列表中指定的游戏对象
            /// </summary>
            public GameObject GetGameObject(int index)
            {
                if (index < 0 || index >= goes.Count || index >= paths.Count)
                {
                    Debug.LogErrorFormat("BoundST.GetGameObject error, index is not fit with goes or paths count, index:{0} goes.count:{1}, paths.count:{2}", index, goes.Count, paths.Count);
                    return null;
                }

                GameObject go = goes[index];
                string path = paths[index];
                if (string.IsNullOrEmpty(path))
                {
                    Debug.LogErrorFormat("BoundST.GetGameObject error, can't get go and path, index:{0}", index);
                }
                else if (parent == null)
                {
                    Debug.LogErrorFormat("BoundST.GetGameObject error, parent is null, index:{0} path:{1}", index, path);
                }
                else if (go == null)             // 对于可能由于替换嵌套预设导致的gameobject为null的情况，尝试从路径记录中从新查找
                {
                    Transform child = parent.Find(path);
                    if (child == null)
                    {
                        Debug.LogErrorFormat("BoundST.GetGameObject error, can't get child, parent:{0} path:{1} index:{2}", parent.name, path, index);
                    }
                    else
                    {
                        go = child.gameObject;
                        goes[index] = go;
                    }
                }
                else                            // 对于go引用为丢失的情况检测路径是否发生变更（比如重命名或者层级修改情况发生时），记录最新路径修改
                {
                    string newPath = MeshCombineBoundsTool.GetRelativeParentPath(go, parent.gameObject);
                    if (string.IsNullOrEmpty(newPath))
                    {
                        Debug.LogErrorFormat("BoundST.GetGameObject error, can't get newPath, parent:{0} child:{1} index:{2}", parent.name, go.name, index);
                    }
                    else if (!newPath.Equals(path))
                    {
                        paths[index] = newPath;
                    }
                }

                return go;
            }

            /// <summary>
            /// 添加一个游戏对象
            /// </summary>
            public void AddOneGameObject(GameObject go)
            {
                if (go == null)
                {
                    Debug.LogError("BoundST.AddOneGameObject error, go is null");
                    return;
                }

                if (parent == null)
                {
                    Debug.LogErrorFormat("BoundST.AddOneGameObject error, parent is null");
                    return;
                }

                if (!go.transform.IsChildOf(parent))
                {
                    Debug.LogErrorFormat("BoundST.AddOneGameObject error, go({0}) is not child of parent({1})", go.name, parent.name);
                    return;
                }

                if (goes.Contains(go)) return;

                string path = MeshCombineBoundsTool.GetRelativeParentPath(go, parent.gameObject);
                if (string.IsNullOrEmpty(path))
                {
                    Debug.LogErrorFormat("BoundST.AddOneGameObject error, path is null, child:{0} parent:{1}", go.name, parent.name);
                    return;
                }

                goes.Add(go);
                paths.Add(path);
            }

            public void RemoveOneGameObject(int index)
            {
                if (index < 0 || index >= goes.Count || index >= paths.Count)
                {
                    Debug.LogErrorFormat("BoundST.RemoveOneGameObject error, index({0}) is not fit with goes.count({1}) or paths.count({2})", index, goes.Count, paths.Count);
                    return;
                }

                goes.RemoveAt(index);
                paths.RemoveAt(index);
            }

            public void AddGameObjects(List<GameObject> list)
            {
                if (list == null || list.Count <= 0) return;

                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] != null) AddOneGameObject(list[i]);
                }
            }

            public BoundST(Vector3 center, Transform parent)
            {
                this.center = center;
                this.parent = parent;
            }

            public int GetGoesCount()
            {
                return goes.Count;
            }

            public bool ContainsGo(GameObject go)
            {
                return goes.Contains(go);
            }

            public bool CheckBoundDuplicate()
            {
                checkBoundDuplicateDic.Clear();
                bool exist = false;
                for (int i = goes.Count - 1; i >= 0; i--)
                {
                    GameObject go = GetGameObject(i);
                    bool remove = false;
                    if (go == null || checkBoundDuplicateDic.ContainsKey(go)) remove = true;
                    else checkBoundDuplicateDic.Add(go, true);

                    if (remove)
                    {
                        RemoveOneGameObject(i);
                        if (!exist) exist = true;
                    }
                }
                return exist;
            }

            public bool CheckIllegalProps()
            {
                bool exist = false;
                for (int i = goes.Count - 1; i >= 0; i--)
                {
                    GameObject go = GetGameObject(i);
                    bool remove = false;
                    if (go == null)
                    {
                        remove = true;
                    }
                    else
                    {
                        MultiTag mt = go.GetComponent<MultiTag>();
                        if (mt == null || mt.IsDoor() || !mt.IsProp() || mt.IsHouse())
                        {
                            remove = true;
                        }
                        if (go.GetComponent<Door>() != null)
                        {
                            remove = true;
                        }
                    }
                    if (remove)
                    {
                        RemoveOneGameObject(i);
                        if (!exist) exist = true;
                    }
                }
                return exist;
            }
        }

        [Serializable]
        public class MeshCombineST
        {
            public List<BoundST> bounds = new List<BoundST>();

            [SerializeField]
            private Transform parent = null;

            private Dictionary<GameObject, List<int>> _checkMcDuplicateDic = null;
            private Dictionary<GameObject, List<int>> checkMcDuplicateDic
            {
                get
                {
                    if (_checkMcDuplicateDic == null)
                    {
                        _checkMcDuplicateDic = new Dictionary<GameObject, List<int>>();
                    }
                    return _checkMcDuplicateDic;
                }
            }

            public MeshCombineST(Vector3 center, Transform parent)
            {
                this.parent = parent;
                bounds.Add(new BoundST(center, parent));
            }

            public void AddOneBounds(Vector3 center)
            {
                bounds.Add(new BoundST(center, parent));
            }

            public void RemoveOneBounds(int delKey)
            {
                if (delKey < 0 || delKey >= bounds.Count)
                {
                    Debug.LogErrorFormat("RemoveOneBounds error, delKey:{0} count:{1}", delKey, bounds.Count);
                    return;
                }
                bounds.RemoveAt(delKey);
            }

            public Dictionary<GameObject, List<int>> CheckMCDuplicate()
            {
                checkMcDuplicateDic.Clear();

                for (int i = 0; i < bounds.Count; i++)
                {
                    int count = bounds[i].GetGoesCount();
                    for (int j = 0; j < count; j++)
                    {
                        List<int> list = null;
                        GameObject go = bounds[i].GetGameObject(j);
                        if (!checkMcDuplicateDic.TryGetValue(go, out list))
                        {
                            list = new List<int>();
                            checkMcDuplicateDic.Add(go, list);
                        }
                        list.Add(i);
                    }
                }

                return checkMcDuplicateDic;
            }
        }

        public bool enableHandle = true;
        public bool enableGizmo = true;
        public bool wireMode = true;
        public Color gizmoColor = Color.yellow;

        public string bakerResultPath = "Assets/Maps/maps/0001/CombineMeshesResults";

        public bool checkError = false;

        public List<MeshCombineST> combines = new List<MeshCombineST>();

        private List<GameObject> allProps = new List<GameObject>();

        private Dictionary<GameObject, List<string>> _checkMCDuplicateDic = null;
        private Dictionary<GameObject, List<string>> checkMcDuplicateDic
        {
            get
            {
                if (_checkMCDuplicateDic == null)
                {
                    _checkMCDuplicateDic = new Dictionary<GameObject, List<string>>();
                }
                return _checkMCDuplicateDic;
            }
        }

        public void AddOneMeshCombine(Vector3 center)
        {
            combines.Add(new MeshCombineST(center, transform));
        }

        public void RemoveOneMeshCombine(int delKey)
        {
            if (delKey < 0 || delKey >= combines.Count)
            {
                Debug.LogErrorFormat("RemoveOneMeshCombine error, delKey:{0} count:{1}", delKey, combines.Count);
                return;
            }
            combines.RemoveAt(delKey);
        }

        public Dictionary<GameObject, List<string>> CheckMCDuplicate()
        {
            checkMcDuplicateDic.Clear();
            for (int i = 0; i < combines.Count; i++)
            {
                MeshCombineST combine = combines[i];
                for (int j = 0; j < combine.bounds.Count; j++)
                {
                    BoundST bound = combine.bounds[j];
                    int count = bound.GetGoesCount();
                    for (int k = 0; k < count; k++)
                    {
                        GameObject go = bound.GetGameObject(k);
                        if (go != null)
                        {
                            List<string> list = null;
                            if (!checkMcDuplicateDic.TryGetValue(go, out list))
                            {
                                list = new List<string>();
                                checkMcDuplicateDic.Add(go, list);
                            }
                            list.Add(string.Format("MC{0}_Bound{1}", i, j));
                        }
                    }
                }
            }
            return checkMcDuplicateDic;
        }

        public void InitAllProps()
        {
            allProps.Clear();

            MultiTag[] mts = GetComponentsInChildren<MultiTag>();
            foreach (MultiTag mt in mts)
            {
                if (mt != null && mt.IsProp())
                {
                    allProps.Add(mt.gameObject);
                }
            }
        }

        public void ClearAllProps()
        {
            allProps.Clear();
        }

        public List<GameObject> CheckUnassignedProps()
        {
            List<GameObject> list = null;
            foreach (GameObject go in allProps)
            {
                if (go == null) continue;

                // 过滤掉物件组下面的物体
                MultiTag[] mts = go.GetComponentsInParent<MultiTag>();
                MultiTag mt = go.GetComponent<MultiTag>();
                bool continueFlag = true;
                foreach (var m in mts)
                {
                    if (m != null && m.IsProp() && !ReferenceEquals(mt, m))
                    {
                        continueFlag = false;
                        break;
                    }
                }
                if (!continueFlag) continue;

                // if ()
                // if(mt!=null)
                // {

                // }
                // foreach(var m in mts)
                // {
                //     // if(m!=null&&)
                //     // if(mt != null && m != null && ReferenceEquals(mt,m))

                // }

                bool exist = false;
                for (int i = 0; i < combines.Count; i++)
                {

                    for (int j = 0; j < combines[i].bounds.Count; j++)
                    {
                        // if (combines[i].bounds[j].gameObjects.Contains(go))
                        if (combines[i].bounds[j].ContainsGo(go))
                        {
                            exist = true;
                            goto Next;
                        }
                    }
                }

            Next:
                if (!exist)
                {
                    if (list == null) list = new List<GameObject>();
                    list.Add(go);
                }
            }
            return list;
        }

        private void OnDrawGizmosSelected()
        {
            if (!enableGizmo) return;

            Gizmos.color = gizmoColor;

            Matrix4x4 oldMatrix = Gizmos.matrix;
            for (int i = 0; i < combines.Count; i++)
            {
                for (int k = 0; k < combines[i].bounds.Count; k++)
                {
                    BoundST st = combines[i].bounds[k];
                    Gizmos.matrix = Matrix4x4.TRS(transform.TransformPoint(st.center), transform.rotation * Quaternion.Euler(st.rotation), st.size);
                    if (wireMode) Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
                    else Gizmos.DrawCube(Vector3.zero, Vector3.one);
                }
            }
            Gizmos.matrix = oldMatrix;
        }

        public static string GetRelativeParentPath(GameObject child, GameObject parent)
        {
            if (child == null || parent == null || !child.transform.IsChildOf(parent.transform)) return null;
            string path = child.name;
            Transform p = child.transform.parent;
            while (!ReferenceEquals(p.gameObject, parent))
            {
                path = p.name + "/" + path;
                p = p.parent;
            }
            return path;
        }
    }
}
