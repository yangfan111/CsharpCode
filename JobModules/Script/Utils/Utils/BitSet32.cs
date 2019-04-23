using System;

namespace Utils.Utils
{
    public interface IBitset<T>
    {
        void SetByMask(T mask);
        void ClearByMask(T mask);
        void SetByOffset(int offset);
        void ClearByOffset(int offset);
        bool HasMask(T mask);
        bool HasOffset(int offset);
    }

    public struct BitSet32 : IBitset<int>
    {
        private int _value;

        public void SetByMask(int mask)
        {
            _value |= mask;

        }

        public void ClearByMask(int mask)
        {
            _value &= ~mask;
        }

        public void SetByOffset(int offset)
        {
            SetByMask(1 << offset);
        }

        public void ClearByOffset(int offset)
        {
            ClearByMask(1 << offset);
        }

        public bool HasMask(int mask)
        {
            return (_value & mask) == mask;
        }

        public bool HasOffset(int offset)
        {
            return HasMask(1 << offset);
        }
    }

    public struct BitSet64 : IBitset<long>
    {
        private long _value;

        public void SetByMask(long mask)
        {
            _value |= mask;

        }

        public void ClearByMask(long mask)
        {
            _value &= ~mask;
        }

        public void SetByOffset(int offset)
        {
            SetByMask(1L << offset);
        }

        public void ClearByOffset(int offset)
        {
            ClearByMask(1L << offset);
        }

        public bool HasMask(long mask)
        {
            return (_value & mask) == mask;
        }

        public bool HasOffset(int offset)
        {
            return HasMask(1L << offset);
        }
    }
}