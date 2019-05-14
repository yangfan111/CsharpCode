using System;
using System.Collections.Generic;
using App.Client.GameModules.Ui.ViewModels.Common;
using App.Client.GameModules.Ui.UiAdapter;
using App.Client.GameModules.Ui.Utils;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.Utils;
using UnityEngine;
using Utils.AssetManager;
using VehicleCommon;

namespace App.Client.GameModules.Ui.Models.Common
{
    public class CommonVehicleTipModel : ClientAbstractModel, IUiSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CommonVehicleTipModel));
        private CommonCarryTipViewModel _viewModel = new CommonCarryTipViewModel();
        //private Dictionary<AssetInfo, GameObject> _vehicleTopViewDict = new Dictionary<AssetInfo, GameObject>(AssetInfo.AssetInfoComparer.Instance);
        private Dictionary<AssetInfo, VehicleTipTopViewItem> _vehicleTopViewDict = new Dictionary<AssetInfo, VehicleTipTopViewItem>(AssetInfo.AssetInfoComparer.Instance);
        private AssetInfo _curCarryAssetInfo;
        private Transform _topViewRoot;

        private GameObject CurVehicleGo
        {
            get
            {
                return CurVehicle.Root;
            }
        }

        private VehicleTipTopViewItem CurVehicle
        {
            get
            {
                if (_vehicleTopViewDict.ContainsKey(_curCarryAssetInfo))
                {
                    return _vehicleTopViewDict[_curCarryAssetInfo];
                }
                else
                {
                    return new VehicleTipTopViewItem();
                }
            }
        }
        private IVehicleUiAdapter _adapter;

        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }

        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            _topViewRoot = FindChildGo("CarryTopViewGroup");
        }

        public CommonVehicleTipModel(IVehicleUiAdapter adapter):base(adapter)
        {
            _adapter = adapter;
        }

       public override void Update(float interval)
        {

            UpdateCurVehicleTopView();

            UpdateSeats();
            UpdateWheels();
            UpdateHp();
            UpdateOil();
            UpdateSpeed();

        }

        private void UpdateWheels()
        {
            var wheelIndexArray = VehicleIndexHelper.GetWheelIndexArray();
            foreach(var index in wheelIndexArray)
            {
                UpdateWheelStateByIndex(index);
            }
        }

        /// <summary>
        /// 刷新载具上对应位置轮胎爆胎情况
        /// </summary>
        /// <param name="index"></param>
        private void UpdateWheelStateByIndex(VehiclePartIndex index)
        {
            VehicleUiWheelIndex uiIndex;
            bool isWheelBroke = _adapter.IsWheelBrokeByIndex(index, out uiIndex);
            SetWheelShow(uiIndex, isWheelBroke);
        }

        private void SetWheelShow(VehicleUiWheelIndex uiIndex, bool isWheelBroke)
        {
            Transform wheel = null;
            wheel = CurVehicle.GetWheel(uiIndex);
            if (wheel != null)
            {
                UIUtils.SetActive(wheel, isWheelBroke);
            }
        }


        private void UpdateSeats()
        {
            var seatIndexArray = VehicleIndexHelper.GetSeatIndexArray();
            foreach(var index in seatIndexArray)
            {
                UpdateSeatStateByIndex(index);
            }
        }

        /// <summary>
        /// 刷新载具对应位置上所坐队友情况
        /// </summary>
        /// <param name="index"></param>
        private void UpdateSeatStateByIndex(VehicleSeatIndex index)
        {
            VehicleSeatIndex uiIndex;
            var isOccupied = _adapter.IsSeatOccupied(index, out uiIndex);
            SetSeatState(uiIndex,isOccupied);
        }

        private void SetSeatState(VehicleSeatIndex uiIndex, bool isOccupied)
        {
            Transform seat = null;
            seat = CurVehicle.GetSeat(uiIndex);
            if (seat != null)
            {
                UIUtils.SetActive(seat, isOccupied);
            }
        }

        /// <summary>
        /// 刷新当前所坐载具缩略图
        /// </summary>
        private void UpdateCurVehicleTopView()
        {
            AssetInfo assetInfo = _adapter.CurVehicleAssetInfo;
            if (_curCarryAssetInfo == assetInfo)
            {
                return;
            }

            CurVehicleGo.SetActive(false);
            _curCarryAssetInfo = assetInfo;
            if (_vehicleTopViewDict.ContainsKey(assetInfo))
            {
                CurVehicleGo.SetActive(true);
            }
            else
            {
                _vehicleTopViewDict.Add(assetInfo, new VehicleTipTopViewItem());
                Loader.LoadAsync(assetInfo.BundleName, assetInfo.AssetName,
                    (obj) =>
                    {
                        GameObject go = obj as GameObject;
                        _vehicleTopViewDict[assetInfo] = new VehicleTipTopViewItem(go);
                        go.transform.SetParent(_topViewRoot,false);
                        go.SetActive(true);
                    });
            }


        }
        /// <summary>
        /// 刷新载具速度
        /// </summary>
        private void UpdateSpeed()
        {
	        var speed = _adapter.CurVehicleSpeed;

			_viewModel.SpeedString = ((int)(speed + 0.5f)).ToString();
        }

        /// <summary>
        /// 刷新载具油量条
        /// </summary>
        private void UpdateOil()
        {
            var curOil = _adapter.CurVehicleOil;
            var maxOil = _adapter.MaxVehicleOil;
            var oilBarVal = curOil / maxOil;
            _viewModel.OilBarValue = oilBarVal;
        }

        /// <summary>
        /// 刷新载具血条
        /// </summary>
        private void UpdateHp()
        {

            var curHp = _adapter.CurVehicleHp;
            var maxHp = _adapter.MaxVehicleHp;
            var hpBarVal = curHp / maxHp;
            _viewModel.HpBarValue = hpBarVal;
			_viewModel.HpFillColor = hpBarVal < 0.3f ? _lowColor : _highColor;

        }

        private readonly Color _lowColor = UiCommonColor.HpLowColor;
        private readonly Color _highColor = UiCommonColor.HpHighColor;

    }

    public class VehicleTipTopViewItem
    {
        public GameObject Root;
        public Dictionary<VehicleUiWheelIndex, Transform> Wheels = new Dictionary<VehicleUiWheelIndex, Transform>();
        public Dictionary<VehicleSeatIndex, Transform> Seats = new Dictionary<VehicleSeatIndex, Transform>();


        private readonly Dictionary<VehicleUiWheelIndex, string> wheelUiNames = new Dictionary<VehicleUiWheelIndex, string>
        {
            {VehicleUiWheelIndex.FrontLeft, "TyreFL"},
            {VehicleUiWheelIndex.FrontRight, "TyreFR"},
            {VehicleUiWheelIndex.RearLeft, "TyreRL"},
            {VehicleUiWheelIndex.RearRight, "TyreRR"},
        };
    
        private readonly Dictionary<VehicleSeatIndex,string> seatNames = new Dictionary<VehicleSeatIndex, string>
        {
            { VehicleSeatIndex.Driver,"SeatDriver"},
            { VehicleSeatIndex.Codriver,"SeatCodriver"},
            { VehicleSeatIndex.BackDriver,"SeatBackDriver"},
            { VehicleSeatIndex.BackCodriver,"SeatBackCodriver"},
            { VehicleSeatIndex.BackDriver_1,"SeatBackDriver_1"},
            { VehicleSeatIndex.BackCodriver_1,"SeatBackCodriver_1"},
        };

        private string rootName = "Group";
        public VehicleTipTopViewItem(GameObject orig)
        {
            Root = orig;
            Transform origTf = orig.transform.Find(rootName);
            InitWheels(origTf);
            InitSeats(origTf);
        }

        private void InitSeats(Transform origTf)
        {
            if (origTf == null) return;
            foreach (var pair in seatNames)
            {
                Seats.Add(pair.Key, origTf.Find(pair.Value));
            }
        }

        private void InitWheels(Transform origTf)
        {
            if (origTf == null) return;
            foreach(var pair in wheelUiNames)
            {
                Wheels.Add(pair.Key, origTf.Find(pair.Value));
            }
        }

        public VehicleTipTopViewItem()
        {
            Root = new GameObject();
        }

        public Transform GetWheel(VehicleUiWheelIndex index)
        {
            Transform ret = null;
            Wheels.TryGetValue(index, out ret);
            return ret;
        }

        public Transform GetSeat(VehicleSeatIndex index)
        {
            Transform ret = null;
            Seats.TryGetValue(index, out ret);
            return ret;
        }
    }
}
