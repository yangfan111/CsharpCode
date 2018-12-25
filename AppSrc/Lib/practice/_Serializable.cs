using System;
using System.IO;
///序列化namespace
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using YF;
namespace YF.Test
{

    [Serializable]
    public class SerializeObject
    {
        public static SerializeObject DefualtSerializeObject = new CustomSerialize() { num = 99, n1 = 1000 };
        public int n1 { get; set; }
        public int num { get; set; }
        public string str { get; set; }
        // private string str2;
    }
    //自定义序列化文件
    [Serializable]
    public class CustomSerialize : SerializeObject, ISerializable
    {


        protected CustomSerialize(SerializationInfo si, StreamingContext context)
        {
            num = si.GetInt32("num");
        }
        public CustomSerialize() { }

        //   protected CustomSerialize(SerializationInfo si, StreamingContext context) :
        //base(si, context)
        //   {
        //       num = si.GetInt32("num");
        //   }

        public void GetObjectData(SerializationInfo si,
     StreamingContext context)
        {
            num = 99;
            si.AddValue("num", num);
        }
    }
    [Serializable]
    public class MSerializeObject : SerializeObject
    {
        public int abc = 999;
    }

    public static class SerializableProcess
    {
        /// Format: IFormatter,BinaryFormatter
        /// Stream:FileStream
        /// formatter.Serialize(stream, s_object):序列化(文件流->文件object)
        /// <param name="s_object"></param>
        public static void SerializableObj(SerializableType type = SerializableType.Bin, SerializeObject s_object = null)
        {

            s_object = s_object != null ? s_object : SerializeObject.DefualtSerializeObject;
            s_object.n1 = 1;
            s_object.num = 99;
            s_object.str = "一些字符串";
            switch (type)
            {
                case SerializableType.Bin:
                    BinFormatHandler(s_object);
                    break;
                case SerializableType.Soap:
                    SoapFormatHandler(s_object);
                    break;

            }


        }
        /// <summary>
        /// 序列化 - bin 
        /// </summary>
        /// <param name="s_object"></param>
        private static void BinFormatHandler(SerializeObject s_object)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(YF.FileUtil.PS.NormalizePath("e:/Github/output.bin"), FileMode.Create,
            FileAccess.Write, FileShare.None);
            //formater.Serialize(Stream,Object)
            formatter.Serialize(stream, s_object);
            stream.Close();
            Console.WriteLine("Bin Serializable sucess");
        }
        /// <summary>
        /// 序列化->soap
        /// </summary>
        /// <param name="s_object"></param>
        private static void SoapFormatHandler(SerializeObject s_object)
        {
            IFormatter formatter = new SoapFormatter();
            Stream stream = new FileStream(YF.FileUtil.PS.NormalizePath("e:/Github/output.xml"), FileMode.Create,
            FileAccess.Write, FileShare.None);
            //formater.Serialize(Stream,Object)
            formatter.Serialize(stream, s_object);
            stream.Close();
            Console.WriteLine("Soap Serializable sucess");
        }
        public static void DeSerializableObj(SerializableType type = SerializableType.Bin)
        {
            switch (type)
            {
                case SerializableType.Bin:
                    DeSerializableBinObjHanlder();
                    break;
                case SerializableType.Soap:
                    DeSerializableSnoapObjHanlder();
                    break;

            }
        }
        /// <summary>
        /// 反序列化- Bianry ->bin
        /// </summary>
        static void DeSerializableBinObjHanlder()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("e:/Github/output.bin", FileMode.Open,
            FileAccess.Read, FileShare.Read);
            SerializeObject obj = (SerializeObject)formatter.Deserialize(stream);
            stream.Close();
            Console.WriteLine(obj.num);
            Console.WriteLine("DeSerializable bin sucess");
        }
        /// <summary>
        /// 反序列化-Snoap ->xml
        /// </summary>
        static void DeSerializableSnoapObjHanlder()
        {
            IFormatter formatter = new SoapFormatter();
            Stream stream = new FileStream("e:/Github/output.xml", FileMode.Open,
            FileAccess.Read, FileShare.Read);
            SerializeObject obj = (SerializeObject)formatter.Deserialize(stream);
            stream.Close();
            Console.WriteLine(obj.num);
            Console.WriteLine("DeSerializable snoap sucess");
        }

    }

}
