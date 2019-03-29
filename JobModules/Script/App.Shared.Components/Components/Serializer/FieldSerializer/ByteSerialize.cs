using System.IO;

namespace App.Shared.Components.Serializer.FieldSerializer
{
    class ByteSerializer :IFieldSerializer<byte>
    {
        public  void Write(byte data, Core.Utils.MyBinaryWriter writer)
        {
            writer.Write(data);
        }

        public  byte Read( BinaryReader reader)
        {
            var data = reader.ReadByte();
            return data;
        }
    }
}
