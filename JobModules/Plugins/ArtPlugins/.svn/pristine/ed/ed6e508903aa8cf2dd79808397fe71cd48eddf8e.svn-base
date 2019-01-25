using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace SplineTools
{
    public enum CurveType
    {
        Spline,
        Line,
    }

    [Serializable]
    public class SplineNode
    {
        [SerializeField]
        private Vector3 position;

        [SerializeField]
        private Vector3 preDirection;

        [SerializeField]
        private Vector3 nextDirection;

        [HideInInspector]
        public UnityEvent changed = new UnityEvent();

        /// <summary>
        /// 记录节点作为起始点的曲线类型
        /// </summary>
        [HideInInspector]
        public CurveType curveType = CurveType.Spline;

        public SplineNode(Vector3 position, Vector3 preDirection, Vector3 nextDirection, CurveType curveType = CurveType.Spline)
        {
            this.position = position;
            this.preDirection = preDirection;
            this.nextDirection = nextDirection;
            this.curveType = curveType;
        }

        public Vector3 GetPosition()
        {
            return position;
        }

        public Vector3 GetPreDirection()
        {
            return preDirection;
        }

        public Vector3 GetNextDirection()
        {
            return nextDirection;
        }

        public void SetPosition(Vector3 position)
        {
            if (this.position != position)
            {
                this.position = position;

                if (changed != null) changed.Invoke();
            }
        }

        public void SetPreDirection(Vector3 preDirection)
        {
            if (this.preDirection != preDirection)
            {
                this.preDirection = preDirection;

                if (changed != null) changed.Invoke();
            }
        }

        public void SetNextDirection(Vector3 nextDirection)
        {
            if (this.nextDirection != nextDirection)
            {
                this.nextDirection = nextDirection;

                if (changed != null) changed.Invoke();
            }
        }

        public void SetCurveType(CurveType curveType)
        {
            if (this.curveType != curveType)
            {
                this.curveType = curveType;

                if (changed != null) changed.Invoke();
            }
        }
    }
}