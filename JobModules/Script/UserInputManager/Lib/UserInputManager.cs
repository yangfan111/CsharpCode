using System;
using System.Collections.Generic;

namespace UserInputManager.Lib
{
    public class UserInputManager 
    {
        private bool enabled;
        private KeyConverter converter = new KeyConverter();
        private KeyDataCollection keyCollection = new KeyDataCollection();
        private PointDataCollection pointerCollection =new PointDataCollection();

        private KeyProvider keyProvider;
        private PointerProvider pointerProvider;
        public Dictionary<UserInputKey, List<KeyConvertItem>> InputConvertDict
        {
            get { return converter.InputConvertDict; }
        }
        public UserInputManager()
        {

            keyProvider = new KeyProvider();
            keyProvider.SetConverter(converter);
            keyProvider.SetCollection(keyCollection);

            pointerProvider = new PointerProvider();
            pointerProvider.SetConverter(converter);
            pointerProvider.SetCollection(pointerCollection);
        }

        public void SetEnable(bool isEnable)
        {
            enabled = isEnable;
        }

        public void Initialize(InputConfig cfg)
        {
            converter.SetConfig(cfg);
            keyProvider.SetConfig(cfg);
            pointerProvider.SetConfig(cfg);
        }


        public void RegisterKeyhandler(KeyHandler handler)
        {
            keyCollection.AddOne(handler);
        }

        public void RegisterPointerhandler(PointerKeyHandler keyHandler)
        {
            pointerCollection.AddOne(keyHandler);
        }

        public void UnregisterKeyhandler(KeyHandler handler)
        {
            keyCollection.Remove(handler);
        }

        public void UnregisterPointerhandler(PointerKeyHandler keyHandler)
        {
            pointerCollection.Remove(keyHandler);
        }

        public void InsertKey(KeyData key)
        {
            keyProvider.Insert(key);
        }

        public void Dispatch()
        {
            if (enabled)
            {
                keyProvider.Collect();
                DispatchKeys();
                pointerProvider.Collect();
                DispatchPointers();
            }
        }

        public void DispatchKeys()
        {

            KeyData key = keyProvider.GetKeyData();
            while (null != key)
            {
                keyCollection.Dispatch(key);
                key = keyProvider.GetKeyData();
            }
        }

        public void DispatchPointers()
        {
            KeyData pointKey = pointerProvider.GetKeyData();
            while (null != pointKey)
            {
                pointerCollection.Dispatch(pointKey);
                pointKey = pointerProvider.GetKeyData();
            }
        }
    }
}