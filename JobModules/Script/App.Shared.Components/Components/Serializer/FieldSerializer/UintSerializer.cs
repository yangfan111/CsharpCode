using System;
using System.IO;
using Core.Utils;

namespace App.Shared.Components.Serializer.FieldSerializer
{
    class UintSerializer :IFieldSerializer<uint>
    {
        public void Write(uint data, Core.Utils.MyBinaryWriter writer)
        {
            writer.Write(data);
        }

        public uint Read( BinaryReader reader)
        {
            var data = reader.ReadUInt32();
            return data;
        }
    }
}
