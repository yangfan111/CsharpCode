using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SplineTools
{
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public class Spline : MonoBehaviour
    {
        /// <summary>
        /// 记录控制点列表
        /// </summary>
        [SerializeField]
        private List<SplineNode> nodes = new List<SplineNode>()
        {
            new SplineNode(new Vector3(-2f,0f,0f),new Vector3(-1f,0f,-1f),new Vector3(-1f,0f,1f)),
            new SplineNode(new Vector3(2f,0f,0f),new Vector3(1f,0f,1f),new Vector3(1f,0f,-1f))
        };

        /// <summary>
        /// 记录曲线段列表
        /// </summary>
        private List<CubicBezierCurve> curves = new List<CubicBezierCurve>();

        public UnityEvent nodeCountChanged = new UnityEvent();

        public UnityEvent curveChanged = new UnityEvent();

        public UnityEvent rebuildSplineEvent = new UnityEvent();

        public NodeCountChangeEvent nodeCountChangeTypeEvent = new NodeCountChangeEvent();

        /// <summary>
        /// 记录曲线长度
        /// </summary>
        private float length = 0f;

        /// <summary>
        /// 场景视图中是否一直显示曲线段
        /// </summary>
        public bool alwaysShow = true;

        public bool altKey = true;
        public bool distance = true;
        public bool direction = false;

        public void RebuildCurves()
        {
            curves.Clear();
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                CubicBezierCurve curve = new CubicBezierCurve(nodes[i], nodes[i + 1], nodes[i].curveType);
                curve.changed.AddListener(UpdateAfterCurveChanged);
                curves.Add(curve);
            }

            RaiseNodeCountChanged();
            UpdateAfterCurveChanged();

            if (rebuildSplineEvent != null) rebuildSplineEvent.Invoke();
        }

        private void RaiseNodeCountChanged()
        {
            if (nodeCountChanged != null) nodeCountChanged.Invoke();
        }

        private void UpdateAfterCurveChanged()
        {
            length = 0;
            foreach (var curve in curves)
            {
                length += curve.GetLength();
            }
            if (curveChanged != null) curveChanged.Invoke();
        }

        public Vector3 GetLocationAlongSpline(float t)
        {
            int index = GetCurveIndexForTime(t);
            return curves[index].GetLocation(t - index);
        }

        public Vector3 GetTangentAlongSpline(float t)
        {
            int index = GetCurveIndexForTime(t);
            return curves[index].GetTangent(t - index);
        }

        public Vector3 GetLocationAlongSplineAtDistance(float d)
        {
            if (d < 0 || d > length)
            {
                throw new ArgumentOutOfRangeException("d", "Spline.GetLocationAlongSplineAtDistance error, d:" + d);
            }

            float dis = d;
            foreach (var curve in curves)
            {
                if (dis > curve.GetLength())
                {
                    dis -= curve.GetLength();
                }
                else
                {
                    return curve.GetLocationAtDistance(dis);
                }
            }

            throw new Exception(string.Format("Spline.GetLocationAlongSplineAtDistance something wrong, d:{0} length:{1}", d, length));
        }

        public Vector3 GetTangentAlongSplineAtDistance(float d)
        {
            if (d < 0f || d > length)
            {
                throw new ArgumentOutOfRangeException("d", "Spline.GetTangentAlongSplineAtDistance error, d:" + d);
            }

            foreach (var curve in curves)
            {
                if (d > curve.GetLength())
                {
                    d -= curve.GetLength();
                }
                else
                {
                    return curve.GetTangentAtDistance(d);
                }
            }

            throw new Exception(string.Format("Spline.GetTangentAlongSplineAtDistance something wrong, d:{0} length:{1}", d, length));
        }

        public int GetCurveIndexAtDistance(float d)
        {
            if (d < 0f || d > length)
            {
                throw new ArgumentOutOfRangeException("d", "Spline.GetCurveIndexAtDistance error, d:" + d);
            }

            int index = 0;
            foreach (var curve in curves)
            {
                float len = curve.GetLength();
                if (d < len) return index;
                ++index;
                d -= len;
            }

            throw new Exception(string.Format("Spline.GetCurveIndexAtDistance something wrong, d:{0} length:{1}", d, length));
        }

        public void AddNode(SplineNode node, bool back = true)
        {
            if (node == null)
            {
                Debug.LogError("Spline.AddNode error, node is null");
                return;
            }

            if (back)                       // 结尾添加
            {
                nodes.Add(node);
                if (nodes.Count != 1)
                {
                    SplineNode previousNode = nodes[nodes.IndexOf(node) - 1];
                    var curve = new CubicBezierCurve(previousNode, node);
                    curve.changed.AddListener(UpdateAfterCurveChanged);
                    curves.Add(curve);
                }

                // event call
                if (nodeCountChangeTypeEvent != null)
                    nodeCountChangeTypeEvent.Invoke(NodeCountChangeType.AddBack, nodes.Count - 1);
            }
            else                            // 起始添加
            {
                nodes.Insert(0, node);
                if (nodes.Count != 1)
                {
                    var curve = new CubicBezierCurve(node, nodes[1]);
                    curve.changed.AddListener(UpdateAfterCurveChanged);
                    curves.Insert(0, curve);
                }

                // event call
                if (nodeCountChangeTypeEvent != null)
                    nodeCountChangeTypeEvent.Invoke(NodeCountChangeType.AddFront, 0);
            }

            // change callback
            UpdateAfterCurveChanged();
            RaiseNodeCountChanged();
        }

        public void InsertNode(int index, SplineNode node)
        {
            if (index <= 0 || index >= nodes.Count)
            {
                Debug.LogError("Spline.InsertNode error, index:" + index);
                return;
            }

            if (node == null)
            {
                Debug.LogError("Spline.InsertNode error, node is null");
                return;
            }

            SplineNode previousNode = nodes[index - 1];
            SplineNode nextNode = nodes[index];
            nodes.Insert(index, node);

            curves[index - 1].ConnectEnd(node);

            CubicBezierCurve curve = new CubicBezierCurve(node, nextNode);
            curve.changed.AddListener(UpdateAfterCurveChanged);
            curves.Insert(index, curve);

            // change callback
            UpdateAfterCurveChanged();
            RaiseNodeCountChanged();

            if (nodeCountChangeTypeEvent != null)
                nodeCountChangeTypeEvent.Invoke(NodeCountChangeType.Insert, index);
        }

        public void RemoveNode(SplineNode node)
        {
            if (nodes.Count <= 2)
            {
                Debug.LogError("Spline.RemoveNode error, nodes's count can't less than two");
                return;
            }

            int index = nodes.IndexOf(node);
            if (index == -1)
            {
                Debug.LogError("Spline.RemoveNode error, node is null");
                return;
            }

            if (index != 0 && index != nodes.Count - 1)
            {
                SplineNode nextNode = nodes[index + 1];
                curves[index - 1].ConnectEnd(nextNode);
            }

            nodes.RemoveAt(index);
            var toRemove = index == nodes.Count ? curves[index - 1] : curves[index];
            toRemove.changed.RemoveListener(UpdateAfterCurveChanged);
            curves.Remove(toRemove);

            UpdateAfterCurveChanged();
            RaiseNodeCountChanged();

            if (nodeCountChangeTypeEvent != null)
                nodeCountChangeTypeEvent.Invoke(NodeCountChangeType.Delete, index);
        }

        private int GetCurveIndexForTime(float t)
        {
            if (t < 0 || t > nodes.Count - 1)
            {
                throw new ArgumentOutOfRangeException("t", "Spline.GetCurveIndexForTime error, t:" + t);
            }

            int index = Mathf.FloorToInt(t);
            if (index == nodes.Count - 1) index--;

            return index;
        }

        public List<SplineNode> GetNodes()
        {
            return nodes;
        }

        public List<CubicBezierCurve> GetCurves()
        {
            return curves;
        }

        public float GetLength()
        {
            return length;
        }

        public void SetCurvesType(CurveType tp)
        {
            for (int i = 0; i < curves.Count; i++)
            {
                curves[i].SetCurveType(tp);
            }
        }

        #region Gizmo
        [SerializeField]
        private float gizmoRadius = 0.05f;
        private int selectCurveIndex = -1;

        private void OnDrawGizmosSelected()
        {
            DrawSplineGizmo(Color.yellow, true);
            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];

                Vector3 pos = transform.TransformPoint(node.GetPosition());
                Vector3 pre = transform.TransformPoint(node.GetPreDirection());
                Vector3 next = transform.TransformPoint(node.GetNextDirection());

                // lines
                Gizmos.color = Color.green;
                if (i != 0 && i <= curves.Count && curves[i - 1] != null && curves[i - 1].curveType == CurveType.Spline)
                {
                    Gizmos.DrawLine(pos, pre);
                }
                if (i != nodes.Count - 1 && i <= curves.Count - 1 && curves[i] != null && curves[i].curveType == CurveType.Spline)
                {
                    Gizmos.DrawLine(pos, next);
                }

                // spheres
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(pos, gizmoRadius);
                Gizmos.color = Color.cyan;
                if (i != 0 && i <= curves.Count && curves[i - 1] != null && curves[i - 1].curveType == CurveType.Spline)
                {
                    Gizmos.DrawSphere(pre, gizmoRadius);
                }
                if (i != nodes.Count - 1 && i <= curves.Count - 1 && curves[i] != null && curves[i].curveType == CurveType.Spline)
                {
                    Gizmos.DrawSphere(next, gizmoRadius);
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (!alwaysShow) return;

            DrawSplineGizmo(Color.gray);
            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];

                Vector3 pos = transform.TransformPoint(node.GetPosition());
                Vector3 pre = transform.TransformPoint(node.GetPreDirection());
                Vector3 next = transform.TransformPoint(node.GetNextDirection());

                Gizmos.color = Color.gray;
                if (i != 0 && i <= curves.Count && curves[i - 1] != null && curves[i - 1].curveType == CurveType.Spline)
                {
                    Gizmos.DrawLine(pos, pre);
                }
                if (i != nodes.Count - 1 && i <= curves.Count - 1 && curves[i] != null && curves[i].curveType == CurveType.Spline)
                {
                    Gizmos.DrawLine(pos, next);
                }

                Gizmos.color = Color.white;
                Gizmos.DrawSphere(pos, gizmoRadius);
                if (i != 0 && i <= curves.Count && curves[i - 1] != null && curves[i - 1].curveType == CurveType.Spline)
                {
                    Gizmos.DrawSphere(pre, gizmoRadius);
                }
                if (i != nodes.Count - 1 && i <= curves.Count - 1 && curves[i] != null && curves[i].curveType == CurveType.Spline)
                {
                    Gizmos.DrawSphere(next, gizmoRadius);
                }
            }
        }

        private void DrawSplineGizmo(Color color, bool selfSelect = false)
        {
            Gizmos.color = color;

            for (int i = 0; i < curves.Count; i++)
            {
                Gizmos.color = (selfSelect && i == selectCurveIndex) ? Color.cyan : color;

                var curve = curves[i];
                float t = curve.step;
                for (; t <= 1f; t += curve.step)
                {
                    Vector3 from = transform.TransformPoint(curve.GetLocation(t - curve.step));
                    Vector3 to = transform.TransformPoint(curve.GetLocation(t));
                    Gizmos.DrawLine(from, to);
                }
                if (t > 1f)
                {
                    t -= curve.step;
                    Vector3 from = transform.TransformPoint(curve.GetLocation(t));
                    Vector3 to = transform.TransformPoint(curve.GetLocation(1f));
                    Gizmos.DrawLine(from, to);
                }
            }
        }

        public void HighlightCurve(int index)
        {
            selectCurveIndex = index;
        }
        #endregion
    }

    public enum NodeCountChangeType
    {
        AddFront,
        AddBack,
        Insert,
        Delete,
    }

    public class NodeCountChangeEvent : UnityEvent<NodeCountChangeType, int> { }
}