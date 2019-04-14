using System.Collections.Generic;
using System.IO;
using App.Shared.Components.Player;
using Core;
using Core.Event;
using Core.Utils;

namespace App.Shared.Components.Serializer.FieldSerializer
{
    internal class InterruptSerializer:IFieldSerializer<InterruptData>
    {
        public void Write(InterruptData data, Core.Utils.MyBinaryWriter writer)
        {
            writer.Write(data.cmdType);
            writer.Write(data.hasValue);
            writer.Write(data.state);
        }

        public InterruptData Read(BinaryReader reader)
        {
            
            var data = new InterruptData();
            data.cmdType = reader.ReadByte();
            data.hasValue = reader.ReadBoolean();
            data.state = reader.ReadByte();

            return data;
        }
    }
}