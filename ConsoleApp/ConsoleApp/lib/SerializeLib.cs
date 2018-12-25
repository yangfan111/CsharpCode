
using System;
using System.IO;
///序列化namespace
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using ConsoleApp;

public partial class YFLib
{
    #region//bin/snoap format
    ///*.bin
    public static void SerializableObj(System.Object s_object, string outPath, SerializableType type = SerializableType.Bin)
    {
        try
        {
            switch (type)
            {
                case SerializableType.Bin:
                    Serializable_BinHandler(s_object, outPath);
                    break;
                case SerializableType.Soap:
                    Serializable_SoapHandler(s_object, outPath);
                    break;

            }
        }
        catch (System.Exception e)
        {
            Console.Write(e);
        }

    }
    ///序列化过程使用try > catch
    public static System.Object DeSerializableObj(string inPath, SerializableType type = SerializableType.Bin)
    {
        try
        {
            switch (type)
            {
                case SerializableType.Bin:
                    return DeSerializable_BinHanlder(inPath);
                case SerializableType.Soap:
                    return DeSerializable_SnoapHanlder(inPath);


            }
        }
        catch (System.Exception e)
        {
            Console.Write(e);
        }
        return null;
    }
    /// <summary>
    /// formator,stream
    /// </summary>
    /// <param name="s_object"></param>
    /// <param name="outPath"></param>
    private static void Serializable_BinHandler(System.Object s_object, string outPath)
    {
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(NormalizeFilePath(outPath), FileMode.Create,
        FileAccess.Write, FileShare.None);
        formatter.Serialize(stream, s_object);
        stream.Close();
        Console.WriteLine("Bin Serializable sucess");
    }
    ///*.xml
    private static void Serializable_SoapHandler(System.Object s_object, string outPath)
    {
        IFormatter formatter = new SoapFormatter();
        Stream stream = new FileStream(YFLib.NormalizeFilePath(outPath), FileMode.Create,
        FileAccess.Write, FileShare.None);
        //formater.Serialize(Stream,Object)
        formatter.Serialize(stream, s_object);
        stream.Close();
        Console.WriteLine("Soap Serializable sucess");
    }
    /// <summary>
    /// 反序列化- Bianry ->bin
    /// </summary>
    private static System.Object DeSerializable_BinHanlder(string inPath)
    {
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(inPath, FileMode.Open,
        FileAccess.Read, FileShare.Read);
        Object obj = formatter.Deserialize(stream);
        stream.Close();
        Console.WriteLine("DeSerializable bin sucess");
        return obj;
    }
    private static System.Object DeSerializable_SnoapHanlder(string inPath)
    {
        IFormatter formatter = new SoapFormatter();
        Stream stream = new FileStream(inPath, FileMode.Open,
        FileAccess.Read, FileShare.Read);
        System.Object obj = formatter.Deserialize(stream);
        stream.Close();
        Console.WriteLine("DeSerializable snoap sucess");
        return obj;
    }
    ///var xmlDoc = new System.Xml.XmlDocument();
    ///var xmlSerializer = new System.Xml.Serialization.XmlSerializer(T);
    ///var xmlStream = new System.IO.MemoryStream()
    ///------------------------------------------------------------------------
    ///streamWriter = new System.IO.StreamWriter(xmlStream, System.Text.Encoding.UTF8);
    ///xmlSerializer.Serialize(streamWriter, s_object);
    #endregion

    #region xml
    public static void SerializeXML(System.Object s_object, string savePath)
    {
        try
        {
            System.Type T = s_object.GetType();
            var xmlDoc = new System.Xml.XmlDocument();
            var xmlSerializer = new System.Xml.Serialization.XmlSerializer(T);
            //创建内存流
            using (var xmlStream = new System.IO.MemoryStream())
            {
                //创建流写入者
                var streamWriter = new System.IO.StreamWriter(xmlStream, System.Text.Encoding.UTF8);
               //写入流
                xmlSerializer.Serialize(streamWriter, s_object);
                xmlStream.Position = 0;
                //加载流 
                xmlDoc.Load(xmlStream);
                savePath = CombineAppPath(savePath);
                xmlDoc.Save(System.IO.Path.Combine(UnityEngine.Application.dataPath, savePath));

            }
        }
        catch (System.Exception)
        {
        }
    }
    
    public static void DeSerializeXML(System.Object s_object,string tarFileName)
    {
        try
        {
            if (SeekAppDataDirTargetFile(tarFileName))
            {
                System.Type T = s_object.GetType();
                var xmlSerializer = new System.Xml.Serialization.XmlSerializer(T);
                //创建文件流
                var xmlFileStream = new System.IO.FileStream(CombineAppPath(tarFileName),
                    System.IO.FileMode.Open, System.IO.FileAccess.Read);
                //加载流
                var Settings = (WwiseSettings)xmlSerializer.Deserialize(xmlFileStream);
                xmlFileStream.Close();
            }
            else
            {
                //do sth
            }
        }catch(System.Exception e) { Console.Write(e); }
     
    }

    #endregion

}
