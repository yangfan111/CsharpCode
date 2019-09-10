using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UserInputManager.Lib;

namespace UserInputManager.Utility
{
    public class Panelhandler
    {
        public Panelhandler(Lib.UserInputManager manager, UserInputKey key, KeyPointAction onOpen, KeyPointAction onClose,
            bool blockEnv = true)
        {
            var openhandler = new KeyHandler(Layer.Ui, BlockType.None);
            var closehandler = new KeyHandler(Layer.Ui, BlockType.All);
            var openBlockhandler = blockEnv ? new PointerKeyHandler(Layer.Ui, BlockType.All) : null;

            openhandler.BindKeyAction(key, (data) =>
            {
                manager.RegisterKeyhandler(closehandler);
                manager.RegisterPointerhandler(openBlockhandler);
                manager.UnregisterKeyhandler(openhandler);
            });

            if (null != onOpen)
            {
                openhandler.BindKeyAction(key, onOpen);
            }

            closehandler.BindKeyAction(key, (data) =>
            {
                manager.RegisterKeyhandler(openhandler);
                manager.UnregisterKeyhandler(closehandler);
                manager.UnregisterPointerhandler(openBlockhandler);
            });
            if (null != onClose)
            {
                closehandler.BindKeyAction(key, onClose);
            }

            manager.RegisterKeyhandler(openhandler);
        }
    }

}
