using System;
using App.Client.GameModules.Ui.UiAdapter.Interface.Common;
using App.Client.GameModules.Ui.ViewModels.Common;
using App.Shared.Components.Ui;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using Core.Utils;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace App.Client.GameModules.Ui.Models.Common
{

    public class CommonOperationTipModel : ClientAbstractModel, IUiHfrSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CommonOperationTipModel));
        private CommonSimpleTipViewModel _viewModel = new CommonSimpleTipViewModel();
        private IOperationTipUiAdapter _adapter;

        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }

        public CommonOperationTipModel(IOperationTipUiAdapter adapter) : base(adapter)
        {
            _adapter = adapter;
            _adapter.Enable = false;
        }

        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            InitVariable();
            InitGui();
            InitAnim();
        }

        private void InitAnim()
        {
            _oldTextMoveAnim = DOTween.To(() => _moveTextTf.anchoredPosition, (x) => _moveTextTf.anchoredPosition = x,
                _targetTextPos, _moveTime);

            _oldTextAlphaAnim = DOTween.To(() => _moveTextCanvasGroup.alpha, (x) => _moveTextCanvasGroup.alpha = x,
                0, _moveTime);

            _oldTextMoveAnimSequence = DOTween.Sequence();
            _oldTextMoveAnimSequence.Insert(0, _oldTextMoveAnim);
            _oldTextMoveAnimSequence.Insert(0, _oldTextAlphaAnim);
            _oldTextMoveAnimSequence.SetAutoKill(false);
            _oldTextMoveAnimSequence.Pause();
            _oldTextMoveAnimSequence.OnRewind(() =>
            {
                if (_moveTextTf != null)
                {
                    _moveTextTf.gameObject.SetActive(true);
                }
            });
            _oldTextMoveAnimSequence.OnComplete(() =>
            {
                if (_moveTextTf != null)
                {
                    _moveTextTf.gameObject.SetActive(false);
                }
            });

        }

        private void InitGui()
        {

        }

        private RectTransform _moveTextTf;
        private Vector2 _origTextPos;
        private float _moveDistance;
        private Vector2 _targetTextPos;
        private float _moveTime = 0.3f;
        private Tween _oldTextMoveAnim,_oldTextAlphaAnim;
        private Sequence _oldTextMoveAnimSequence;
        private Text _moveText;
        private CanvasGroup _moveTextCanvasGroup;

        private void InitVariable()
        {
            var origTextTf = FindChildGo("Content").GetComponent<RectTransform>();
            _moveTextTf = Transform.Instantiate(origTextTf, origTextTf.parent);
            _moveTextCanvasGroup = _moveTextTf.gameObject.AddComponent<CanvasGroup>();
            _moveTextCanvasGroup.alpha = 1;
            _moveTextTf.gameObject.SetActive(false);
            _moveDistance = (_moveTextTf.parent as RectTransform).sizeDelta.y;
            _origTextPos = (_moveTextTf).anchoredPosition;
            _moveText = _moveTextTf.GetComponent<Text>();
            _targetTextPos = _origTextPos + new Vector2(0, _moveDistance);
        }


        public override void Update(float intervalTime)
        {
            UpdateTip();
            UpdateRemainTime();
            UpdateCurShowTip();
        }

        private void UpdateCurShowTip()
        {
            var tip = _adapter.OperationTipData;

            if (null == tip)
            {
                _adapter.Enable = false;
                return;
            }

            _viewModel.Content = tip.Title;
         
        }

        private void UpdateRemainTime()
        {
            var tip = _adapter.OperationTipData;
            var curTime = DateTime.Now.Ticks / 10000;
            if ((curTime - _createTime) > tip.DurationTime)
            {
                _adapter.OperationTipData = null;
            }
        }

        private void UpdateTip()
        {
            var tip = _adapter.OperationTipData;
            if (tip == _curTipData)
            {
                return;
            }

            MoveOldTip();
            SetNewTip(tip);
        }

        private void SetNewTip(ITipData tip)
        {
            _curTipData = tip;
            var curTime = DateTime.Now.Ticks / 10000;
            _createTime = curTime;
        }

        private void MoveOldTip()
        {
            if (_moveText != null && _oldTextMoveAnim != null && _curTipData != null)
            {
                _moveText.text = _curTipData.Title;
                _oldTextMoveAnimSequence.Restart();
            }
        }

        private ITipData _curTipData;
        private long _createTime;

        protected override void OnCanvasEnabledUpdate(bool enable)
        {
            base.OnCanvasEnabledUpdate(enable);
            if (!enable)
            {
                _curTipData = null;
                if (_oldTextMoveAnimSequence != null)
                    _oldTextMoveAnimSequence.Complete();
            }
        }

        public override void OnDestory()
        {
            base.OnDestory();
            if (_oldTextMoveAnimSequence != null)
                _oldTextMoveAnimSequence.Kill();
        }
    }
}

    
