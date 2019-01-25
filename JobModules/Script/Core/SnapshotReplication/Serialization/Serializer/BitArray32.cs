using System;
using System.IO;
using Core.Utils;

namespace Core.SnapshotReplication.Serialization.Serializer
{
    public struct BitArray32
    {
        private ulong _value;
        private byte _bitNum;
        private static ulong[] _maskArray;
     

        static BitArray32()
        {
            _maskArray = new ulong[Capacity];

            for (int i = 0; i < 64; i++)
            {
                _maskArray[i] = 1ul << i;
            }
        }

        public const int Capacity = 64;
        

        public BitArray32(byte bitNum)
        {
            _value = 0;
            _bitNum = bitNum;
            
            if (bitNum > Capacity)
            {
                string info = string.Format("max supported bits num is {0} ",Capacity);
                throw new Exception(info);
            }
        }

        public int BitNum
        {
            get { return _bitNum; }
        }

        public void SetBit(int index,bool val = true)
        {
            CheckRange(index, _bitNum);
            if (val)
            {
                _value = (_value | _maskArray[index]);
            }
            else
            {
                ulong mask = ~_maskArray[index];
                _value = _value & mask;
            }
        }

        // ReSharper disable once UnusedParameter.Local
        private void CheckRange(int index, int bitNum)
        {
            if (index >= BitNum)
            {
                string info = string.Format("BitNum {0} Index {1} ", BitNum, index);
                throw new IndexOutOfRangeException(info);
            }
        }
        public void ClearBit(int index)
        {
            CheckRange(index, _bitNum);
            ulong mask = _maskArray[index];
            mask = ~mask;
            _value = _value & mask;
        }

        public bool GetBit(int index)
        {
            CheckRange(index, _bitNum);
            ulong mask = _maskArray[index];
            ulong res = mask & _value;
            if (res == 0)
            {
                return false;
            }
            return true;
        }

        public void Serialize(MyBinaryWriter writer)
        {
            writer.Write(_bitNum);
            ulong val = _value;
            ulong byteMask = 0x000000ff;
            int byteNum = (int)System.Math.Ceiling(_bitNum * 1.0 / 8);
            for (int i = 0; i < byteNum; i++)
            {
                byte b = (byte)(val & byteMask);
                val = val >> 8;
                writer.Write(b);
            }
        }

        public void Serialize(MyBinaryWriter writer, int sendLength)
        {
            _bitNum = (byte)sendLength;
            ulong val = _value;
            ulong byteMask = 0x000000ff;
            int byteNum = (int)System.Math.Ceiling(_bitNum * 1.0 / 8);
            for (int i = 0; i < byteNum; i++)
            {
                byte b = (byte)(val & byteMask);
                val = val >> 8;
                writer.Write(b);
            }
        }

        public void Deserialize(BinaryReader reader)
        {
            _bitNum = reader.ReadByte();
            int byteNum = (int)System.Math.Ceiling(_bitNum * 1.0 / 8);
            ulong val = 0;
            for (int i = 0; i < byteNum; i++)
            {
                byte v = reader.ReadByte();
                ulong tmp = v;
                tmp = tmp << i*8;
                val |= tmp;
            }
            _value = val;
        }

        public void Deserialize(BinaryReader reader, int sendLength)
        {
            _bitNum = (byte)sendLength;
            int byteNum = (int)System.Math.Ceiling(_bitNum * 1.0 / 8);
            ulong val = 0;
            for (int i = 0; i < byteNum; i++)
            {
                byte v = reader.ReadByte();
                ulong tmp = v;
                tmp = tmp << i * 8;
                val |= tmp;
            }
            _value = val;
        }

        public bool HasValue()
        {
            return _value != 0;
        }

        public bool this[int idIndex]
        {
            get {return  GetBit(idIndex); }
            set { SetBit(idIndex, value); }
        }
    }
}
