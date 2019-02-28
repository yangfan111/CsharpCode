using App.Client.GameModules.Ui.ViewModels.Common;
using App.Client.GameModules.Ui.UiAdapter;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using UnityEngine;

namespace App.Client.GameModules.Ui.Models.Common
{
    public class CommonMCountDownModel : ClientAbstractModel, IUiSystem
    {
        private ICountDownUiAdapter adapter;
        private CommonMCountDownViewModel _viewModel;
        private bool isCountingDown = false;
        private float duringTime = 0;
        private float curTime = 0;
        private float lastTime = 0;

        public CommonMCountDownModel(ICountDownUiAdapter adapter):base(adapter)
        {
            this.adapter = adapter;
            _viewModel = new CommonMCountDownViewModel();
        }

        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            SetRootActive(false);
        }

        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }

        public override void Update(float interval)
        {
            if (!isVisible) return;

            if (adapter.StartCountDown && adapter.CountDownNum != 0)
            {
                SetRootActive(true);
                duringTime = adapter.CountDownNum;
                curTime = duringTime;
                lastTime = Time.time;
                adapter.CountDownNum = 0;
            }
            else if(adapter.StartCountDown == false)
            {
                if(RootActive())
                {
                    //SetCross(true);
                    adapter.ShowUiGroup(Core.Ui.UiGroup.TimeCountDownHide);
                    SetRootActive(false);
                }
            }

            if (RootActive() && duringTime > 0)
            {
                var temperTime = Time.time - lastTime;
                lastTime = Time.time;
                curTime = curTime - temperTime;
                if (curTime <= 0)
                {
                    adapter.ShowUiGroup(Core.Ui.UiGroup.TimeCountDownHide);
                    SetRootActive(false);
                    adapter.StartCountDown = false;
                }
                else
                {
                    adapter.HideUiGroup(Core.Ui.UiGroup.TimeCountDownHide);
                    _viewModel.numBgFillAmount = curTime / duringTime;
                    _viewModel.countNumText = curTime.ToString("0.#");
                }
            } 
        }

        private void SetCross(bool isActive)
        {
            if(isActive !=  adapter.CrossActiveStatue())
                adapter.SetCrossActive(isActive);
        }

        private void SetRootActive(bool isActive)
        {
            if (_viewModel.rootActive != isActive)
                _viewModel.rootActive = isActive;
        }

        private bool RootActive()
        {
            return _viewModel.rootActive;
        }
    }
}
