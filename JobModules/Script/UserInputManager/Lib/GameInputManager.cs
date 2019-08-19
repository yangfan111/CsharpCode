using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace UserInputManager.Lib
{
    public class GameInputManager 
    {
        private bool enabled;
        private KeyConverter converter = new KeyConverter();
        private KeyDataCollection keyDataCollection = new KeyDataCollection();
        private PointDataCollection pointerCollection =new PointDataCollection();

        private KeyProvider keyProvider;
        private AbstractProvider<PointerReceiver, PointerData> pointerPoingProvider;
        public GameInputManager(AbstractProvider<PointerReceiver, PointerData>  poingProvider)
        {
            enabled = true;
            keyProvider = new KeyProvider();
            keyProvider.SetConverter(converter);
            keyProvider.SetCollection(keyDataCollection);

            pointerPoingProvider = poingProvider;
            pointerPoingProvider.SetConverter(converter);
            pointerPoingProvider.SetCollection(pointerCollection);
        }
        public GameInputManager()
        {
            keyProvider = new KeyProvider();
            keyProvider.SetConverter(converter);
            keyProvider.SetCollection(keyDataCollection);

            pointerPoingProvider = new PointerProvider();
            pointerPoingProvider.SetConverter(converter);
            pointerPoingProvider.SetCollection(pointerCollection);
        }

        public void SetEnable(bool isEnable)
        {
            enabled = isEnable;
        }

        public void Initialize(InputConfig cfg)
        {
            converter.SetConfig(cfg);
            keyProvider.SetConfig(cfg);
            pointerPoingProvider.SetConfig(cfg);
        }


        public void RegisterKeyReceiver(KeyReceiver receiver)
        {
            keyDataCollection.AddOne(receiver);
        }

        public void RegisterPointerReceiver(PointerReceiver receiver)
        {
            pointerCollection.AddOne(receiver);
        }

        public void UnregisterKeyReceiver(KeyReceiver receiver)
        {
            keyDataCollection.Remove(receiver);
        }

        public void UnregisterPointerReceiver(PointerReceiver receiver)
        {
            pointerCollection.Remove(receiver);
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
                pointerPoingProvider.Collect();
                DispatchPointers();
            }
        }

        public void DispatchKeys()
        {

            KeyData key = keyProvider.GetKeyData();
            while (null != key)
            {
                keyDataCollection.Dispatch(key);
                key = keyProvider.GetKeyData();
            }
        }

        public void DispatchPointers()
        {
            var pointKey = pointerPoingProvider.GetKeyData();
            
            while (null != pointKey)
            {
                pointerCollection.Dispatch(pointKey);
                pointKey = pointerPoingProvider.GetKeyData() ;
            }
          // Debug.LogFormat("[{0}]{1}",Time.frameCount,s);
        }
    }
}