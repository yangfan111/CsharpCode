using App.Shared.Components.Ui;
using App.Shared.Components.UserInput;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;
using Core.Ui;
using UserInputManager.Lib;
using NotImplementedException = System.NotImplementedException;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public class UIAdapter : IAbstractUiAdapter
    {
        private UISessionComponent _uiSessionComponent;
        private UserInputManagerComponent _userInputManager;

        protected bool _enable = true;
        public virtual bool Enable
        {
            get { return _enable; }

            set { _enable = value; }
        }

        

        public virtual bool IsReady()
        {
            return true;
        }

        public void HideUiGroup(UiGroup group)
        {
            if (UiSessionComponent != null && UiSessionComponent.HideGroup.Contains(group) == false)
            {
                UiSessionComponent.HideGroup.Add(group);
            }
        }
        public void ShowUiGroup(UiGroup group)
        {
            if (UiSessionComponent != null && UiSessionComponent.HideGroup.Contains(group))
            {
                UiSessionComponent.HideGroup.Remove(group);
            }
        }
        private bool _canOpenUiByKey = true;

        public bool CanOpenUiByKey
        {
            get
            {
                return _canOpenUiByKey;
            }
            set
            {
                if (_canOpenUiByKey == value) return;
                if (value)
                {
                    foreach (var it in UiSessionComponent.OpenUiKeyReceiverList)
                    {
                        RegisterKeyReceive(it as KeyReceiver);
                    }
                }
                else
                {
                    foreach (var it in UiSessionComponent.OpenUiKeyReceiverList)
                    {
                        UnRegisterKeyReceive(it as KeyReceiver);
                    }
                }

                _canOpenUiByKey = value;
            }
        }


        public void RegisterKeyReceive(KeyReceiver keyReceive)
        {
            UserInputManager.Mgr.RegisterKeyReceiver(keyReceive);
        }

        public void UnRegisterKeyReceive(KeyReceiver keyReceive)
        {
            UserInputManager.Mgr.UnregisterKeyReceiver(keyReceive);
        }

        public void RegisterPointerReceive(PointerReceiver pointReceive)
        {
            UserInputManager.Mgr.RegisterPointerReceiver(pointReceive);
        }

        public void UnRegisterPointerReceive(PointerReceiver pointReceive)
        {
            UserInputManager.Mgr.UnregisterPointerReceiver(pointReceive);
        }

 
        public void RegisterOpenKey(KeyReceiver keyReceiver)
        {
            if (CanOpenUiByKey)
            {
                RegisterKeyReceive(keyReceiver );
            }
            UiSessionComponent.OpenUiKeyReceiverList.Add(keyReceiver);
        }

        public UISessionComponent UiSessionComponent
        {
            get
            {
                return _uiSessionComponent;
            }

            set
            {
                _uiSessionComponent = value;
            }
        }

        public UserInputManagerComponent UserInputManager
        {
            get
            {
                return _userInputManager;
            }

            set
            {
                _userInputManager = value;
            }
        }
    }
}