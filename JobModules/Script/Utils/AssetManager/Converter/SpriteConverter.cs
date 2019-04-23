using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utils.AssetManager.Converter
{
    public class SpriteConverter : ILoadRequestConverter<UnityObject>
    {
        public UnityObject Convert(Object obj, AssetInfo assetInfo)
        {
            if (obj is Texture2D && obj != null)
            {
                Texture2D tex = obj as Texture2D;
                var s = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
                new UnityObject(s, assetInfo);
            }

            throw new ArgumentException(string.Format("{0} is not Texture2D {1}", assetInfo, obj));
        }
        public bool IsMultiThreadSupport
        {
            get { return false; }
        }
    }
}