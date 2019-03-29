using System.IO;

namespace App.Shared.Components.Serializer.FieldSerializer
{
    class ShortSerializer :IFieldSerializer<short>
    {
        public  void Write(short data, Core.Utils.MyBinaryWriter writer)
        {
            writer.Write(data);
        }

        public  short Read( BinaryReader reader)
        {
            var data = reader.ReadInt16();
            return data;
        }
    }
}
