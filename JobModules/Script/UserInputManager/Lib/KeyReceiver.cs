using System;
using System.Collections.Generic;

namespace UserInputManager.Lib
{
    public interface IKeyReceiver
    {
        int       GetLayer();
        BlockType GetBlockType();
    }
    public delegate void KeyPointAction(KeyData tData);
    public interface IKeyReceiver<TData> : IKeyReceiver where TData:KeyData
    {
        Dictionary<TData, KeyPointAction> GetBindingActions();
    }

    public sealed class KeyReceiver : IKeyReceiver<KeyData>
    {
        private static int staticId;
        private Dictionary<KeyData,KeyPointAction> dataActions =
                        new Dictionary<KeyData, KeyPointAction> (KeyDataComparer.Instance);

        public string handlerName;
        public KeyReceiver(int layer, BlockType block,string handlerName ="")
        {
            staticId         += 1;
            Id                =  staticId;
            handlerLayer     =  layer;
            handlerBlockType =  block;
            this.handlerName = handlerName;
        }

        public KeyReceiver(EInputLayer eInputLayer, BlockType block,string handlerName ="")
        {
            staticId         += 1;
            Id                =  staticId;
            handlerLayer     =  (int) eInputLayer;
            handlerBlockType =  block;
            this.handlerName = handlerName;

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

        public Dictionary<KeyData, KeyPointAction> GetBindingActions()
        {
            return dataActions;
        }

        public KeyReceiver BindKeyAction(UserInputKey key, KeyPointAction keyPointAction)
        {
            var                   data = new KeyData(key);
            KeyPointAction actionAss;
            if (dataActions.TryGetValue(data, out actionAss))
            {
                actionAss += keyPointAction;
            }
            else
            {
                actionAss = keyPointAction;
            }
            dataActions[data] = actionAss;
            return this;
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2}", handlerName, handlerLayer, handlerBlockType);
        }
    }
}