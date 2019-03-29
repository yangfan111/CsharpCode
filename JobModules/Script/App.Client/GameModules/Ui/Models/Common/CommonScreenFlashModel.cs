using System;
using App.Client.GameModules.Ui.ViewModels.Common;
using App.Client.GameModules.Ui.UiAdapter;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using UnityEngine;

namespace App.Client.GameModules.Ui.Models.Common
{
    struct FlashAttr
    {
        public float Alpha;
        public float KeepTime;
        public float DecayTime;

        public void Reset()
        {
            Alpha = 0;
            KeepTime = 0;
            DecayTime = 0;
        }
    }

    public class CommonScreenFlashModel : ClientAbstractModel, IUiSystem
    {
        private Contexts _contexts;
        private IScreenFlashUiAdapter _state;
        private CommonScreenFlashViewModel _viewModel;

        private FlashAttr _setAttr;
        private FlashAttr _curAttr;
        private DateTime _lastUpdateTime;

        public CommonScreenFlashModel(IScreenFlashUiAdapter state):base(state)
        {
            _state = state;
            _viewModel = new CommonScreenFlashViewModel();
            _setAttr = new FlashAttr();
            _curAttr = new FlashAttr();
        }

        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }

        public override void Update(float interval)
        {
            if (!isVisible) return;
            if(!_state.IsShow)
            {
                if (_viewModel.Show)
                {
                    _state.Clear();
                    _curAttr.Reset();
                    _setAttr.Reset();
                    _viewModel.Show = false;
                }
                return;
            }
            if (_state.IsDirty)
            {
                if (_curAttr.KeepTime + _curAttr.DecayTime <= 0)
                {
                    _setAttr.Reset();
                    _curAttr.Reset();
                }
                _setAttr.Alpha = Mathf.Min(1, _setAttr.Alpha + _state.Alpha);
                _setAttr.KeepTime += _state.KeepTime;
                _setAttr.DecayTime += _state.DecayTime;

                _curAttr.Alpha = Mathf.Min(1, _curAttr.Alpha + _state.Alpha);
                _curAttr.KeepTime += _state.KeepTime;
                _curAttr.DecayTime += _state.DecayTime;

                _lastUpdateTime = DateTime.Now;
                _state.Clear();
            }

            if (_curAttr.KeepTime > 0)
            {
                KeepScreenFlash();
                _viewModel.Show = true;
            }
            else if (_curAttr.DecayTime > 0)
            {
                DecayScreenFlash();
                _viewModel.Show = true;
            }
            else
            {
                _viewModel.Show = false;
            }
        }

        private void KeepScreenFlash()
        {
            _viewModel.FlashColor = new Color(255, 255, 255, _curAttr.Alpha);
            _curAttr.KeepTime = Mathf.Max(0, _curAttr.KeepTime - (float)(DateTime.Now - _lastUpdateTime).TotalSeconds);
            _lastUpdateTime = DateTime.Now;
        }

        private void DecayScreenFlash()
        {
            _curAttr.DecayTime = Mathf.Max(0, _curAttr.DecayTime - (float)(DateTime.Now - _lastUpdateTime).TotalSeconds);
            _curAttr.Alpha = _setAttr.Alpha * (_curAttr.DecayTime/_setAttr.DecayTime);
            _viewModel.FlashColor = new Color(255, 255, 255, _curAttr.Alpha);
            _lastUpdateTime = DateTime.Now;
        }
    }
}
