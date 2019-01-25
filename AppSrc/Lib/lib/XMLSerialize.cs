using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace YF.FileUtil.Serialize
{
    public static class XMLSerialize
    {
        ///stage1:
        ///new XMLDoc() ,new MemStream
        ///XMLDoc.load(xmlStream)
        ///XMLDoc.Save(path)
        ///******************************
        ///stage2: memStream -> streamWriter->xmlSerializer
        


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
                savePath = FS.GetPath_StreamingAsset_CustomFolder(savePath);
                xmlDoc.Save(System.IO.Path.Combine(UnityEngine.Application.dataPath, savePath));

            }

        }
        public static T DeSerializeXMLByText<T>(string text) where T:class
        {

            try
            {

                System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                //创建文件流
                return xmlSerializer.Deserialize(new StringReader(text)) as T;

            }
            catch (System.Exception e) { Console.Write(e); }
            return null;

        }
        public static System.Object DeSerializeXML(System.Type T, string tarFileName)
        {
            try
            {

                System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(T);
                //创建文件流
                var xmlFileStream = new System.IO.FileStream(tarFileName,
                    System.IO.FileMode.Open, System.IO.FileAccess.Read);
                //加载流
                var Settings = xmlSerializer.Deserialize(xmlFileStream);
                xmlFileStream.Close();
                return Settings;


            }
            catch (System.Exception e) { Console.Write(e); }
            return null;
        }
    }
}
