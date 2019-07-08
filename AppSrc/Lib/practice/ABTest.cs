using System.Collections;
using System.IO;
using UnityEngine;
namespace Lib.practice
{
    public class ABLoadTest
    {
        public static IEnumerator LoadABFromMemoryAsync(string path)
        {
            //序列化的二进制流 :任何读取byteArray方法都可以
            var fileBytes = File.ReadAllBytes(path);
            //建立request
            AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync(fileBytes);
            yield return request;
            //下载下来的bundle
            AssetBundle requestAB = request.assetBundle;
            //取到bundle内部物体
            var prefab = requestAB.LoadAsset<GameObject>("MyObject");
            
        }
        
    }
}