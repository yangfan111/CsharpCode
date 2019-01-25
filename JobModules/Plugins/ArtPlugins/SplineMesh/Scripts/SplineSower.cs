using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SplineTools
{
    /// <summary>
    /// 自定义样条曲线摆放模型，需要考虑两点
    /// 1. 模型的迭代
    /// 2. LOD功能的支持
    /// </summary>
    [ExecuteInEditMode]
    [SelectionBase]
    [RequireComponent(typeof(Spline))]
    public class SplineSower : MonoBehaviour
    {
        /// <summary>
        /// 带摆放的fbx模型
        /// </summary>
        public GameObject fbxModel;

        /// <summary>
        /// 摆放的最小间隔
        /// </summary>
        public float spacing = 1f;

        /// <summary>
        /// 间隔是否随机
        /// </summary>
        public bool randomSpacing = false;

        /// <summary>
        /// 摆放的间隔随机范围
        /// </summary>
        public float spacingRange = 0;

        /// <summary>
        /// 缩放的最小比例
        /// </summary>
        public float scale = 1f;

        /// <summary>
        /// 是否随机缩放
        /// </summary>
        public bool randomScale = false;

        /// <summary>
        /// 缩放的随机范围
        /// </summary>
        public float scaleRange = 0f;

        /// <summary>
        /// Y方向最小偏移量
        /// </summary>
        public float yOffset = 0f;

        /// <summary>
        /// Y方向是否随机偏移
        /// </summary>
        public bool randomYOffset = false;

        /// <summary>
        /// Y方向随机偏移范围
        /// </summary>
        public float yOffsetRange = 0f;

        /// <summary>
        /// Z方向最小偏移量
        /// </summary>
        public float zOffset = 0f;

        /// <summary>
        /// Z方向是否随机偏移
        /// </summary>
        public bool randomZOffset = false;

        /// <summary>
        /// Z方向随机偏移范围
        /// </summary>
        public float zOffsetRange = 0f;

        // /// <summary>
        // /// 旋转的最小角度
        // /// </summary>
        // public Vector3 rotation = new Vector3(-90f, 0f, 0f);

        /// <summary>
        /// 记录每一个曲线段的旋转量
        /// </summary>
        public List<Vector3> rotations = new List<Vector3>();

        // /// <summary>
        // /// 是否随机旋转
        // /// </summary>
        // public bool randomRotation = false;

        // /// <summary>
        // /// 随机旋转范围
        // /// </summary>
        // public Vector3 randomRotationRange;

        /// <summary>
        /// 随机种子
        /// </summary>
        // public int randomSeed = 0;

        /// <summary>
        /// 摆放的模型列表
        /// </summary>
        public List<GameObject> models = new List<GameObject>();

        /// <summary>
        /// 曲线路径
        /// </summary>
        public Spline spline;

        /// <summary>
        /// 是否将实例化后的模型子物体旋转取消
        /// </summary>
        public bool forceChildNoRotation = true;

        /// <summary>
        /// 模型开始摆放的偏移量
        /// </summary>
        public float startOffset = 0f;

        /// <summary>
        /// 是否更新摆放
        /// </summary>
        private bool needUpdate = false;

        public void Init()
        {
            spline = GetComponent<Spline>();

            if (spline != null)
            {
                spline.rebuildSplineEvent.AddListener(OnRebuildSplineEvent);
                spline.nodeCountChangeTypeEvent.AddListener(OnNodeCountChangeTypeEvent);
                spline.curveChanged.AddListener(OnCurveChanged);
            }
        }

        private void OnEnable()
        {
            Init();
        }

        private void Update()
        {
            if (!needUpdate) return;

            Sow();
            needUpdate = false;
        }

        private void OnDisable()
        {
            Recover();
        }

        public void Recover()
        {
            if (spline != null)
            {
                spline.rebuildSplineEvent.RemoveListener(OnRebuildSplineEvent);
                spline.nodeCountChangeTypeEvent.RemoveListener(OnNodeCountChangeTypeEvent);
                spline.curveChanged.RemoveListener(OnCurveChanged);
            }
        }

        public List<GameObject> GetModels()
        {
            return models;
        }

        private void OnNodeCountChangeTypeEvent(NodeCountChangeType tp, int index)
        {
            if (spline == null) return;

            // add rotation
            switch (tp)
            {
                case NodeCountChangeType.AddBack:
                    {
                        rotations.Add(new Vector3(-90f, 0f, 0f));
                    }
                    break;
                case NodeCountChangeType.AddFront:
                    {
                        rotations.Insert(0, new Vector3(-90f, 0f, 0f));
                    }
                    break;
                case NodeCountChangeType.Delete:
                    {
                        if (index == rotations.Count) --index;
                        rotations.RemoveAt(index);
                    }
                    break;
                case NodeCountChangeType.Insert:
                    {
                        rotations.Insert(index, new Vector3(-90f, 0f, 0));
                    }
                    break;
            }

            // force immediate update
            Sow();
        }

        /// <summary>
        /// 响应曲线重构
        /// </summary>
        private void OnRebuildSplineEvent()
        {
            if (spline == null) return;

            rotations.Clear();
            var curves = spline.GetCurves();
            for (int i = 0; i < curves.Count; i++)
            {
                rotations.Add(new Vector3(-90f, 0f, 0f));
            }

            // force immediate update 
            Sow();
        }

        /// <summary>
        /// 曲线段的改变回调
        /// </summary>
        private void OnCurveChanged()
        {
            needUpdate = true;
        }

        /// <summary>
        /// 模型摆放
        /// </summary>
        public void Sow()
        {
            if (spline == null) return;

            // 清除旧存数据
            foreach (var go in models)
            {
                if (go == null) continue;

                if (Application.isPlaying)
                    Destroy(go);
                else
                    DestroyImmediate(go);
            }
            models.Clear();

            if (fbxModel == null) return;

            // 实例化模型列表
            float distance = startOffset;
            int index = 0;
            while (distance <= spline.GetLength())
            {
                // 实例化模型
                string name = string.Format("{0} ({1})", fbxModel.name, index);
                GameObject go = Instantiate<GameObject>(fbxModel, Vector3.zero, Quaternion.identity, transform);
                go.name = name;
                if (forceChildNoRotation)
                {
                    foreach (Transform ch in go.transform) ch.localRotation = Quaternion.identity;
                }

                // set position
                Vector3 modelPos = spline.GetLocationAlongSplineAtDistance((distance));
                go.transform.localPosition = modelPos;

                // set scale
                float modelScale = scale + (randomScale ? Random.Range(0f, scaleRange) : 0f);
                go.transform.localScale = Vector3.one * modelScale;

                // set rotation
                Vector3 horTangent = spline.GetTangentAlongSplineAtDistance(distance);
                horTangent.y = 0;
                go.transform.rotation = Quaternion.LookRotation(horTangent) * Quaternion.LookRotation(Vector3.left, Vector3.up);

                // apply additional rotation
                int curveIndex = spline.GetCurveIndexAtDistance(distance);
                Vector3 modelRot = rotations[curveIndex];
                // Vector3 modelRot = rotation + (randomRotation ? new Vector3(
                //     Random.Range(Mathf.Min(0f, randomRotationRange.x), Mathf.Max(0f, randomRotationRange.x)),
                //     Random.Range(Mathf.Min(0f, randomRotationRange.y), Mathf.Max(0f, randomRotationRange.y)),
                //     Random.Range(Mathf.Min(0f, randomRotationRange.z), Mathf.Max(0f, randomRotationRange.z))) : Vector3.zero);
                go.transform.Rotate(modelRot);

                // apply yOffset
                Vector3 binormal = spline.GetTangentAlongSplineAtDistance(distance);
                binormal = Quaternion.LookRotation(Vector3.right, Vector3.up) * binormal;
                float random = yOffsetRange * Mathf.Sign(yOffset);
                random = Random.Range(Mathf.Min(0f, random), Mathf.Max(0f, random));
                binormal *= yOffset + (randomYOffset ? random : 0f);
                go.transform.position += binormal;

                // apply zOffset
                float modelZOffset = zOffset + (randomZOffset ? Random.Range(Mathf.Min(0f, zOffsetRange), Mathf.Max(0f, zOffsetRange)) : 0f);
                go.transform.localPosition += Vector3.up * modelZOffset;

                models.Add(go);

                distance += spacing + (randomSpacing ? Random.Range(0f, spacingRange) : 0f);
            }
        }
    }
}
