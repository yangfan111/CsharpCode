using App.Client.GameModules.Ui;
using App.Client.Utility;
using Assets.Sources.Free;
using Assets.Sources.Free.UI;
using Core.Free;
using Free.framework;
using UserInputManager.Lib;
using Utils.Singleton;

namespace App.Client.GameModules.GamePlay.Free.UI
{
    public class UILockMouseHandler : ISimpleMesssageHandler
    {
        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.LockMouse;
        }

        public void Handle(SimpleProto simpleProto)
        {
            bool isLock = simpleProto.Bs[0];
            if (isLock)
            {
                CursorLocker.SystemUnlock = false;
                //Lock
                SingletonManager.Get<FreeUiManager>().Contexts1.ui.uI.IsShowCrossHair = true;

                SingletonManager.Get<FreeUiManager>().Contexts1.userInput.userInputManager.Helper.UnblockKey(CursorLocker.SystemBlockKeyId);
                SingletonManager.Get<FreeUiManager>().Contexts1.userInput.userInputManager.Helper.UnblockPointer(CursorLocker.SystemBlockPointerId);
            }
            else
            {
                UiModule.CloseGameSettingUI();
                //chat
                SingletonManager.Get<FreeUiManager>().Contexts1.ui.chat.CloseChatView();
                //bag
                CursorLocker.SystemUnlock = true;
                if (SingletonManager.Get<FreeUiManager>().GetUi("bagSystemUI").Visible)
                {
                    SingletonManager.Get<FreeUiManager>().GetUi("bagSystemUI").Show(-1);
                }
                else
                {
                    //Unlock
                    SingletonManager.Get<FreeUiManager>().Contexts1.ui.uI.IsShowCrossHair = false;
                    CursorLocker.SystemBlockKeyId = SingletonManager.Get<FreeUiManager>().Contexts1.userInput.userInputManager.Helper.BlockKey(Layer.System);
                    CursorLocker.SystemBlockPointerId = SingletonManager.Get<FreeUiManager>().Contexts1.userInput.userInputManager.Helper.BlockPointer(Layer.System);
                }
            }
        }
    }
}
