using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
///序列化namespace
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using Newtonsoft.Json;
namespace YF.FileUtil
{
    /// <summary>
    /// 具体见Xmind
    /// </summary>

    public static class Seri
    {
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
                    case SerializableType.Json:
                        Serializable_JsonHandler(s_object, outPath);
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
                    case SerializableType.Json:
                        return DeSerializable_JsonHandler(inPath);


                }
            }
            catch (System.Exception e)
            {
                Console.Write(e);
            }
            return null;
        }
        ///new Stream,new IFormatter
        ///IFormatter.Serialize(Stream)
        /// stream.Close()
        private static void Serializable_BinHandler(System.Object s_object, string outPath)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(PS.NormalizePath(outPath), FileMode.Create,
            FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, s_object);
            stream.Close();
            Console.WriteLine("Bin Serializable sucess");
        }
        ///*.xml
        private static void Serializable_SoapHandler(System.Object s_object, string outPath)
        {
            IFormatter formatter = new SoapFormatter();
            Stream stream = new FileStream(PS.NormalizePath(outPath), FileMode.Create,
            FileAccess.Write, FileShare.None);
            //formater.Serialize(Stream,Object)
            formatter.Serialize(stream, s_object);
            stream.Close();
            Console.WriteLine("Soap Serializable sucess");
        }
        ///简单的Json序列化,复杂需求再写单独类库
        private static void Serializable_JsonHandler(System.Object s_object, string outPath)
        {
            string jsonStr = JsonConvert.SerializeObject(s_object, Formatting.Indented);
            //IFormatter formatter = new SoapFormatter();
            //Stream stream = new FileStream(PS.NormalizePath(outPath), FileMode.Create,
            //FileAccess.Write, FileShare.None);
            File.WriteAllText(outPath, jsonStr);
            Console.WriteLine("Json Serializable sucess");
        }
        public static Type myJsonDeSeriType;
        private static System.Object DeSerializable_JsonHandler(string outPath)
        {

            string str = File.ReadAllText(outPath);
            var obj = JsonConvert.DeserializeObject(str, myJsonDeSeriType);
            myJsonDeSeriType = null;
            return obj;

        }
        ///相同操作
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
        ///stage1:
        ///new XMLDoc() 
        ///XMLDoc.load(xmlStream)
        ///XMLDoc.Save(path)
        ///******************************
        ///stage2:
        ///var xmlStream = new System.IO.MemoryStream()
        ///var streamWriter = new System.IO.StreamWriter(xmlStream, System.Text.Encoding.UTF8);
        public static void SerializeXML(System.Object s_object, string savePath)
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
                savePath = FS.GetPath_PlatformStream(savePath);
                xmlDoc.Save(System.IO.Path.Combine(UnityEngine.Application.dataPath, savePath));

            }

        }

        public static void DeSerializeXML(System.Object s_object, string tarFileName)
        {
            try
            {
                if (FS.SeekAppDataDirTargetFile(tarFileName))
                {
                    System.Type T = s_object.GetType();
                    var xmlSerializer = new System.Xml.Serialization.XmlSerializer(T);
                    //创建文件流
                    var xmlFileStream = new System.IO.FileStream(FS.GetPath_PlatformStream(tarFileName),
                        System.IO.FileMode.Open, System.IO.FileAccess.Read);
                    //加载流
                    var Settings = (WwiseSettings)xmlSerializer.Deserialize(xmlFileStream);
                    xmlFileStream.Close();
                }
                else
                {
                    //do sth
                }
            }
            catch (System.Exception e) { Console.Write(e); }

        }


    }
}
