using System.IO;

namespace App.Shared.Components.Serializer.FieldSerializer
{
    class LongSerializer : IFieldSerializer<long>
    {
        public void Write(long data, Core.Utils.MyBinaryWriter writer)
        {
            writer.Write(data);
        }

        public long Read(BinaryReader reader)
        {
            var data = reader.ReadInt64();
            return data;
        }
    }
}
