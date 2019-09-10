using System;
using System.Collections.Generic;
using App.Client.Utility;
using App.Shared.Components.UserInput;
using Entitas;
using Loxodon.Framework.Binding;
using UnityEngine;
using UserInputManager.Lib;

namespace App.Client.ClientSystems
{
    public  class MouseLockSystem : IInitializeSystem
    {
        Contexts _contexts;
        public MouseLockSystem(Contexts contexts)
        {
            _contexts = contexts;
            InitKeyhandler(contexts.userInput);
        }

        private void InitKeyhandler(UserInputContext context)
        {
            var keyHandler = new KeyHandler(Layer.Env + 2, BlockType.None);
            keyHandler.BindKeyAction(UserInputKey.LockCursor, (data) =>
            {
                //暂时屏蔽               
                _contexts.ui.uI.IsShowCrossHair = true;
            });
            keyHandler.BindKeyAction(UserInputKey.UnlockCursor, (data) =>
            {
                //暂时屏蔽                
                _contexts.ui.uI.IsShowCrossHair = false;
            });
            context.userInputManager.Instance.RegisterKeyhandler(keyHandler);
        }

        public void Initialize()
        {

        }
    }
    
}
