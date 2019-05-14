using App.Shared.Components.Player;
using App.Shared.GameModules.Player;
using App.Shared.GameModules.Vehicle;
using App.Shared.GameModules.Vehicle.WheelCarrier;
using Assets.App.Client.GameModules.Ui;
using Utils.AssetManager;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.Utils;
using VehicleCommon;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public class VehicleTipUiAdapter : UIAdapter, IVehicleUiAdapter
    {
        private PlayerContext _playerContext;
        private VehicleContext _vehicleContext;

        public VehicleTipUiAdapter(Contexts contexts)
        {
            this._playerContext = contexts.player;
            this._vehicleContext = contexts.vehicle;
        }

        public override bool Enable
        {
            get
            {
                return IsOnVehicle && !IsPlayerDead && base.Enable;
            }
            set { base.Enable = value; }
        }

        public bool IsOnVehicle
        {
            get { return null != _playerContext.flagSelfEntity && _playerContext.flagSelfEntity.IsOnVehicle(); }
        }

        private VehicleEntity GetCurrentVehicle()
        {
            return _vehicleContext.GetEntityWithEntityKey(_playerContext.flagSelfEntity.controlledVehicle.EntityKey);
        }

        public bool IsWheelBrokeByIndex(VehiclePartIndex index, out VehicleUiWheelIndex uiIndex)
        {
            AssertUtility.Assert(IsOnVehicle);

            uiIndex = VehicleUiWheelIndex.None;
            var vehicle = GetCurrentVehicle();
            if (vehicle != null && vehicle.IsCar())
            {
                uiIndex = WheelEntityUtility.GetUiWheelIndex(vehicle, index);
                return vehicle.vehicleBrokenFlag.IsVehiclePartBroken(index);
            }

            return false;
        }

        public bool IsSeatOccupied(VehicleSeatIndex index, out VehicleSeatIndex uiIndex)
        {
            AssertUtility.Assert(IsOnVehicle);

            uiIndex = VehicleSeatIndex.None;
            var vehicle = GetCurrentVehicle();
            if (vehicle != null)
            {
                var seats = vehicle.vehicleSeat;
                if (seats.IsConfigured((int)index))
                {
                    uiIndex = index;
                }

                return seats.IsOccupied((int)index);
            }

            return false;
        }

        public float CurVehicleHp
        {
            get
            {
                AssertUtility.Assert(IsOnVehicle);

                var vehicle = GetCurrentVehicle();
                if (vehicle != null && vehicle.HasGameData())
                {
                    return vehicle.GetGameData().Hp;
                }

                return 100.0f;
            }
        }
        public float MaxVehicleHp
        {
            get {
                AssertUtility.Assert(IsOnVehicle);

                var vehicle = GetCurrentVehicle();
                if (vehicle != null && vehicle.HasGameData())
                {
                    return vehicle.GetGameData().MaxHp;
                }

                return 100.0f;
            }
        }
        public float CurVehicleSpeed
        {
            get
            {
                AssertUtility.Assert(IsOnVehicle);

                var vehicle = GetCurrentVehicle();
                if (vehicle != null)
                {
                    return vehicle.GetUiPresentSpeed();
                }

                return 0f;
            }
        }

        public float CurVehicleOil
        {
            get
            {
                AssertUtility.Assert(IsOnVehicle);

                var vehicle = GetCurrentVehicle();
                if (vehicle != null && vehicle.HasGameData())
                {
                    return vehicle.GetGameData().RemainingFuel;
                }

                return 0.0f;
            }

        }
        public float MaxVehicleOil
        {
            get
            {
                AssertUtility.Assert(IsOnVehicle);

                var vehicle = GetCurrentVehicle();
                if (vehicle != null && vehicle.HasGameData())
                {
                    return vehicle.GetGameData().MaxFuel;
                }

                return 100.0f;
            }
        }
        public bool IsPlayerDead
        {
            get { return _playerContext.flagSelfEntity.gamePlay.IsLifeState(EPlayerLifeState.Dead); }
        }

        public AssetInfo CurVehicleAssetInfo
        {
            get
            {
                AssertUtility.Assert(IsOnVehicle);

                var vehicle = GetCurrentVehicle();
                var curId = vehicle.vehicleAssetInfo.Id;
                AssetInfo assetInfo = new AssetInfo(AssetBundleConstant.UiPrefab_Carry, "CarryTopView" + curId);
                return assetInfo;
            }
        }
    }
}
