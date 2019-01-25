using System.IO;

namespace App.Shared.Components.Serializer.FieldSerializer
{
    public class BoolSerializer : IFieldSerializer<bool>
    {
        public void Write(bool data, Core.Utils.MyBinaryWriter writer)
        {
            writer.Write(data);
        }

        public bool Read(BinaryReader reader)
        {
            return reader.ReadBoolean();
        }
    }
}
