using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SplineTools
{
    /// <summary>
    /// 自定义样条曲线弯曲多个模型，需要考虑的两点：
    /// 1. 模型的迭代
    /// 2. LOD功能的支持
    /// </summary>
    [ExecuteInEditMode]
    [SelectionBase]
    [RequireComponent(typeof(Spline))]
    public class SplineModels : MonoBehaviour
    {
        /// <summary>
        /// Curve段中模型摆放信息结构体
        /// </summary>
        [System.Serializable]
        public class CurveST
        {
            /// <summary>
            /// 模型预制体
            /// </summary>
            public GameObject modelPrefab;

            /// <summary>
            /// 曲线段模型数量
            /// </summary>
            public int numbers = 5;

            /// <summary>
            /// 布局物体的旋转度
            /// </summary>
            public Vector3 rotation = new Vector3(-90f, 0f, 0f);

            /// <summary>
            /// 布局物体的开始处缩放比例
            /// </summary>
            public float startScale = 1f;

            /// <summary>
            /// 布局物体的结尾处缩放比例
            /// </summary>
            public float endScale = 1f;

            /// <summary>
            /// 相对样条曲线Y方向的偏离
            /// </summary>
            public float yOffset;

            /// <summary>
            /// 相对样条曲线Z方向的偏离
            /// </summary>
            public float zOffset;

            /// <summary>
            /// 曲线开始处的翻转度
            /// </summary>
            public float startRoll;

            /// <summary>
            /// 曲线结尾处的翻转度
            /// </summary>
            public float endRoll;

            /// <summary>
            /// 投射区域相对曲线开头处的偏移[0,1]
            /// </summary>
            public float startOffset = 0f;

            /// <summary>
            /// 投射区域相对曲线结尾处的偏移[0,1]
            /// </summary>
            public float endOffset = 1f;

            /// <summary>
            /// 模型间隔（尺寸标识相对于整个曲线长度的比例）
            /// </summary>
            public float space = 0f;

            /// <summary>
            /// 是否强制消去子物体的旋转
            /// </summary>
            public bool forceChildNoRotation = true;

            /// <summary>
            /// 记录生成的模型列表
            /// </summary>
            public List<GameObject> models = new List<GameObject>();

            /// <summary>
            /// 记录gui界面curve是否展开
            /// </summary>
            [System.NonSerialized]
            public bool isExpandCurveGui = false;

            /// <summary>
            /// 记录gui界面models是否展开
            /// </summary>
            [System.NonSerialized]
            public bool isExpandModelsGui = false;
        }

        /// <summary>
        /// 样条曲线
        /// </summary>
        public Spline spline;

        /// <summary>
        /// 记录曲线段信息列表
        /// </summary>
        public List<CurveST> curves = new List<CurveST>();

        public void InitCurves()
        {
            if (curves.Count > 0)
            {
                RebuildMeshes();
                return;
            }

            curves.Clear();
            int count = spline.GetNodes().Count;
            for (int i = 0; i < count - 1; i++)
            {
                curves.Add(new CurveST());
            }
        }

        private void OnEnable()
        {
            spline = GetComponent<Spline>();
            spline.rebuildSplineEvent.AddListener(OnRebuildSplineEvent);
            spline.nodeCountChangeTypeEvent.AddListener(OnNodeCountChangeTypeEvent);
        }

        private void OnDisable()
        {
            spline.rebuildSplineEvent.RemoveListener(OnRebuildSplineEvent);
            spline.nodeCountChangeTypeEvent.RemoveListener(OnNodeCountChangeTypeEvent);
        }

        private void OnRebuildSplineEvent()
        {
            InitCurves();
        }

        private void OnNodeCountChangeTypeEvent(NodeCountChangeType tp, int index)
        {
            switch (tp)
            {
                case NodeCountChangeType.AddFront:
                    {
                        curves.Insert(0, new CurveST());
                    }
                    break;
                case NodeCountChangeType.AddBack:
                    {
                        curves.Add(new CurveST());
                    }
                    break;
                case NodeCountChangeType.Delete:
                    {
                        int pos = index == curves.Count ? index - 1 : index;
                        curves.RemoveAt(pos);
                    }
                    break;
                case NodeCountChangeType.Insert:
                    {
                        curves.Insert(index, new CurveST());
                    }
                    break;
            }
        }

        public void RebuildMeshes()
        {
            for (int i = 0; i < curves.Count; i++)
            {
                RebuildCurve(i, true);
            }
        }

        public void RebuildCurve(int curveIndex, bool repro)
        {
            var curveSt = curves[curveIndex];
            if (curveSt.modelPrefab == null) return;

            if (repro)
            {
                for (int i = 0; i < curveSt.models.Count; i++)
                {
                    GameObject go = curveSt.models[i];
                    if (go == null) continue;

                    if (Application.isPlaying) Destroy(go);
                    else DestroyImmediate(go);
                }
                curveSt.models.Clear();

                for (int i = 0; i < curveSt.numbers; i++)
                {
                    GameObject go = Instantiate<GameObject>(curveSt.modelPrefab, transform.position, Quaternion.identity, transform);
                    go.transform.localScale = Vector3.one;
                    go.name = string.Format("curve{0}_{1} ({2})", curveIndex, curveSt.modelPrefab, i);
                    if (curveSt.forceChildNoRotation)
                    {
                        foreach (Transform ch in go.transform)
                            ch.localRotation = Quaternion.identity;
                    }
                    FbxModelBender bender = go.GetComponent<FbxModelBender>();
                    if (bender == null) bender = go.AddComponent<FbxModelBender>();
                    curveSt.models.Add(go);
                }
            }

            if (!CheckModel(curveSt.modelPrefab)) return;

            float len = (curveSt.endOffset - curveSt.startOffset - curveSt.space * (curveSt.numbers - 1)) / curveSt.numbers;
            var curveList = spline.GetCurves();
            CubicBezierCurve bezier = curveList[curveIndex];

            // produce every model
            for (int i = 0; i < curveSt.numbers; i++)
            {
                float sOffset = (i - 0) * (len + curveSt.space) + curveSt.startOffset;
                float eOffset = sOffset + len;
                // Debug.LogErrorFormat("i:{0} models:{1} numbers:{2}", i, curveSt.models.Count, curveSt.numbers);
                GameObject go = curveSt.models[i];
                FbxModelBender bender = go.GetComponent<FbxModelBender>();
                bender.SetCurve(bezier, false);
                bender.SetParentRotation(transform.rotation);
                bender.SetSourceRotation(Quaternion.Euler(curveSt.rotation), false);
                bender.SetTranslation(new Vector3(0, curveSt.yOffset, curveSt.zOffset), false);
                bender.SetStartScale(curveSt.startScale, false);
                bender.SetEndScale(curveSt.endScale, false);
                bender.SetStartRoll(curveSt.startRoll, false);
                bender.SetEndRoll(curveSt.endRoll, false);
                bender.SetStartOffset(sOffset, false);
                bender.SetEndOffset(eOffset);
            }
        }

        private bool CheckModel(GameObject baseGo)
        {
            if (baseGo == null)
            {
                Debug.LogError("CheckModel error, baseGo is null");
                return false;
            }

            Dictionary<string, Mesh> baseMeshes = new Dictionary<string, Mesh>();
            var mcs = baseGo.GetComponentsInChildren<MeshCollider>(true);
            foreach (var mc in mcs)
            {
                if (mc == null) continue;

                if (baseMeshes.ContainsKey(mc.name))
                {
                    Debug.LogError("请确保网格物体名称无重复", baseGo);
                    return false;
                }

                baseMeshes.Add(mc.name, mc.sharedMesh);
            }
            var mfs = baseGo.GetComponentsInChildren<MeshFilter>(true);
            foreach (var mf in mfs)
            {
                if (mf == null) continue;

                if (baseMeshes.ContainsKey(mf.name))
                {
                    Debug.LogError("请确保网格物体名称无重复", baseGo);
                    return false;
                }

                baseMeshes.Add(mf.name, mf.sharedMesh);
            }

            return baseMeshes.Count > 0;
        }
    }
}