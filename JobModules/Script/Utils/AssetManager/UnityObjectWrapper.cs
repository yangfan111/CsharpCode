using ArtPlugins;
using UnityEngine;

namespace Utils.AssetManager
{

    public class UnityObjectWrapper<T> where T : Object
    {
        protected T Value;
        public AssetInfo Address { get; private set; }

        public bool Available
        {
            get { return Value != null; }
        }

        public int Id { get; private set; }

        private SceneObjectAttribute _sceneObjAttr;
        public SceneObjectAttribute SceneObjAttr
        {
            get
            {
                if (_sceneObjAttr == null) _sceneObjAttr = new SceneObjectAttribute();
                return _sceneObjAttr;
            }
        }

        public UnityObjectWrapper(T value, AssetInfo address, int id)
        {
            Value = value;
            Address = address;
            Id = id;
        }

        public void Destroy()
        {
            if (Value != null && Value is GameObject)
            {
                Object.Destroy(Value);
            }

            Value = null;
        }
    }

    public class SceneObjectAttribute
    {
        public int Id;
        public MultiTagBase Tag;
    }
}