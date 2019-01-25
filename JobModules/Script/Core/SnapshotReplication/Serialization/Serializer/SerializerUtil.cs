using System;
using System.Collections;
using System.IO;
using Core.Utils;
using UnityEngine;

namespace Core.SnapshotReplication.Serialization.Serializer
{
    public class SerializerUtil
    {
        public static BitArray ReadFieldsMask(BinaryReader reader)
        {
            byte bitCount = reader.ReadByte();
            int bytesCount = (int)System.Math.Ceiling(bitCount * 1.0 / 8);
            byte[] bytes = new byte[bytesCount];
            for (int i = 0; i < bytesCount; i++)
            {
                bytes[i] = reader.ReadByte();
            }
            BitArray mask = new BitArray(bytes);
            return mask;
        }

        public static void WriteFieldsMask(MyBinaryWriter writer, BitArray fieldsMask)
        {
            var count = fieldsMask.Count;
            if (count > Byte.MaxValue)
            {
                string info = string.Format("property count {0} exceed the byte max",count);
                throw new Exception(info);
            }
            writer.Write((byte)count);
            byte[] bytes = ToBytes(fieldsMask);

            for (int i = 0; i < bytes.Length; i++)
            {
                writer.Write(bytes[i]);
            }

        }

        public static byte[] ToBytes(BitArray bits)
        {
            int bytesCount = (int)System.Math.Ceiling(bits.Count * 1.0 / 8);
            byte[] bytes = new byte[bytesCount];
            bits.CopyTo(bytes, 0);
            return bytes;
        }
    }
}
