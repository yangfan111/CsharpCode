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
            InitKeyReceiver(contexts.userInput);
        }

        private void InitKeyReceiver(UserInputContext context)
        {
            var recevier = new KeyReceiver(Layer.Env + 2, BlockType.None);
            recevier.AddAction(UserInputKey.LockCursor, (data) =>
            {
                //暂时屏蔽               
                _contexts.ui.uI.IsShowCrossHair = true;
            });
            recevier.AddAction(UserInputKey.UnlockCursor, (data) =>
            {
                //暂时屏蔽                
                _contexts.ui.uI.IsShowCrossHair = false;
            });
            context.userInputManager.Instance.RegisterKeyReceiver(recevier);
        }

        public void Initialize()
        {

        }
    }
    
}
