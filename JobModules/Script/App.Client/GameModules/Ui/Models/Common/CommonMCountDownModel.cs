using App.Client.GameModules.Ui.ViewModels.Common;
using App.Client.GameModules.Ui.UiAdapter;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using UnityEngine;
using System;
using DG.Tweening;

namespace App.Client.GameModules.Ui.Models.Common
{
    public class CommonMCountDownModel : ClientAbstractModel, IUiHfrSystem
    {
        private ICountDownUiAdapter adapter;
        private CommonMCountDownViewModel _viewModel;
        private bool isCountingDown = false;
        private long duringTime = 0;
        private long curTime = 0;
        private long lastTime = 0;

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

        private bool isBackward;

        private bool haveCompleted
        {
            get { return adapter.HaveCompleted; }
            set { adapter.HaveCompleted = value; }
        }

        public override void Update(float interval)
        {

            if (RootActive() && duringTime > 0 && !isBackward)
            {
                var temperTime = DateTime.Now.Ticks / 10000 - lastTime;
                lastTime = DateTime.Now.Ticks / 10000;
                curTime = curTime - temperTime;
                if (curTime <= 0)
                {
                    adapter.ShowUiGroup(Core.Ui.UiGroup.TimeCountDownHide);
                    SetRootActive(false);
                    adapter.StartCountDown = false;
                    adapter.CountDownNum = 0;
                    duringTime = 0;
                    haveCompleted = true;
                }
                else
                {
                    adapter.HideUiGroup(Core.Ui.UiGroup.TimeCountDownHide);
                    _viewModel.numBgFillAmount = (float)curTime / duringTime;
                    _viewModel.countNumText = ((float)curTime / 1000).ToString("0.0");
                }
            }

            if (adapter.StartCountDown && adapter.CountDownNum != 0)
            {
                isBackward = false;
                haveCompleted = false;
                SetRootActive(true);
                duringTime = (long)adapter.CountDownNum * 1000;
                curTime = duringTime;
                lastTime = DateTime.Now.Ticks / 10000;
                adapter.CountDownNum = 0;
                _backwardAnime.Kill();
            }
            else if (adapter.StartCountDown == false)
            {
                if (RootActive())
                {
                    //SetCross(true);
                    if (!haveCompleted)
                    {
                        PlayBackwardAnim();
                    }
                    else
                    {
                        adapter.ShowUiGroup(Core.Ui.UiGroup.TimeCountDownHide);
                        SetRootActive(false);
                    }
                }
            }
        }


        private Tween _backwardAnime;

        private const float _backwardTime = 0.3f;
        private void PlayBackwardAnim()
        {
            if (isBackward)
            {
                return;
            }
            if (_backwardAnime != null) _backwardAnime.Kill();
            isBackward = true;
            _backwardAnime = DOTween.To(() => _viewModel.numBgFillAmount, (x) => _viewModel.numBgFillAmount = x , 1, _backwardTime);
            _backwardAnime.onComplete = OnCompleteAnime;

        }

        private void OnCompleteAnime()
        {
            //Debug.Log("Anime done");
            haveCompleted = true;
            SetRootActive(false);
            adapter.StartCountDown = false;
            adapter.CountDownNum = 0;
            duringTime = 0;
            adapter.ShowUiGroup(Core.Ui.UiGroup.TimeCountDownHide);
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

        public override void OnDestory()
        {
            base.OnDestory();
            if (_backwardAnime != null) _backwardAnime.Kill();
        }
    }
}
