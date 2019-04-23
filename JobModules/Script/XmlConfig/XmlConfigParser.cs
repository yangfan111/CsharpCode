using System.IO;
using System.Xml.Serialization;

namespace XmlConfig
{
    public static class XmlConfigParser<T> where T : class
    {
        private static XmlSerializer writer;
        static XmlConfigParser()
        {
            writer= new XmlSerializer(typeof(T));
        }

        public static T Load(string xmlStr)
        {
           
            return writer.Deserialize(new StringReader(xmlStr)) as T;
        }

        public static string Write(T t)
        {
            using(StringWriter textWriter = new StringWriter())
            {
                writer.Serialize(textWriter, t);
                return textWriter.ToString();
            }
        }
    }
}
