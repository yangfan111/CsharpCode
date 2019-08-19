using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace UserInputManager.Lib
{

    public class PointerDataComparer : IEqualityComparer<PointerData>
    {
        public bool Equals(PointerData x, PointerData y)
        {
            if(null == x && null == y )
            {
                return true;
            }
            if(null == x)
            {
                return false;
            }
            if(null == y)
            {
                return false;
            }
            return x.Key == y.Key;
        }

        public int GetHashCode(PointerData obj)
        {
            return (int)obj.Key;
        }

        public static readonly PointerDataComparer Instance = new PointerDataComparer();
    }


    /// <summary>
    /// 鼠标信息，没有重新实现Hashcode，因为position不做标记使用
    /// 循环使用，每个Update起始，信息会被重置
    /// </summary>
    public class PointerData : KeyData 
    {
        public PointerData()
        {
            Position = Vector3.zero;
            MouseX = 0;
            MouseY = 0;
            Data = null;
            IdList = null;
        }

        public override string ToString()
        {
            return string.Format("Pos:{0} MouseX:{1} MouseY:{2} ",Position,MouseX,MouseY);
        }

        public PointerData(UserInputKey key):base(key)
        {
            Position = Vector3.zero;
        }

        public PointerData(UserInputKey key, Vector3 position) : base(key)
        {
            Position = position;
        }

        public Vector3 Position { get; set; }
        public float MouseX { get; set; }
        public float MouseY { get; set; }
        /// <summary>
        /// 用于通用信息存储，引用类型
        /// </summary>
        public object Data { get; set; }
        /// <summary>
        /// 用于通用信息存储，值类型
        /// </summary>
        public List<int> IdList { get; set; }

        public override void Reset()
        {
            base.Reset();
            Position = new Vector3();
            MouseX = 0;
            MouseY = 0;
            Data = null;
            IdList = null;
        }
    }
}
