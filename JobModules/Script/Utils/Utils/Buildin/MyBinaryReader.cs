using System.Collections;
using System.IO;
using Core.SnapshotReplication.Serialization.Serializer;

namespace Utils.Utils.Buildin
{
    public static class MyBinaryReader
    {
        public static BitArrayWrapper ReadBitArray(this BinaryReader binaryReader)
        {
            int count = binaryReader.ReadByte();
            BitArrayWrapper bitArray = BitArrayWrapper.Allocate(count);
            if (count != 0)
            {
                byte b = 0;
                for (int i = 0; i < count; i++)
                {
                    if (i % 8 == 0)
                        b = binaryReader.ReadByte();
                    bitArray[i] = (b & (1<<(i % 8))) != 0;
                }
            }

            return bitArray;
        }
    }
}