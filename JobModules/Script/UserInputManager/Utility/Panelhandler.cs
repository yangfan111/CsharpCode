using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UserInputManager.Lib;

namespace UserInputManager.Utility
{
    public class Panelhandler
    {
        public Panelhandler(Lib.GameInputManager manager, UserInputKey key, KeyPointAction onOpen, KeyPointAction onClose,
            bool blockEnv = true)
        {
            var openhandler = new KeyReceiver(EInputLayer.Ui, BlockType.None);
            var closehandler = new KeyReceiver(EInputLayer.Ui, BlockType.All);
            var openBlockhandler = blockEnv ? new PointerReceiver(EInputLayer.Ui, BlockType.All) : null;

            openhandler.BindKeyAction(key, (data) =>
            {
                manager.RegisterKeyReceiver(closehandler);
                manager.RegisterPointerReceiver(openBlockhandler);
                manager.UnregisterKeyReceiver(openhandler);
            });

            if (null != onOpen)
            {
                openhandler.BindKeyAction(key, onOpen);
            }

            closehandler.BindKeyAction(key, (data) =>
            {
                manager.RegisterKeyReceiver(openhandler);
                manager.UnregisterKeyReceiver(closehandler);
                manager.UnregisterPointerReceiver(openBlockhandler);
            });
            if (null != onClose)
            {
                closehandler.BindKeyAction(key, onClose);
            }

            manager.RegisterKeyReceiver(openhandler);
        }
    }

}
