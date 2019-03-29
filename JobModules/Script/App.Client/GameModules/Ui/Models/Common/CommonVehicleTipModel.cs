using System;
using System.Collections.Generic;
using App.Client.GameModules.Ui.ViewModels.Common;
using App.Client.GameModules.Ui.UiAdapter;
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
        private Dictionary<AssetInfo, GameObject> _vehicleTopViewDict = new Dictionary<AssetInfo, GameObject>(AssetInfo.AssetInfoComparer.Instance);
        private AssetInfo _curCarryAssetInfo;
        private Transform _topViewRoot;

        private GameObject CurVehicleGo
        {
            get
            {
                if (_vehicleTopViewDict.ContainsKey(_curCarryAssetInfo))
                {
                    return _vehicleTopViewDict[_curCarryAssetInfo];
                }
                else
                {
                    return new GameObject();
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

            UpdateCurCarryTopView();

            UpdateCarrySeats();
            UpdateCarryTyres();
            UpdateCarryHp();
            UpdateCarryOil();
            UpdateCarrySpeed();

        }

        private void UpdateCarryTyres()
        {
            var wheelIndexArray = VehicleIndexHelper.GetWheelIndexArray();
            foreach(var index in wheelIndexArray)
            {
                UpdateTyreStateByIndex(index);
            }
        }

        /// <summary>
        /// 刷新载具上对应位置轮胎爆胎情况
        /// </summary>
        /// <param name="index"></param>
        private void UpdateTyreStateByIndex(VehiclePartIndex index)
        {
            VehicleUiWheelIndex uiIndex;
            bool isTyreBroke = _adapter.IsTyreBrokeByIndex(index, out uiIndex);
            SetTyreShow(uiIndex, isTyreBroke);
        }

        private void SetTyreShow(VehicleUiWheelIndex uiIndex, bool isTyreBroke)
        {
            //Debug.Log("uiIndex:" + uiIndex);
            //Debug.Log("isTyreBroke:" + isTyreBroke);
            var groupTf = CurVehicleGo.transform.Find("Group");
            if (groupTf == null)
            {
                return;
            }
            Transform tyreTf = null;
            switch (uiIndex)
            {
                case VehicleUiWheelIndex.FrontLeft:
                    tyreTf = groupTf.Find("TyreFL");
                    break;
                case VehicleUiWheelIndex.FrontRight:
                    tyreTf = groupTf.Find("TyreFR");
                    break;
                case VehicleUiWheelIndex.RearLeft:
                    tyreTf = groupTf.Find("TyreRL");
                    break;
                case VehicleUiWheelIndex.RearRight:
                    tyreTf = groupTf.Find("TyreRR");
                    break;
            }

            if (tyreTf == null)
            {
                return;
            }

            tyreTf.gameObject.SetActive(isTyreBroke);
        }


        private void UpdateCarrySeats()
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
            //Debug.Log("uiIndex:" + uiIndex);
            //Debug.Log("isOccupied:" + isOccupied);
            var groupTf = CurVehicleGo.transform.Find("Group");
            if (groupTf == null)
            {
                return;
            }
            Transform seatTf = null;
            switch (uiIndex)
            {
                case VehicleSeatIndex.Driver:
                    seatTf = groupTf.Find("SeatDriver");
                    break;
                case VehicleSeatIndex.Codriver:
                    seatTf = groupTf.Find("SeatCodriver");
                    break;
                case VehicleSeatIndex.BackDriver:
                    seatTf = groupTf.Find("SeatBackDriver");
                    break;
                case VehicleSeatIndex.BackCodriver:
                    seatTf = groupTf.Find("SeatBackCodriver");
                    break;
                case VehicleSeatIndex.BackDriver_1:
                    seatTf = groupTf.Find("SeatBackDriver_1");
                    break;
                case VehicleSeatIndex.BackCodriver_1:
                    seatTf = groupTf.Find("SeatBackCodriver_1");
                    break;
            }

            if (seatTf == null)
            {
                return;
            }
            seatTf.gameObject.SetActive(isOccupied);
        }

        /// <summary>
        /// 刷新当前所坐载具缩略图
        /// </summary>
        private void UpdateCurCarryTopView()
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
                _vehicleTopViewDict.Add(assetInfo, new GameObject());
                Loader.LoadAsync(assetInfo.BundleName, assetInfo.AssetName,
                    (obj) =>
                    {
                        GameObject go = obj as GameObject;
                        _vehicleTopViewDict[assetInfo] = go;
                        go.transform.SetParent(_topViewRoot,false);
                        go.SetActive(true);
                    });
            }


        }
        /// <summary>
        /// 刷新载具速度
        /// </summary>
        private void UpdateCarrySpeed()
        {
	        var speed = _adapter.CurVehicleSpeed;

			_viewModel.SpeedString = ((int)(speed + 0.5f)).ToString();
        }

        /// <summary>
        /// 刷新载具油量条
        /// </summary>
        private void UpdateCarryOil()
        {
            var curOil = _adapter.CurVehicleOil;
            var maxOil = _adapter.MaxVehicleOil;
            var oilBarVal = curOil / maxOil;
            _viewModel.OilBarValue = oilBarVal;
        }

        /// <summary>
        /// 刷新载具血条
        /// </summary>
        private void UpdateCarryHp()
        {

            var curHp = _adapter.CurVehicleHp;
            var maxHp = _adapter.MaxVehicleHp;
            var hpBarVal = curHp / maxHp;
            _viewModel.HpBarValue = hpBarVal;
			if (hpBarVal < 0.3f)
            {
                _viewModel.HpFillColor = new Color(237f / 255f, 129f / 255f, 129f / 255f, 1.0f);
            }
            else
			{
				_viewModel.HpFillColor = new Color(247f / 255f, 238f / 255f, 201f / 255f, 1.0f);
			}

        }

       
    }
}
