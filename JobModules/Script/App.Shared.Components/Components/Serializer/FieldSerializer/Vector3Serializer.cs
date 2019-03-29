using System.IO;
using UnityEngine;

namespace App.Shared.Components.Serializer.FieldSerializer
{
    public class Vector3Serializer :IFieldSerializer<Vector3>
    {
        public void Write(Vector3 data, Core.Utils.MyBinaryWriter writer)
        {
            writer.Write(data.x);
            writer.Write(data.y);
            writer.Write(data.z);
        }

        public Vector3 Read(BinaryReader reader)
        {
            Vector3 val = new Vector3
            {
                x = reader.ReadSingle(),
                y = reader.ReadSingle(),
                z = reader.ReadSingle()
            };
            return val;
        }
    }
}
