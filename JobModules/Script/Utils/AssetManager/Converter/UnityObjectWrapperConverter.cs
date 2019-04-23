using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utils.AssetManager.Converter
{
    public class UnityObjectWrapperConverter : ILoadRequestConverter<UnityObjectWrapper<GameObject>>
    {
        private int seq = 0;

        public UnityObjectWrapper<GameObject> Convert(Object obj, AssetInfo assetInfo)
        {
            if (obj is GameObject && obj != null)
            {
                seq++;
                return new UnityObjectWrapper<GameObject>(obj as GameObject, assetInfo, seq);
            }

            throw new ArgumentException(string.Format("{0} is not GameObject {1}", assetInfo, obj));
        }

        public bool IsMultiThreadSupport
        {
            get { return false; }
        }
    }
}