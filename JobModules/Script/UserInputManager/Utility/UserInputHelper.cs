using System.Collections.Generic;
using UserInputManager.Lib;

namespace UserInputManager.Utility
{

    public class GameInputHelper 
    {
        public class KeyReceiverItem
        {
            public int Id;
            public KeyReceiver Receiver;
        }

        public class PointerhandlerItem
        {
            public int Id;
            public PointerReceiver Receiver;
        }

        private Lib.GameInputManager _manager;
        private Dictionary<int, KeyReceiverItem> _keyDic = new Dictionary<int, KeyReceiverItem>();
        private Queue<KeyReceiverItem> _avaliableKeys = new Queue<KeyReceiverItem>(); 
        private Dictionary<int, PointerhandlerItem> _pointerDic = new Dictionary<int, PointerhandlerItem>();
        private Queue<PointerhandlerItem> _avaliablePointers = new Queue<PointerhandlerItem>();
        private int _nextKeyId;
        private int NextKeyId
        {
            get
            {
                return _nextKeyId++;
            }
        }

        private int _nextPointerId;
        private int NextPointerId
        {
            get
            {
                return _nextPointerId++; 
            }
        }

        public GameInputHelper(Lib.GameInputManager manager)
        {
            _manager = manager;
        }

        public int BlockKey(EInputLayer eInputLayer)
        {
            int id = 0;
            if(_avaliableKeys.Count > 0)
            {
                var item = _avaliableKeys.Dequeue();
                item.Receiver.handlerLayer = (int)eInputLayer;
                _manager.RegisterKeyReceiver(item.Receiver);
                id = item.Id;
            }
            else
            {
                var item = new KeyReceiverItem
                {
                    Id = NextKeyId,
                    Receiver = new KeyReceiver(eInputLayer, BlockType.All),
                };
                _manager.RegisterKeyReceiver(item.Receiver);
                _keyDic[item.Id] = item;
                id = item.Id;
            }
            return id;
        }

        public void UnblockKey(int key)
        {
            if(_keyDic.ContainsKey(key))
            {
                _avaliableKeys.Enqueue(_keyDic[key]);
                _manager.UnregisterKeyReceiver(_keyDic[key].Receiver);
            }
        }

        public int BlockPointer(EInputLayer eInputLayer)
        {
            int id = 0;
            if (_avaliablePointers.Count > 0)
            {
                var item = _avaliablePointers.Dequeue();
                _manager.RegisterPointerReceiver(item.Receiver);
                id = item.Id;
            }
            else
            {
                var item = new PointerhandlerItem 
                {
                    Id = NextPointerId,
                    Receiver = new PointerReceiver(eInputLayer, BlockType.All),
                };
                _manager.RegisterPointerReceiver(item.Receiver);
                _pointerDic[item.Id] = item;
                id = item.Id;
            };
            return id;
        }

        public void UnblockPointer(int key)
        {
            if (_pointerDic.ContainsKey(key))
            {
                _avaliablePointers.Enqueue(_pointerDic[key]);
                _manager.UnregisterPointerReceiver(_pointerDic[key].Receiver);
            }
        }
    }
}
