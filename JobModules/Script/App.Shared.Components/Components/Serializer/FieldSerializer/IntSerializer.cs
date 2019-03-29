using System.IO;

namespace App.Shared.Components.Serializer.FieldSerializer
{
    class IntSerializer :IFieldSerializer<int>
    {
        public  void Write(int data, Core.Utils.MyBinaryWriter writer)
        {
            writer.Write(data);
        }

        public  int Read( BinaryReader reader)
        {
            var data = reader.ReadInt32();
            return data;
        }
    }
}
