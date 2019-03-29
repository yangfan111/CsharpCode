using System.IO;

namespace App.Shared.Components.Serializer.FieldSerializer
{
    class FloatSerializer :IFieldSerializer<float>
    {
        public  void Write(float data,Core.Utils.MyBinaryWriter writer)
        {
            writer.Write(data);
        }

        public  float Read( BinaryReader reader)
        {
            var data = reader.ReadSingle();
            return data;
        }
    }
}
