using System;
using System.Collections.Generic;

namespace UserInputManager.Lib
{
    public interface IKeyHandler
    {
        int       GetLayer();
        BlockType GetBlockType();
    }
    public delegate void KeyPointAction(KeyData tData);
    public interface IKeyHandler<TData> : IKeyHandler where TData:KeyData
    {
        Dictionary<TData, KeyPointAction> GetBindingDict();
    }

    public sealed class KeyHandler : IKeyHandler<KeyData>
    {
        private static int staticId;
        private Dictionary<KeyData,KeyPointAction> binding =
                        new Dictionary<KeyData, KeyPointAction> (KeyDataComparer.Instance);

        public KeyHandler(int layer, BlockType block)
        {
            staticId         += 1;
            Id                =  staticId;
            handlerLayer     =  layer;
            handlerBlockType =  block;
        }

        public KeyHandler(Layer layer, BlockType block)
        {
            staticId         += 1;
            Id                =  staticId;
            handlerLayer     =  (int) layer;
            handlerBlockType =  block;
        }

        public int       Id                { get; private set; }
        public int       handlerLayer     { get; set; }
        public BlockType handlerBlockType { get; set; }

        public int GetLayer()
        {
            return handlerLayer;
        }

        public BlockType GetBlockType()
        {
            return handlerBlockType;
        }

        public Dictionary<KeyData, KeyPointAction> GetBindingDict()
        {
            return binding;
        }

        public KeyHandler BindKeyAction(UserInputKey key, KeyPointAction pointAction)
        {
            var                   data = new KeyData(key);
            KeyPointAction actionAss;
            if (binding.TryGetValue(data, out actionAss))
            {
                actionAss += pointAction;
            }
            else
            {
                binding[data] = pointAction;
            }
            return this;
        }

        public override string ToString()
        {
            return string.Format("Id : {0}, Layer : {1} ,Block : {2}", Id, handlerLayer, handlerBlockType);
        }
    }
}