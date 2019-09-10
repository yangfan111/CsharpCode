using System.Collections.Generic;

namespace UserInputManager.Lib
{
    public class KeyDataComparer : IEqualityComparer<KeyData>
    {
        public bool Equals(KeyData x, KeyData y)
        {
            if(null == x && null == y)
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

        public int GetHashCode(KeyData obj)
        {
            return (int)obj.Key;
        }

        public static readonly KeyDataComparer Instance = new KeyDataComparer();
    }
    /// <summary>
    /// 按键输入信息（鼠标也算）
    /// 循环使用，每个Update起始，信息会被重置
    /// </summary>
    public class KeyData : IReusableData
    {
        public UserInputKey Key { get; set; }
        public float Axis { get; set; }

        public KeyData()
        {
            Key = UserInputKey.None;
            Axis = 0;
        }

        public KeyData(UserInputKey key)
        {
            Key = key;
        }

        public KeyData(UserInputKey key, float axis)
        {
            Key = key;
            Axis = axis;
        }

        public virtual void Reset()
        {
            Key = UserInputKey.None;
            Axis = 0;
        }
    }
}
