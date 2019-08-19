using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UserInputManager.Lib
{
    public class PointerKeyHandler : IKeyHandler<PointerData>
    {
        public int handlerLayer { get; set; }
        public BlockType handlerBlockType { get; set; }
        private Dictionary<PointerData, KeyPointAction> binding = new Dictionary<PointerData, KeyPointAction>(PointerDataComparer.Instance);

        public PointerKeyHandler(int layer, BlockType block)
        {
            handlerLayer = layer;
            handlerBlockType = block;
        }

        public PointerKeyHandler(Layer layer, BlockType block)
        {
            handlerLayer = (int)layer;
            handlerBlockType = block;
        }

        public Dictionary<PointerData, KeyPointAction> GetBindingDict()
        {
            return binding;
        }

        public int GetLayer()
        {
            return handlerLayer;
        }

        public BlockType GetBlockType()
        {
            return handlerBlockType;
        }

        public PointerKeyHandler BindPointAction(UserInputKey key, KeyPointAction action)
        {
            var data = new PointerData(key);
            KeyPointAction pointActions;
            if (binding.TryGetValue(data, out pointActions))
            {
                pointActions += action;
            }
            else
            {
                binding[data] = action;
            }
            return this;
        }
    }
}
