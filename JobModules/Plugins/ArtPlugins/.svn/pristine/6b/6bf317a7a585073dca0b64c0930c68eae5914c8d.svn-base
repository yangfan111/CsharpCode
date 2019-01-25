using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SplineTools
{
    /// <summary>
    /// 自定义样条曲线弯曲模型，需要考虑的两点：
    /// 1. 模型的迭代
    /// 2. LOD功能的支持
    /// </summary>
    [ExecuteInEditMode]
    [SelectionBase]
    [RequireComponent(typeof(Spline))]
    public class SplineModel : MonoBehaviour
    {
        /// <summary>
        /// 模型预制体
        /// </summary>
        public GameObject modelPrefab;

        /// <summary>
        /// 记录局部替换需要的基础模型
        /// </summary>
        /// <typeparam name="int">曲线段索引</typeparam>
        /// <typeparam name="GameObject">基础模型</typeparam>
        public Dictionary<int, GameObject> specialModels = new Dictionary<int, GameObject>();

        /// <summary>
        /// 样条曲线
        /// </summary>
        public Spline spline;

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
        public float curveStartOffset = 0f;

        /// <summary>
        /// 投射区域相对曲线结尾处的偏移[0,1]
        /// </summary>
        public float curveEndOffset = 1f;

        /// <summary>
        /// 是否强制消去子物体的旋转
        /// </summary>
        public bool forceChildNoRotation = true;

        /// <summary>
        /// 产生的待布局物体列表
        /// </summary>
        private List<GameObject> models = new List<GameObject>();

        /// <summary>
        /// 是否需要更新
        /// </summary>
        // private bool needUpdate = true;

        private void OnEnable()
        {
            spline = GetComponent<Spline>();
            spline.nodeCountChanged.AddListener(OnSplineCurveChanged);
        }

        private void OnDisable()
        {
            spline.nodeCountChanged.RemoveListener(OnSplineCurveChanged);
        }

        /// <summary>
        /// 监听样条曲线的更改
        /// </summary>
        private void OnSplineCurveChanged()
        {
            RebuildMeshes();
        }

        public List<GameObject> GetModels()
        {
            return models;
        }

        /// <summary>
        /// 重构变形体
        /// </summary>
        public void RebuildMeshes()
        {
            if (spline == null || !gameObject.activeSelf || !enabled) return;

            // 清理先前物体列表
            for (int i = 0; i < models.Count; i++)
            {
                GameObject go = models[i];
                if (go == null) continue;
                if (Application.isPlaying) Destroy(go);
                else DestroyImmediate(go);
            }
            models.Clear();

            if (modelPrefab == null) return;

            // 核实待变形的基础网格模型
            {
                if (!CheckModel(modelPrefab)) return;

                foreach (var baseGo in specialModels.Values)
                {
                    if (!CheckModel(baseGo)) return;
                }
            }

            // 曲线段
            var curves = spline.GetCurves();
            for (int i = 0; i < curves.Count; i++)
            {
                CubicBezierCurve curve = curves[i];

                // 实例化模型物体
                GameObject model = null;
                specialModels.TryGetValue(i, out model);
                if (model == null) model = modelPrefab;
                string name = string.Format("{0} ({1})", model.name, i);
                GameObject go = Instantiate<GameObject>(model, transform.position, Quaternion.identity, transform);
                go.transform.localScale = Vector3.one;
                go.name = name;
                if (forceChildNoRotation)
                {
                    foreach (Transform ch in go.transform)
                    {
                        ch.localRotation = Quaternion.identity;
                    }
                }

                // 设置弯曲属性
                FbxModelBender bender = go.GetComponent<FbxModelBender>();
                if (bender == null) bender = go.AddComponent<FbxModelBender>();
                bender.SetCurve(curve, false);
                bender.SetParentRotation(transform.rotation,false);
                bender.SetSourceRotation(Quaternion.Euler(rotation), false);
                bender.SetTranslation(new Vector3(0, yOffset, zOffset), false);
                bender.SetStartScale(startScale, false);
                bender.SetEndScale(endScale, false);
                bender.SetStartRoll(startRoll, false);
                bender.SetEndRoll(endRoll, false);
                bender.SetStartOffset(curveStartOffset, false);
                bender.SetEndOffset(curveEndOffset);

                // 添加到模型列表
                models.Add(go);
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