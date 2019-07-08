using System.Collections.Generic;
using UserInputManager.Lib;

namespace App.Client.GameModules.UserInput
{
    public class InputProcessor : IInputProcessor
    {
        private IDispatcher<IKeyReceiver> _keyDispatcher;
        private IDispatcher<IPointerReceiver> _pointerDispatcher;
        private IKeyConverter _converter = new KeyConverter();
        private IKeyProvider<IKeyReceiver> _keyProvider = new KeyProvider();
        private IKeyProvider<IPointerReceiver> _pointerProvider = new PointerProvider(null);
        private ReceiverDataCollection<IKeyReceiver> _keyCollection = new ReceiverDataCollection<IKeyReceiver>();
        private ReceiverDataCollection<IPointerReceiver> _pointerCollection = new ReceiverDataCollection<IPointerReceiver>();
        private DataPool<KeyData> _keyDataPool;
        private DataPool<PointerData> _pointerDataPool;

        public void SetKeyDispatcher(IDispatcher<IKeyReceiver> dispatcher)
        {
            _keyDispatcher = dispatcher;
            _keyDispatcher.SetCollection(_keyCollection);
        }

        public void SetPointerDispatcher(IDispatcher<IPointerReceiver> dispatcher)
        {
            _pointerDispatcher = dispatcher;
            _pointerDispatcher.SetCollection(_pointerCollection);
        }

        public void SetConfig(InputConfig cfg)
        {
            if (null != _converter)
            {
                _converter.SetConfig(cfg);
            }
            else
            {
                System.Console.WriteLine("Error: convert is null !!");
            }
            if(null != _keyProvider)
            {
                _keyProvider.SetConfig(cfg);
            }
            if(null != _pointerProvider)
            {
                _pointerProvider.SetConfig(cfg);
            }
        }

        public void SetConverter(IKeyConverter convert)
        {
            _converter = convert;
        }

        public void SetKeyProvider(IKeyProvider<IKeyReceiver> provider)
        {
            _keyProvider = provider;
            _keyProvider.SetConverter(_converter);
            _keyProvider.SetCollection(_keyCollection);
        }

        public void SetPointerProvider(IKeyProvider<IPointerReceiver> provider)
        {
            _pointerProvider = provider;
            _pointerProvider.SetConverter(_converter);
            _pointerProvider.SetCollection(_pointerCollection);
        }

        public void SetKeyCollection(ReceiverDataCollection<IKeyReceiver> collection)
        {
            _keyCollection = collection;
        }

        public void SetPointerCollection(ReceiverDataCollection<IPointerReceiver> collection)
        {
            _pointerCollection = collection;
        }

        public void AddKeyReceiver(IKeyReceiver receiver)
        {
            _keyCollection.Add(receiver);
        }

        public void AddPointerReceiver(IPointerReceiver receiver)
        {
            _pointerCollection.Add(receiver);
        }

        public void RemoveKeyReceiver(IKeyReceiver receiver)
        {
            _keyCollection.Remove(receiver);
        }

        public void RemovePointerReceiver(IPointerReceiver receiver)
        {
            _pointerCollection.Remove(receiver);
        }

        public void InsertKey(KeyData key)
        {
            _keyProvider.Insert(key);
        }

        public void Dispatch()
        {
            _keyProvider.Collect();
            DispatchKeys();
            _pointerProvider.Collect();
            DispatchPointers();
        }

        public void DispatchKeys()
        {
            if (null == _keyDispatcher)
            {
                System.Console.WriteLine("KeyDispatcher is null");
                return;
            }
            var key = _keyProvider.GetKeyData();
            while (null != key)
            {
                _keyDispatcher.Dispatch(key);
                key = _keyProvider.GetKeyData();
            }
        }

        public void DispatchPointers()
        {
            if (null == _pointerDispatcher)
            {
                System.Console.WriteLine("PointerDispatcher is null");
                return;
            }
            var data = _pointerProvider.GetKeyData();
            while (null != data)
            {
                _pointerDispatcher.Dispatch(data);
                data = _pointerProvider.GetKeyData();
            }
        }

        public void SetKeyDataPool(DataPool<KeyData> pool)
        {
            _keyDataPool = pool;
        }

        public void SetPointerDataPool(DataPool<PointerData> pool)
        {
            _pointerDataPool = pool;
        }

        public Dictionary<UserInputKey, List<KeyConvertItem>> GetInputDic()
        {
            return _converter.GetInputDic();
        }
    }
}
