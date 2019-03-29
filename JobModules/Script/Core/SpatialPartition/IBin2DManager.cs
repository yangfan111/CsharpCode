using System;
using System.Collections.Generic;
using Core.EntityComponent;
using UnityEngine;

namespace Core.SpatialPartition
{
    [Serializable]
    public class Bin2dState
    {
        public readonly int Type;
        public readonly Bin2D<IGameEntity> Bin2D;
        public readonly int VisibleRadius;
        public readonly Func<Vector3, bool> Bin2DFilter;

        public Bin2dState(int type, Bin2D<IGameEntity> bin2D, int visibleRadius, Func<Vector3, bool> bin2DFilter = null)
        {
            Type = type;
            Bin2D = bin2D;
            VisibleRadius = visibleRadius;
            Bin2DFilter = bin2DFilter;
        }

        public bool Filter(Vector3 position)
        {
            if (Bin2DFilter == null) return true;
            return Bin2DFilter(position);
        }

        public void Retrieve(Vector3 position, Rect rect, Action<IGameEntity> doInsert)
        {
            Bin2D.Retrieve(rect, doInsert);
        }
    }

    public interface IBin2DManager:IDisposable
    {
        Bin2dState AddBin2D(int type, Bin2D<IGameEntity> bin2D, int visibleRadius,
            Func<Vector3, bool> bin2DFilter = null);

        Bin2dState GetBin2D(int type);
        ICollection<Bin2dState> GetBin2Ds();
    }
    [Serializable]
    public class Bin2DManager : IBin2DManager
    {
        public Dictionary<int, Bin2dState> Bin2DStates = new Dictionary<int, Bin2dState>();

        public Bin2dState AddBin2D(int type, Bin2D<IGameEntity> bin2D, int visibleRadius,
            Func<Vector3, bool> bin2DFilter = null)
        {
            var r = new Bin2dState(type, bin2D, visibleRadius, bin2DFilter);
            Bin2DStates[type] = r;
            return r;
        }

        public Bin2dState GetBin2D(int type)
        {
            return Bin2DStates[type];
        }

        public ICollection<Bin2dState> GetBin2Ds()
        {
            return Bin2DStates.Values;
        }

        public void Dispose()
        {
            foreach (var bin2DState in Bin2DStates.Values)
            {
                bin2DState.Bin2D.Dispose();
            }
        }
    }
}