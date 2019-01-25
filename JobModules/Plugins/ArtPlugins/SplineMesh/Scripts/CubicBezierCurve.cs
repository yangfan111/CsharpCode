using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SplineTools
{
    public class CubicBezierCurve
    {
        private class CurveSample
        {
            public Vector3 location;
            public Vector3 tangent;
            public float distance;
        }

        private SplineNode node1;
        private SplineNode node2;

        private float length;

        public float step = 0.01f;

        private List<CurveSample> samples = new List<CurveSample>();

        public UnityEvent changed = new UnityEvent();

        public CurveType curveType
        {
            get
            {
                return node1.curveType;
            }
        }

        public CubicBezierCurve(SplineNode node1, SplineNode node2, CurveType curveType = CurveType.Spline)
        {
            this.node1 = node1;
            this.node2 = node2;

            this.node1.changed.AddListener(OnNodeChanged);
            this.node2.changed.AddListener(OnNodeChanged);
            ComputeSamplePoints();
        }

        /// <summary>
        /// 更改曲线段起始节点
        /// </summary>
        public void ConnectStart(SplineNode node1)
        {
            if (node1 == null)
            {
                Debug.LogError("CubeBezierCurve.ConnectStart error, node1 is null");
                return;
            }

            this.node1.changed.RemoveListener(OnNodeChanged);
            this.node1 = node1;
            this.node1.changed.AddListener(OnNodeChanged);
            ComputeSamplePoints();
        }

        /// <summary>
        /// 更改曲线段终端节点
        /// </summary>
        public void ConnectEnd(SplineNode node2)
        {
            if (node2 == null)
            {
                Debug.LogError("CubeBezierCurve.ConnectEnd error, node2 is null");
                return;
            }

            this.node2.changed.RemoveListener(OnNodeChanged);
            this.node2 = node2;
            this.node2.changed.AddListener(OnNodeChanged);
            ComputeSamplePoints();
        }

        public void SetCurveType(CurveType tp)
        {
            node1.SetCurveType(tp);
        }

        public void SetCurveStep(float step)
        {
            if (step != this.step)
            {
                this.step = step;
                ComputeSamplePoints();
            }
        }

        /// <summary>
        /// 取得指定t对应的曲线点位置
        /// </summary>
        /// <param name="t">[0,1]</param>
        public Vector3 GetLocation(float t)
        {
            if (t < 0f || t > 1f)
            {
                throw new ArgumentOutOfRangeException("t", "CubeBezierCurve.GetLocation(t) error, t is out of range, t:" + t.ToString());
            }

            Vector3 p = Vector3.zero;
            Vector3 p0 = node1.GetPosition();
            Vector3 p3 = node2.GetPosition();

            if (curveType == CurveType.Spline)                          // spline 
            {
                float omt = 1f - t;
                float omt2 = omt * omt;
                float t2 = t * t;

                Vector3 p1 = node1.GetNextDirection();
                Vector3 p2 = node2.GetPreDirection();
                p = p0 * (omt2 * omt) + p1 * (3f * omt2 * t) + p2 * (3f * omt * t2) + p3 * (t2 * t);
            }
            else                                                        // line
            {
                p = p0 * (1 - t) + p3 * t;
            }
            return p;
        }

        /// <summary>
        /// 取得指定t对应的曲线点切线
        /// </summary>
        /// <param name="t">[0,1]</param>
        public Vector3 GetTangent(float t)
        {
            if (t < 0f || t > 1f)
            {
                throw new ArgumentOutOfRangeException("t", "CubeBezierCurve.GetTangent(t) error, t is out of range, t:" + t.ToString());
            }

            Vector3 tangent = Vector3.zero;
            Vector3 p0 = node1.GetPosition();
            Vector3 p3 = node2.GetPosition();

            if (curveType == CurveType.Spline)
            {
                float omt = 1f - t;
                float omt2 = omt * omt;
                float t2 = t * t;
                Vector3 p1 = node1.GetNextDirection();
                Vector3 p2 = node2.GetPreDirection();
                tangent = p0 * (-omt) + p1 * (3 * omt2 - 2 * omt) + p2 * (-3 * t2 + 2 * t) + p3 * t2;
            }
            else
            {
                tangent = p3 - p0;
            }

            return tangent.normalized;
        }

        public float GetLength()
        {
            return length;
        }

        public void OnNodeChanged()
        {
            ComputeSamplePoints();
        }

        public void Rebuild()
        {
            ComputeSamplePoints();
        }

        private void ComputeSamplePoints()
        {
            samples.Clear();
            length = 0;

            if (curveType == CurveType.Spline)                          // spline
            {
                Vector3 prePoint = GetLocation(0);
                for (float t = 0f; t < 1f; t += step)
                {
                    CurveSample sample = new CurveSample();
                    sample.location = GetLocation(t);
                    sample.tangent = GetTangent(t);
                    length += Vector3.Distance(prePoint, sample.location);
                    sample.distance = length;
                    samples.Add(sample);
                    prePoint = sample.location;
                }
                {
                    CurveSample sample = new CurveSample();
                    sample.location = GetLocation(1);
                    sample.tangent = GetTangent(1);
                    length += Vector3.Distance(prePoint, sample.location);
                    sample.distance = length;
                    samples.Add(sample);
                }
            }
            else                                                        // line
            {
                Vector3 p0 = node1.GetPosition();
                Vector3 p3 = node2.GetPosition();
                length = Vector3.Distance(p0, p3);
            }

            if (changed != null) changed.Invoke();
        }

        private CurveSample GetCurvePointAtDistance(float d)
        {
            if (d < 0 || d > length)
            {
                throw new ArgumentOutOfRangeException("d", string.Format("CubeBezierCurve.GetCurvePointAtDistance error, d is out of range, d:{0}, length:{1}", d, length));
            }

            CurveSample sample = new CurveSample();
            if (curveType == CurveType.Spline)                      // spline
            {
                CurveSample previous = samples[0];
                CurveSample next = null;
                foreach (CurveSample cp in samples)
                {
                    if (cp.distance >= d)
                    {
                        next = cp;
                        break;
                    }
                    previous = cp;
                }
                if (next == null)
                {
                    throw new Exception("Can't find curve samples");
                }

                float t = next == previous ? 0 : (d - previous.distance) / (next.distance - previous.distance);
                sample.distance = d;
                sample.location = Vector3.Lerp(previous.location, next.location, t);
                sample.tangent = Vector3.Lerp(previous.tangent, next.tangent, t).normalized;
            }
            else                                                    // line
            {
                Vector3 p0 = node1.GetPosition();
                Vector3 p3 = node2.GetPosition();
                sample.distance = d;
                sample.tangent = (p3 - p0).normalized;
                sample.location = p0 + d * (p3 - p0) / Vector3.Distance(p3, p0);
            }
            return sample;
        }

        public Vector3 GetLocationAtDistance(float d)
        {
            return GetCurvePointAtDistance(d).location;
        }

        public Vector3 GetTangentAtDistance(float d)
        {
            return GetCurvePointAtDistance(d).tangent;
        }

        public static Quaternion GetRotationFromTangent(Vector3 tangent)
        {
            if (tangent == Vector3.zero) return Quaternion.identity;
            return Quaternion.LookRotation(tangent, Vector3.up);
        }
    }
}