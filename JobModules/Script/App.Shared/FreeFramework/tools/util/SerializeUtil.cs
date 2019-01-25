using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Sharpen;

namespace com.cpkf.yyjd.tools.util
{
	public class SerializeUtil
	{

	    public static object Clone(object obj)
	    {
	        if (obj == null)
	        {
	            return null;
	        }
	        return ByteToObject(ObjectToByte(obj));
	    }

		public static object ByteToObject(byte[] bytes)
		{
		    var ms = new MemoryStream(bytes);

		    var bf = new BinaryFormatter();

		    var obj = bf.Deserialize(ms);

		    ms.Close();

		    return obj;
        }

		public static string ObjectToByteString(object obj)
		{
			byte[] bytes = ObjectToByte(obj);
			StringBuilder sb = new StringBuilder();
			foreach (byte bt in bytes)
			{
				sb.Append(bt + " ");
			}
			return sb.ToString().Trim();
		}

		public static object ByteStringToObject(string byteString)
		{
			string[] ss = byteString.Trim().Split(" ");
			byte[] bytes = new byte[ss.Length];
			for (int i = 0; i < ss.Length; i++)
			{
				bytes[i] = byte.Parse(ss[i]);
			}
			return ByteToObject(bytes);
		}

		public static byte[] ObjectToByte(object obj)
		{
		    var ms = new MemoryStream();

            var bf = new BinaryFormatter();

		    bf.Serialize(ms, obj);

            ms.Close();

		    return ms.ToArray();
		}
	}
}
