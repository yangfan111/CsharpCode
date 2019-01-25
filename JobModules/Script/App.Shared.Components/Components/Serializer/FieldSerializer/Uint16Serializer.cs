using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace App.Shared.Components.Serializer.FieldSerializer
{
    class Uint16Serializer : IFieldSerializer<UInt16>
    {
        public void Write(UInt16 data, Core.Utils.MyBinaryWriter writer)
        {
            writer.Write(data);
        }

        public UInt16 Read(BinaryReader reader)
        {
            var data = reader.ReadUInt16();
            return data;
        }
    }
}
