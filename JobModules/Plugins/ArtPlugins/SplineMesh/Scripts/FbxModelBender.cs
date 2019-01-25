using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SplineTools
{
    /// <summary>
    /// 自定义的用于工程的fbx模型弯曲器
    /// </summary>
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public class FbxModelBender : MonoBehaviour
    {
        /// <summary>
        /// 网格的旋转
        /// </summary>
        private Quaternion sourceRotation;

        /// <summary>
        /// 父物体旋转
        /// </summary>
        private Quaternion parentRotation;

        /// <summary>
        /// 原始网格的位移
        /// </summary>
        private Vector3 sourceTranslation;

        /// <summary>
        /// 起始处的缩放
        /// </summary>
        private float startScale = 1f;

        /// <summary>
        /// 结尾处的缩放
        /// </summary>
        private float endScale = 1f;

        /// <summary>
        /// 起始处的翻转度
        /// </summary>
        private float startRoll;

        /// <summary>
        /// 结尾处的翻转度
        /// </summary>
        private float endRoll;

        /// <summary>
        /// 投射区域相对曲线开头处的偏移[0,1]
        /// </summary>
        private float startOffset = 0f;

        /// <summary>
        /// 投射区域相对曲线结尾处的偏移[0,1]
        /// </summary>
        private float endOffset = 1f;

        /// <summary>
        /// 样条曲线
        /// </summary>
        private CubicBezierCurve curve;

        /// <summary>
        /// 网格记录结构体
        /// </summary>
        public class meshST
        {
            public Mesh baseMesh;
            public Mesh newMesh;
            public GameObject go;
            public bool isCollider;
        }

        /// <summary>
        /// 网格列表记录
        /// </summary>
        private Dictionary<string, meshST> meshes = new Dictionary<string, meshST>();

        private void Awake()
        {
            meshes.Clear();
            var mcs = GetComponentsInChildren<MeshCollider>();
            foreach (var mc in mcs)
            {
                if (mc == null || mc.sharedMesh == null) continue;

                string key = mc.name;
                if (meshes.ContainsKey(key))
                {
                    Debug.LogError("FbxModelBender Init error, 网格物体重名", gameObject);
                    continue;
                }

                meshST st = new meshST()
                {
                    baseMesh = mc.sharedMesh,
                    newMesh = new Mesh(),
                    go = mc.gameObject,
                    isCollider = true,
                };
                meshes.Add(key, st);
            }
            var mfs = GetComponentsInChildren<MeshFilter>();
            foreach (var mf in mfs)
            {
                if (mf == null || mf.sharedMesh == null) continue;

                string key = mf.name;
                if (meshes.ContainsKey(key))
                {
                    Debug.LogError("FbxModelBender Init error, 网格物体重名", gameObject);
                    continue;
                }

                meshST st = new meshST()
                {
                    baseMesh = mf.sharedMesh,
                    newMesh = new Mesh(),
                    go = mf.gameObject,
                    isCollider = false,
                };
                meshes.Add(key, st);
            }
        }

        private void OnDestroy()
        {
            if (curve != null) curve.changed.RemoveListener(OnDeformedModelChange);
            meshes.Clear();
        }

        /// <summary>
        /// 相应目标模型的更改
        /// </summary>
        private void OnDeformedModelChange()
        {
            Rebuild();
        }

        public Dictionary<string, meshST> GetMeshes()
        {
            return meshes;
        }

        /// <summary>
        /// 设置弯曲线条段
        /// </summary>
        public void SetCurve(CubicBezierCurve curve, bool update = true)
        {
            if (curve == null)
            {
                Debug.LogError("SetCurve error, curve is null");
                return;
            }
            if (this.curve != null) this.curve.changed.RemoveListener(OnDeformedModelChange);
            this.curve = curve;
            this.curve.changed.AddListener(OnDeformedModelChange);
            if (update) Rebuild();
        }

        /// <summary>
        /// 设置网格旋转
        /// </summary>
        public void SetSourceRotation(Quaternion rotation, bool update = true)
        {
            sourceRotation = rotation;
            if (update) Rebuild();
        }

        public void SetParentRotation(Quaternion rotation, bool update = true)
        {
            parentRotation = rotation;
            if (update) Rebuild();
        }

        /// <summary>
        /// 设置移动
        /// </summary>
        public void SetTranslation(Vector3 translation, bool update = true)
        {
            sourceTranslation = translation;
            if (update) Rebuild();
        }

        /// <summary>
        /// 设置开始处缩放
        /// </summary>
        public void SetStartScale(float scale, bool update = true)
        {
            startScale = scale;
            if (update) Rebuild();
        }

        /// <summary>
        /// 设置结尾处缩放
        /// </summary>
        public void SetEndScale(float scale, bool update = true)
        {
            endScale = scale;
            if (update) Rebuild();
        }

        /// <summary>
        /// 设置开始处的翻转度
        /// </summary>
        public void SetStartRoll(float roll, bool update = true)
        {
            startRoll = roll;
            if (update) Rebuild();
        }

        /// <summary>
        /// 设置结尾处的翻转度
        /// </summary>
        public void SetEndRoll(float roll, bool update = true)
        {
            endRoll = roll;
            if (update) Rebuild();
        }

        /// <summary>
        /// 设置曲线起始处的偏移量
        /// </summary>
        public void SetStartOffset(float offset, bool update = true)
        {
            startOffset = Mathf.Clamp01(offset);
            if (update) Rebuild();
        }

        /// <summary>
        /// 设置曲线结尾处的偏移量
        /// </summary>
        public void SetEndOffset(float offset, bool update = true)
        {
            endOffset = Mathf.Clamp01(offset);
            if (update) Rebuild();
        }

        /// <summary>
        /// 重构模型
        /// </summary>
        public void Rebuild()
        {
            if (curve == null)
            {
                Debug.LogError("Rebuild error, curve is null", gameObject);
                return;
            }

            if (meshes.Count == 0)
            {
                Debug.LogError("Rebuild error, meshes.Count is zero", gameObject);
                return;
            }

            startOffset = Mathf.Clamp01(startOffset);
            endOffset = Mathf.Clamp01(endOffset);

            if (startOffset >= endOffset)
            {
                Debug.LogErrorFormat(gameObject, "Rebuild error, startOffset({0}) is more than endOffset({1})", startOffset, endOffset);
                return;
            }

            // 变形每一个网格
            foreach (var pair in meshes)
            {
                string key = pair.Key;
                meshST st = pair.Value;
                Mesh baseMesh = st.baseMesh;
                Mesh resultMesh = st.newMesh;
                resultMesh.Clear();

                // 找到网格模型在x轴上的最大宽度
                float minX = float.MaxValue;
                float maxX = float.MinValue;
                foreach (var vert in baseMesh.vertices)
                {
                    Vector3 p = vert;
                    if (sourceRotation != Quaternion.identity)
                    {
                        p = sourceRotation * p;
                    }

                    if (sourceTranslation != Vector3.zero)
                    {
                        p += sourceTranslation;
                    }
                    maxX = Mathf.Max(maxX, p.x);
                    minX = Mathf.Min(minX, p.x);
                }
                float length = Mathf.Abs(maxX - minX);

                // 记录变形后的顶点位置和法线
                int vertCount = baseMesh.vertices.Length;
                List<Vector3> deformedVerts = new List<Vector3>(vertCount);
                List<Vector3> deformedNormals = new List<Vector3>(vertCount);

                // 计算每一个顶点在曲线上的对应位置
                for (int i = 0; i < vertCount; i++)
                {
                    Vector3 p = baseMesh.vertices[i];
                    Vector3 n = baseMesh.normals[i];

                    // apply rotation and translation
                    if (sourceRotation != Quaternion.identity)
                    {
                        p = sourceRotation * p;
                        n = sourceRotation * n;
                    }
                    if (sourceTranslation != Vector3.zero)
                    {
                        p += sourceTranslation;
                    }

                    // 计算顶点x-轴比例
                    float distanceRate = Mathf.Abs(p.x - minX) / length;

                    // 得到投射曲线点的位置和切线
                    float dis = ((endOffset - startOffset) * distanceRate + startOffset) * curve.GetLength();
                    Vector3 curvePoint = curve.GetLocationAtDistance(dis);
                    Vector3 curveTangent = curve.GetTangentAtDistance(dis);
                    Quaternion q = CubicBezierCurve.GetRotationFromTangent(curveTangent) * Quaternion.Euler(0, -90, 0); // *Quaternion.Inverse(parentRotation);

                    // apply scale
                    float scaleAtDistance = startScale + (endScale - startScale) * distanceRate;
                    p *= scaleAtDistance;

                    // apply roll
                    float rollAtDistance = startRoll + (endRoll - startRoll) * distanceRate;
                    p = Quaternion.AngleAxis(rollAtDistance, Vector3.right) * p;
                    n = Quaternion.AngleAxis(rollAtDistance, Vector3.right) * n;

                    // reset x 
                    p = new Vector3(0, p.y, p.z);

                    p = q * p + curvePoint;
                    n = q * n;
                    if (parentRotation != Quaternion.identity)
                    {
                        p = parentRotation * p;
                        n = parentRotation * n;
                    }
                    deformedVerts.Add(p);
                    deformedNormals.Add(n);

                    // deformedVerts.Add(q * p + curvePoint);
                    // deformedNormals.Add(q * n);
                }

                resultMesh.vertices = deformedVerts.ToArray();
                resultMesh.normals = deformedNormals.ToArray();
                resultMesh.uv = baseMesh.uv;
                resultMesh.triangles = baseMesh.triangles;

                if (st.isCollider) st.go.GetComponent<MeshCollider>().sharedMesh = resultMesh;
                else st.go.GetComponent<MeshFilter>().sharedMesh = resultMesh;
            }
        }
    }
}