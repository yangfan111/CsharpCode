using System.IO;
using Core.Components;
using Core.Utils;

namespace App.Shared.Components.Serializer.FieldSerializer
{
    public class FixedVector3Serializer:IFieldSerializer<FixedVector3>
    {
        public void Write(FixedVector3 data, MyBinaryWriter writer)
        {
            var v = data.WorldVector3();
            writer.Write(v.x);
            writer.Write(v.y);
            writer.Write(v.z);
        }

        public FixedVector3 Read(BinaryReader reader)
        {
            FixedVector3 val = new FixedVector3
            {
                x = reader.ReadSingle(),
                y = reader.ReadSingle(),
                z = reader.ReadSingle()
            };
            return val;
        }
    }
}