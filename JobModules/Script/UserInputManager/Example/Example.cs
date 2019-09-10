using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using UserInputManager.Lib;

namespace UserInputManager.Example
{
    public class Example
    {
        private class ReceiveItem
        {
            public ReceiveItem(int layer, BlockType block, UserInputKey key, string log)
            {
                Layer = layer;
                Block = block;
                Log = log;
                Key = key;
            }

            public int Layer { get; set; }
            public BlockType Block { get; set; }
            public string Log { get; set; }
            public UserInputKey Key { get; set; }
        }

        private List<KeyHandler>_handlerList = new List<KeyHandler>();
        private List<ReceiveItem> _receiveItemList = new List<ReceiveItem>();
        private Lib.UserInputManager _manager;
        private void Run()
        {
            _manager = new Lib.UserInputManager();
            InitReceiveItems();
            Registerhandler();
        }

        private void Print(params object[] log)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var s in log)
            {
                sb.AppendLine(s.ToString());
            } 
        }

        private void Registerhandler()
        {
            foreach (var item in _receiveItemList)
            {
                var log = item.Log;
                _manager.RegisterKeyhandler(new KeyHandler(item.Layer, item.Block).BindKeyAction(item.Key, (data) =>
                {
                    Console.Write(log);
                }));
            }
        }

        private void InitReceiveItems()
        {
            _receiveItemList.Add(new ReceiveItem((int)Layer.Ui, BlockType.None, UserInputKey.Switch1, "Uihandler"));
            _receiveItemList.Add(new ReceiveItem((int)Layer.Env, BlockType.None, UserInputKey.Switch1, "Envhandler"));
            _receiveItemList.Add(new ReceiveItem((int)Layer.System, BlockType.None, UserInputKey.Switch1, "Systemhandler"));
            _receiveItemList.Add(new ReceiveItem((int)Layer.Top, BlockType.None, UserInputKey.Switch1, "Tophandler"));
        }
    }
}
