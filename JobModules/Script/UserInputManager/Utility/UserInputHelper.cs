using System.Collections.Generic;
using UserInputManager.Lib;

namespace UserInputManager.Utility
{

    public class UserInputHelper 
    {
        public class KeyhandlerItem
        {
            public int Id;
            public KeyHandler Handler;
        }

        public class PointerhandlerItem
        {
            public int Id;
            public PointerKeyHandler KeyHandler;
        }

        private Lib.UserInputManager _manager;
        private Dictionary<int, KeyhandlerItem> _keyDic = new Dictionary<int, KeyhandlerItem>();
        private Queue<KeyhandlerItem> _avaliableKeys = new Queue<KeyhandlerItem>(); 
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

        public UserInputHelper(Lib.UserInputManager manager)
        {
            _manager = manager;
        }

        public int BlockKey(Layer layer)
        {
            int id = 0;
            if(_avaliableKeys.Count > 0)
            {
                var item = _avaliableKeys.Dequeue();
                item.Handler.handlerLayer = (int)layer;
                _manager.RegisterKeyhandler(item.Handler);
                id = item.Id;
            }
            else
            {
                var item = new KeyhandlerItem
                {
                    Id = NextKeyId,
                    Handler = new KeyHandler(layer, BlockType.All),
                };
                _manager.RegisterKeyhandler(item.Handler);
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
                _manager.UnregisterKeyhandler(_keyDic[key].Handler);
            }
        }

        public int BlockPointer(Layer layer)
        {
            int id = 0;
            if (_avaliablePointers.Count > 0)
            {
                var item = _avaliablePointers.Dequeue();
                _manager.RegisterPointerhandler(item.KeyHandler);
                id = item.Id;
            }
            else
            {
                var item = new PointerhandlerItem 
                {
                    Id = NextPointerId,
                    KeyHandler = new PointerKeyHandler(layer, BlockType.All),
                };
                _manager.RegisterPointerhandler(item.KeyHandler);
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
                _manager.UnregisterPointerhandler(_pointerDic[key].KeyHandler);
            }
        }
    }
}
