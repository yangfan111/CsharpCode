// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using YF;
// using YF.FileUtil;
//
// using System.Xml.Serialization;
//
// public enum CopyCmdType
// {
//     CopyFile,
//     De_CopyFile,
// }
// class Program
// {
//     static void Main(string[] args)
//     {
//         //Console.WriteLine("Sucess");
//         //   Console.WriteLine(args[0]);
//
//         CopyCmdType cmd = (CopyCmdType)int.Parse(args[0]);
//         switch(cmd)
//         {
//             case CopyCmdType.CopyFile:
//                 CopyImp.CopyProjPackages();
//                 break;
//             case CopyCmdType.De_CopyFile:
//                 CopyImp.DeCopyProjPackages();
//                 break;
//         }
//       
//         Console.Write("Copy Sucess");
//             //  _URI.MakRelative();
//     
//         if (Console.ReadKey() != null)
//         {
//             return;
//         }
//     }
//
//     static void Copy()
//     {
//         string spath = @"F:\voyager\UnityPackages\Assets\Configuration\Sound.xml";
//         var sconfig = new SoundConfig();
//         XmlSerializer xmlSerializer = new XmlSerializer(sconfig.GetType());
//     }
//
//     [XmlRoot("SoundConfig")]
//     public class SoundConfig
//     {
//         [XmlArray("Items")]
//         public SoundItem[] Items;
//     }
//     public class SoundItem
//     {
//         public int Id = 1;
//     }
//
// }
// [Serializable]
// public class SD_Strs
// {
//     public string DS;
//     public string SS;
//     public SD_Strs(string s, string d)
//     {
//         DS = d;
//         SS = s;
//     }
//
// }
// [Serializable]
// public class ImplentConfig
// {
//
//     public  List<SD_Strs> SDList = new List<SD_Strs>
//     {
//         new SD_Strs(
//             @"F:\voyager\UnityPackages\Wwise\Deployment\API\",
//             @"E:\Github\CsharpCode\JobModules\Wise_Unity\API"),
//         new SD_Strs(
//               @"F:\voyager\UnityPackages\Wwise\Deployment\Components",
//               @"E:\Github\CsharpCode\JobModules\Wise_Unity\Components"),
//         new SD_Strs(
//             @"f:\voyager\UnityProjects\App.Client.Unity\App.Client.Unity-Windows\Assets\Core\Audio\",
//             @"E:\Github\CsharpCode\JobModules\Wise_Imp\"),
//
//
//     };
//
// }
//
// public class CopyImp
// {
//     public static readonly string configPath = @"e:\Github\CsharpCode\AppSrc\Apps\CopyApp\config.json";
//     public static readonly string d_configPath = @"e:\Github\CsharpCode\AppSrc\Apps\CopyApp\d_config.json";
//     public static void ExportXML()
//     {
//         var obj = new ImplentConfig();
//         NormalSerialize.SerializableObj(obj, configPath, YF.SerializableType.Json);
//         //   Seri.SerializeXML(obj, @"e:\Github\CsharpCode\AppSrc\Apps\CopyApp\config.xml");
//
//     }
//     public static void CopyProjPackages()
//     {
//         NormalSerialize.myJsonDeSeriType = typeof(ImplentConfig);
//         var obj = NormalSerialize.DeSerializableObj(configPath,YF.SerializableType.Json);
//         var imp = obj as ImplentConfig;
//         foreach (var data in imp.SDList)
//         {
//             FS.FileOrDicretoryCopy(data.SS, data.DS, true);
//         }
//     }
//     public static void DeCopyProjPackages()
//     {
//         NormalSerialize.myJsonDeSeriType = typeof(ImplentConfig);
//         var obj = NormalSerialize.DeSerializableObj(d_configPath, YF.SerializableType.Json);
//         var imp = obj as ImplentConfig;
//         foreach (var data in imp.SDList)
//         {
//             FS.FileOrDicretoryCopy(data.SS, data.DS, true);
//         }
//     }
//     public static void CopyWise()
//     {
//
//     }
// }
//
