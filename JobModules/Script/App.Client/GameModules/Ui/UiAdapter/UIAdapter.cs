using App.Shared.Components.Ui;
using App.Shared.Components.UserInput;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;
using Core.Ui;
using UserInputManager.Lib;

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


        public void RegisterKeyReceive(IKeyReceiver keyReceive)
        {
            UserInputManager.Instance.RegisterKeyReceiver(keyReceive);
        }

        public void UnRegisterKeyReceive(IKeyReceiver keyReceive)
        {
            UserInputManager.Instance.UnregisterKeyReceiver(keyReceive);
        }

        public void RegisterPointerReceive(IPointerReceiver pointReceive)
        {
            UserInputManager.Instance.RegisterPointerReceiver(pointReceive);
        }

        public void UnRegisterPointerReceive(IPointerReceiver pointReceive)
        {
            UserInputManager.Instance.UnregisterPointerReceiver(pointReceive);
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