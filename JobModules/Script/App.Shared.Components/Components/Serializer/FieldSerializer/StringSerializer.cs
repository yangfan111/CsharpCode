using System.IO;
using Core.Utils;

namespace App.Shared.Components.Serializer.FieldSerializer
{
    public class StringSerializer :IFieldSerializer<string>
    {
         private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(StringSerializer)); 
        public void Write(string data, Core.Utils.MyBinaryWriter writer)
        {

            if (data == null)
            {
                Logger.InfoFormat("StringSerializer String is Null");
                data = string.Empty;
            }
            writer.Write(data);
        }

        public string Read(BinaryReader reader)
        {
            return reader.ReadString();
        }
    }
}
