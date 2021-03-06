﻿using Core.Compare;
using Core.Compensation;
using Core.EntityComponent;
using Core.Interpolate;
using Core.Playback;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.Utils;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using System;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable PossibleNullReferenceException

namespace Core.Components
{
    public delegate void PositionChangedDelgate(IGameEntity owner, Vector3 oldPos, Vector3 newPos);

    public enum PositionInterpolateMode
    {
        Linear = 0,
        Discrete = 1,
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class PositionComponent : IPlaybackComponent, ICompensationComponent, IResetableComponent
    {
        private FixedVector3 _fixedValue;
        [DontInitilize] [NetworkProperty] public bool AlwaysEqual;


        [DontInitilize] [NetworkProperty] public byte InterpolateType;
        [DontInitilize] [NetworkProperty] public int ServerTime;

        [DontInitilize]
        [NetworkProperty]
        public FixedVector3 FixedVector3
        {
            get { return _fixedValue; }
            set
            {
                var old = _fixedValue;
                _fixedValue = value;
                for (int i = 0; i < _positionListener.Count; i++)
                {
                    if (_positionListener[i] != null)
                        _positionListener[i](_owner, old.WorldVector3(), value.WorldVector3());
                }
            }
        }

        [DontInitilize]
        public Vector3 Value
        {
            get { return FixedVector3.ShiftedVector3(); }
            set
            {
                FixedVector3 = value.ShiftedToFixedVector3();
            }
        }

        [DontInitilize] public Bounds Bounds { get; set; }
        [DontInitilize] public bool ModelRotate { get; set; }

        private List<PositionChangedDelgate> _positionListener = new List<PositionChangedDelgate>();

        public void AddPositionListener(PositionChangedDelgate func)
        {
            _positionListener.Add(func);
        }

        public void RemovePositionListener(PositionChangedDelgate func)
        {
            _positionListener.Remove(func);
        }

        private IGameEntity _owner;

        public void SetOwner(IGameEntity owner)
        {
            _owner = owner;
            owner.AcquireReference();
        }

        public void CleanOwner()
        {
            if (_owner != null)
                _owner.ReleaseReference();
        }

        public override string ToString()
        {
            return string.Format("AlwaysEqual: {0}, Value: {1}", AlwaysEqual, Value.ToStringExt());
        }

        public bool IsInterpolateEveryFrame()
        {
            return true;
        }

        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            PositionComponent l = left as PositionComponent;
            PositionComponent r = right as PositionComponent;
            if (r.InterpolateType == 0)
            {
                Value = InterpolateUtility.Interpolate(l.Value, r.Value, interpolationInfo.RatioWithOutClamp);
            }
            else
            {
                InterpolateType = r.InterpolateType;
                ServerTime = r.ServerTime;

                if (interpolationInfo.LeftServerTime <= r.ServerTime &&
                    interpolationInfo.RightServerTime >= r.ServerTime)
                {
                    if (interpolationInfo.CurrentRenderTime > r.ServerTime)
                    {
                        Value = r.Value;
                    }
                    else
                    {
                        Value = l.Value;
                    }
                }
                else
                {
                    Value = InterpolateUtility.Interpolate(l.Value, r.Value, interpolationInfo.RatioWithOutClamp);
                }
            }
        }

        public void CopyFrom(object rightComponent)
        {
            var r = (PositionComponent) rightComponent;
            FixedVector3 = r.FixedVector3;
            AlwaysEqual = r.AlwaysEqual;
            ServerTime = r.ServerTime;
            InterpolateType = r.InterpolateType;
        }

        public bool IsApproximatelyEqual(object right)
        {
            var r = (right as PositionComponent);
            if (r.AlwaysEqual || AlwaysEqual)
                return true;
            return CompareUtility.IsApproximatelyEqual(FixedVector3.WorldVector3(), r.FixedVector3.WorldVector3());
        }

        public int GetComponentId()
        {
            {
                return (int) ECoreComponentIds.Position;
            }
        }

        protected bool Equals(PositionComponent other)
        {
            return FixedVector3.Equals(other.FixedVector3);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((PositionComponent) obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }

        public void Reset()
        {
            _positionListener.Clear();
            InterpolateType = 0;
            ServerTime = 0;
        }
    }
}