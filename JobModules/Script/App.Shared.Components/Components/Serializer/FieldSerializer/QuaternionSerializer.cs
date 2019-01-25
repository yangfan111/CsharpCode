using System.IO;
using UnityEngine;

namespace App.Shared.Components.Serializer.FieldSerializer
{
    public class QuaternionSerializer :IFieldSerializer<Quaternion>
    {
        public void Write(Quaternion data, Core.Utils.MyBinaryWriter writer)
        {
            writer.Write(data.x);
            writer.Write(data.y);
            writer.Write(data.z);
            writer.Write(data.w);
        }

        public Quaternion Read(BinaryReader reader)
        {
            Quaternion data = new Quaternion
            {
                x = reader.ReadSingle(),
                y = reader.ReadSingle(),
                z = reader.ReadSingle(),
                w = reader.ReadSingle()
            };
            return data;
        }
    }
}
