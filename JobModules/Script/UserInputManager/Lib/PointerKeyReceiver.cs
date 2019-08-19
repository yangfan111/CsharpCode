using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UserInputManager.Lib
{
    public class PointerReceiver : IKeyReceiver<PointerData>
    {
        public int handlerLayer { get; set; }
        public BlockType handlerBlockType { get; set; }
        private Dictionary<PointerData, KeyPointAction> binding = new Dictionary<PointerData, KeyPointAction>(PointerDataComparer.Instance);

        public PointerReceiver(int layer, BlockType block)
        {
            handlerLayer = layer;
            handlerBlockType = block;
        }

        public PointerReceiver(EInputLayer eInputLayer, BlockType block)
        {
            handlerLayer = (int)eInputLayer;
            handlerBlockType = block;
        }

        public Dictionary<PointerData, KeyPointAction> GetBindingActions()
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

        public PointerReceiver BindPointAction(UserInputKey key, KeyPointAction action)
        {
            var data = new PointerData(key);
            KeyPointAction pointActions;
            if (binding.TryGetValue(data, out pointActions))
            {
                pointActions += action;
            }
            else
            {
                pointActions = action;
            }

            binding[data] = pointActions;
            return this;
        }
    }
}
