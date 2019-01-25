using Assets.Plugins.ArtPlugins.Editor;
using System.IO;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;


public class SoundFxDistanceProcessor
{
    public class AssetInfo
    {
        [XmlAttribute()] public string BundleName;
        [XmlAttribute()] public string AssetName;
    }

    public class SoundConfig
    {
        public SoundConfigItem[] Items;
    }

    public class SoundConfigItem
    {
        public int Id;
        public AssetInfo Asset;
        public bool Sync;
        public int Delay;
        public float Distance;
    }

    public class XmlConfigParser<T> where T : class
    {
        public static T Load(string xmlStr)
        {
            var writer = new XmlSerializer(typeof(T));
            return writer.Deserialize(new StringReader(xmlStr)) as T;
        }
    }

    [MenuItem("Assets/资源生成检查/更新音效配置的距离参数")]
    static void ProcessSoundFxDistance()
    {
        var xmlPath = "Assets/Assets/Configuration/Sound.xml";
        var prefabs = CommonEditorUtility.GetAllPrefabInSelectedFolder();
        var xmlAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(xmlPath);
        var content = xmlAsset.text;
        var soundConfig = XmlConfigParser<SoundConfig>.Load(content);

        foreach (var prefab in prefabs)
        {
            var soundName = prefab.Go.name.Replace(".prefab", "");
            Debug.Log(soundName);
            foreach (var cfg in soundConfig.Items)
            {
                if (cfg.Asset.AssetName == soundName)
                {
                    var audioSrc = prefab.Go.GetComponent<AudioSource>();
                    Debug.LogFormat("set distance {0}", audioSrc.maxDistance);
                    cfg.Distance = audioSrc.maxDistance;
                }
            }
        }
        XmlSerializer writer = new XmlSerializer(typeof(SoundConfig));
        var file = File.Create(xmlPath);
        writer.Serialize(file, soundConfig);
        file.Close();
    }
}
