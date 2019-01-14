using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.SpatialPartition
{
    /// <summary>
    /// 将地图的y轴去除，从上(Unity的top视图)往下看：x作为width，z作为height。
    ///           ^ height(z)
    ///           |
    ///           |       
    ///           |
    ///           |
    ///           \------------------------------> width(x)
    ///           bottomLeft
    /// 
    /// width方向分为GridWidth个格子，height方向分为GridHeight个格子
    /// 每个格子的width为CellWidth，高度为CellHeight
    /// bottomLeft为地图的包围盒的左下方坐标（实际坐标，不是格子坐标）
    /// </summary>
    ///
    [Serializable]
    public class Bin2DConfig
    {
        public Bin2DConfig(float minX, float minY, float maxX, float maxY, int cellSize, int visibleRadius)
        {
            GridWidth = (int)((maxX - minX + cellSize) / cellSize);
            GridHeight = (int)((maxY - minY + cellSize) / cellSize);
            CellWidth = cellSize;
            CellHeight = cellSize;
            
            BottomLeft = new Vector2(minX, minY);
            VisibleRadius = visibleRadius;

        }

        public Bin2DConfig()
        {
            
        }
        public  Bin2DConfig Clone(Bin2DConfig t)
        {
            Bin2DConfig ret = new Bin2DConfig();
            ret.GridWidth = t.GridWidth;
            ret.GridHeight = t.GridHeight;
            ret.CellWidth = t.CellWidth;
            ret.CellHeight = t.CellHeight;

            ret.BottomLeft = t.BottomLeft;
            ret.VisibleRadius = t.VisibleRadius;
            return ret;
        }

        public int CellWidth;
        public int CellHeight;
        public int GridWidth;
        public int GridHeight;
        public Vector2 BottomLeft;
       
        public int VisibleRadius;
       
    }
    public static class Bin2DUtility
    {
        public static Vector2 To2D(this Vector3 v3)
        {
            return new Vector2(v3.x, v3.z);
        }
    }
}