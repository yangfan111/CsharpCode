using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YF.FileUtil;
class Program
{
    static void Main(string[] args)
    {
        //Console.WriteLine("Sucess");
        Console.WriteLine(args);
        CopyImp.CopyProjPackages();
            Console.Write("Copy Sucess");
            //  _URI.MakRelative();
    
        if (Console.ReadKey() != null)
        {
            return;
        }
    }

}
[Serializable]
public class SD_Strs
{
    public string DS;
    public string SS;
    public SD_Strs(string s, string d)
    {
        DS = d;
        SS = s;
    }

}
[Serializable]
public class ImplentConfig
{

    public  List<SD_Strs> SDList = new List<SD_Strs>
    {
        new SD_Strs(
            @"F:\voyager\UnityPackages\Wwise\Deployment\API\",
            @"E:\Github\CsharpCode\JobModules\Wise_Unity\API"),
        new SD_Strs(
              @"F:\voyager\UnityPackages\Wwise\Deployment\Components",
              @"E:\Github\CsharpCode\JobModules\Wise_Unity\Components"),
        new SD_Strs(
            @"f:\voyager\UnityProjects\App.Client.Unity\App.Client.Unity-Windows\Assets\Core\Audio\",
            @"E:\Github\CsharpCode\JobModules\Wise_Imp\"),


    };

}

public class CopyImp
{
    public static readonly string configPath = @"e:\Github\CsharpCode\AppSrc\Apps\CopyApp\config.json";
    public static void ExportXML()
    {
        var obj = new ImplentConfig();
        Seri.SerializableObj(obj, configPath, YF.SerializableType.Json);
        //   Seri.SerializeXML(obj, @"e:\Github\CsharpCode\AppSrc\Apps\CopyApp\config.xml");

    }
    public static void CopyProjPackages()
    {
        Seri.myJsonDeSeriType = typeof(ImplentConfig);
        var obj = Seri.DeSerializableObj(configPath,YF.SerializableType.Json);
        var imp = obj as ImplentConfig;
        foreach (var data in imp.SDList)
        {
            FS.FileOrDicretoryCopy(data.SS, data.DS, true);
        }
    }

    public static void CopyWise()
    {

    }
}

