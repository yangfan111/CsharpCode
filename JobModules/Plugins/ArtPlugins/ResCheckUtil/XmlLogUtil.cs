#if UNITY_EDITOR
using System.IO;
using System.Xml.Serialization;

//namespace Assets.Editor.packageUtils
//{
public class XmlLogUtil<T> where T : class
{
    public static void Write(T t, string path)
    {
        CreateDir(path);
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        TextWriter writer = new StreamWriter(path);
        serializer.Serialize(writer, t);
        writer.Close();
    }

    /**public static void TransformToHtml(string xmlPath, string xsltPath, string htmlPath)
    {
        CreateDir(htmlPath);
        XmlDocument doc = new XmlDocument();
        doc.Load(xmlPath);
        XslCompiledTransform transform = new XslCompiledTransform();
        transform.Load(xsltPath);
        TextWriter writer = new StreamWriter(htmlPath);
        transform.Transform(doc, null, writer);
        writer.Close();
    }**/

    public static void CreateDir(string path)
    {
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }

}
//}


#endif
