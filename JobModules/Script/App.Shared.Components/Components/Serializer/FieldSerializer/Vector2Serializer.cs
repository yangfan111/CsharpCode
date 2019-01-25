using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace App.Shared.Components.Serializer.FieldSerializer
{
    public class Vector2Serializer : IFieldSerializer<Vector2>
    {
        public void Write(Vector2 data, Core.Utils.MyBinaryWriter writer)
        {
            writer.Write(data.x);
            writer.Write(data.y);
        }

        public Vector2 Read(BinaryReader reader)
        {
            Vector2 val = new Vector2
            {
                x = reader.ReadSingle(),
                y = reader.ReadSingle(),
            };
            return val;
        }
    }
}
