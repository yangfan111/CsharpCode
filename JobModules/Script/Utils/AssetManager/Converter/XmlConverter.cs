using System;
using UnityEngine;
using XmlConfig;
using Object = UnityEngine.Object;

namespace Utils.AssetManager.Converter
{
    public interface IXmlAsset
    {
        
    }
    public class XmlConverter<T> : ILoadRequestConverter<T> where T : class,IXmlAsset
    {
       
        public T Convert(Object obj, AssetInfo assetInfo)
        {
            if (obj is TextAsset && obj != null)
            {
                TextAsset text = obj as TextAsset;
                return XmlConfigParser<T>.Load(text.text);
            }

            throw new ArgumentException(string.Format("{0} is not Texture2D {1}", assetInfo, obj));
        }
        public bool IsMultiThreadSupport
        {
            get { return true; }
        }
    }
}