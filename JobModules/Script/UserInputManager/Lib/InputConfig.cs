using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace UserInputManager.Lib
{
    public class InputConfig : InputProviderConfig
    {
        public InputConvertItem[] Items;
    }

    public class InputProviderConfig
    {
        public string RaycastLayer;
        public float RaycastMaskDistance;
    }

    [System.Serializable]
    public class InputConvertItem
    {
        public UserInputKey Key;
        public List<KeyConvertItem> Items = new List<KeyConvertItem>();
    }

    public class InputConfigLoader<T> where T : class
    {
        public static T Load(string xmlStr)
        {
            var writer = new XmlSerializer(typeof(T));
            return writer.Deserialize(new StringReader(xmlStr)) as T;
        }
    }
}