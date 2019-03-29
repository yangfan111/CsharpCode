using System.IO;

namespace App.Shared.Components.Serializer.FieldSerializer
{
    class DoubleSerializer :IFieldSerializer<double>
    {
        public  void Write(double data, Core.Utils.MyBinaryWriter writer)
        {
            writer.Write(data);
        }

        public  double Read(BinaryReader reader)
        {
            var data = reader.ReadDouble();
            return data;
        }
    }
}
