using System;
using App.Client.GameModules.Ui.ViewModels.Common;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using UserInputManager.Lib;
using Core.Utils;
using UnityEngine;
using App.Client.GameModules.Ui.Logic;
using Utils.Configuration;
using App.Client.CastObjectUtil;
using App.Client.GameModules.Ui.UiAdapter;
using UnityEngine.UI;
using Utils.Singleton;

namespace App.Client.GameModules.Ui.Models.Common
{
    /// <summary>
    /// 拾取和cmd指令以及输入指令有关，需要保持帧率一致，所以使用Playbacksystem
    /// </summary>
    public class CommonPickUpModel : ClientAbstractModel, IUiHfrSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CommonPickUpModel));
        private CommonPickUpViewModel _viewModel = new CommonPickUpViewModel();
        private PointerData _castData;
        private float _lastCastTime;
        private SceneObjectCastLogic _sceneObjectCastLogic;
        private MapObjectCastLogic _mapObjectCastLogic;
        private VehicleCastLogic _vehicleCastLogic;
        private FreeObjectCastLogic _freeCastLogic;
        private DoorCastLogic _doorCastLogic;
        private PlayerCastLogic _playerCastLogic;
        private CommonCastLogic _commonCastLogic;
        private ICastLogic _currentCastLogic;
        private IPlayerStatTipLogic _playerStateTipLogic;
        //准星的状态会影响F提示的显示
        private IPickUpUiAdapter _pickUpUiAdapter;
        // Buff类型通用提示
        private BuffTipLogic _buffTipLogic;

        /// <summary>
        /// 因为碰撞检测和按钮点击的时序不固定（当前固定，但是可能会因为修改而导致变化），所以通过回调保证在同一帧时序不同的时候结果都正确
        /// </summary>
        private Action _doCastAction;
        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }

        public CommonPickUpModel(IPickUpUiAdapter pickUpUiAdapter):base(pickUpUiAdapter)
        {
            _pickUpUiAdapter = pickUpUiAdapter;
            RegisterKeyBinding();

            _currentCastLogic = null;
            _sceneObjectCastLogic = _pickUpUiAdapter.GetSceneObjectCastLogic();
            _mapObjectCastLogic = _pickUpUiAdapter.GetMapObjectCastLogic();
            _vehicleCastLogic = _pickUpUiAdapter.GetVehicleCastLogic();
            _freeCastLogic = _pickUpUiAdapter.GetFreeObjectCastLogic();
            _doorCastLogic = _pickUpUiAdapter.GetDoorCastLogic();
            _playerCastLogic = _pickUpUiAdapter.GetPlayerCastLogic();
            _playerStateTipLogic = _pickUpUiAdapter.GetPlayerStateTipLogic();
            _buffTipLogic = _pickUpUiAdapter.GetBuffTipLogic();
            _commonCastLogic = _pickUpUiAdapter.GetCommonCastLogic();
        }

        private void RegisterKeyBinding()
        {
           // 虽然是Ui但是其实层级应该是Env
            var keyreciever = new KeyHandler(Layer.Env, BlockType.None);
            keyreciever.BindKeyAction(UserInputKey.PickUp, OnAction);
            _pickUpUiAdapter.RegisterKeyhandler(keyreciever);

            var pointerhandler = new PointerKeyHandler(Layer.Env, BlockType.None);
            pointerhandler.BindPointAction(UserInputKey.PickUpTip, SetCastData);
            _pickUpUiAdapter.RegisterPointerhandler(pointerhandler);
        }

        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            InitVariable();
        }

        private Text _itemNameText;
        private RectTransform _itemNameBgRtf;
        private Vector2 _origNameBgSize;
        private Vector2 _origNameSize;
        private float offset = 12.5f;
        private void InitVariable()
        {
            _itemNameText = FindComponent<Text>("ItemName");
            _itemNameBgRtf = FindComponent<RectTransform>("ItemNameBg");
            if (_itemNameBgRtf != null && _itemNameText != null)
            {
                _origNameBgSize = _itemNameBgRtf.sizeDelta;
                _origNameSize = _itemNameText.rectTransform.sizeDelta;
            }
        }

        private void ResizeText()
        {
            if (_itemNameText == null || _itemNameBgRtf == null) return;
            var newSize = _itemNameText.preferredWidth;
            if (newSize > _origNameSize.x - offset)
            {
                _itemNameText.rectTransform.sizeDelta = new Vector2(newSize, _origNameSize.y);
                _itemNameBgRtf.sizeDelta =
                    new Vector2(_origNameBgSize.x + newSize - _origNameSize.x + offset, _origNameBgSize.y);
            }
            else
            {
                _itemNameBgRtf.sizeDelta = _origNameBgSize;
            }
        }

        public override void Update(float interval)
        {

            _viewModel.Show = false;

            ResetCastLogic();

            if (_buffTipLogic.HasTipState())
            {
                ShowBuffTip();
            }

            if(_playerStateTipLogic.HasTipState())
            {
                ShowStateTip();
            }
            else if (CheckCastData())
            {
                ShowCastTip();
            }

            _castData = null;
            _doCastAction = null;
            _currentCastLogic = null;
        }

        private void ResetCastLogic()
        {
            _commonCastLogic.Clear();
        }

        private bool CheckCastData()
        {
            if(_castData != null)
            {
                return true;
            }
            return false;
        }

        private void OnAction(KeyData data)
        {
            if(_playerStateTipLogic.HasTipState())
            {
                //倒计时出现时不能进行F键操作
                if (_pickUpUiAdapter.IsCountDown())
                {
                    return;
                }
                _playerStateTipLogic.Action();
            }
            else
            {
                _doCastAction = DoCastAction;
            }
        }

        private void DoCastAction()
        {
            if(Time.time - _lastCastTime < SingletonManager.Get<RaycastActionConfigManager>().Interval)
            {
                if(Logger.IsDebugEnabled)
                {
                    Logger.Debug("action in cd ");
                }
                return;
            }
            _lastCastTime = Time.time;
            if (null == _castData)
            {
                if(Logger.IsDebugEnabled)
                {
                    Logger.Debug("caset Data is null");
                }
                return;
            }
            if(null != _currentCastLogic)
            {
                _currentCastLogic.Action();
            }
            else
            {
                if(Logger.IsDebugEnabled)
                {
                    Logger.Debug("current cast logic is null");
                }
            }
            
            _castData = null;
            _currentCastLogic = null;
        }

        private void SetCastData(KeyData data)
        {
            _castData = data as PointerData;
        }

        private void ShowStateTip()
        {
            if(!string.IsNullOrEmpty(_playerStateTipLogic.StateTip))
            {
                //倒计时出现时不能显示Tip
                if (_pickUpUiAdapter.IsCountDown())
                {
                    return;
                }

                ShowText(_playerStateTipLogic.StateTip);

                if (Logger.IsDebugEnabled)
                {
                    Logger.DebugFormat("ShowStateTip {0}", _playerStateTipLogic.StateTip);
                }
            }
        }

        private void ShowText(string stateTip)
        {
            _viewModel.Show = true;
            _viewModel.ItemName = stateTip;
            ResizeText();
        }

        private void ShowBuffTip()
        {
            ShowText(_buffTipLogic.StateTip);
        }

        private void ShowCastTip()
        {
            _viewModel.Show = false;
            var pointerData = _castData;
            if (null == pointerData)
            {
                Logger.Error("no pointer data ");
                return;
            }
            if(null == pointerData.IdList)
            {
                Logger.Error("id list of pointer data  is null, raycast target may not been inited");
                return;
            }
            var type = pointerData.IdList[0];
            switch ((ECastDataType)type)
            {
                case ECastDataType.SceneObject:
                    _currentCastLogic = _sceneObjectCastLogic;
                    break;
                case ECastDataType.MapObject:
                    _currentCastLogic = _mapObjectCastLogic;
                    break;
                case ECastDataType.Vehicle:
                    _currentCastLogic = _vehicleCastLogic;
                    break;
                case ECastDataType.FreeObject:
                    _currentCastLogic = _freeCastLogic;
                    break;
                case ECastDataType.Door:
                    _currentCastLogic = _doorCastLogic;
                    break;
                case ECastDataType.Player:
                    _currentCastLogic = _playerCastLogic;
                    break;
                case ECastDataType.Common:
                    _currentCastLogic = _commonCastLogic;
                    break;
            }
            _currentCastLogic.SetData(pointerData);
            var tip = _currentCastLogic.Tip;

            //倒计时出现时不显示Tip，不能进行F键操作
            if (_pickUpUiAdapter.IsCountDown())
            {
                return;
            }

            if(string.IsNullOrEmpty(tip))
            {
                return;
            }
            else
            {
                ShowText(tip);
                if (null != _doCastAction)
                {
                    _doCastAction();
                    _doCastAction = null;
                }
            }
       }

    }
}