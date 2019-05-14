using Core.Prediction.VehiclePrediction.Cmd;
using App.Shared.Components.Ui;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;
using Utils.AssetManager;
using VehicleCommon;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public interface IVehicleUiAdapter : IAbstractUiAdapter
    {
        /// <summary>
        /// 表示玩家是否在载具上
        /// </summary>
        bool IsOnVehicle { get; }
        /// <summary>
        /// 表示当前载具的即时血量
        /// </summary>
        float CurVehicleHp { get; }
        /// <summary>
        /// 表示当前载具的最大血量
        /// </summary>
        float MaxVehicleHp { get; }
        /// <summary>
        /// 表示当前载具的即时速度
        /// </summary>
        float CurVehicleSpeed { get; }
        /// <summary>
        /// 表示当前载具的即时油量
        /// </summary>
        float CurVehicleOil { get; }
        /// <summary>
        /// 表示当前载具的最大油量
        /// </summary>
        float MaxVehicleOil { get; }
        /// <summary>
        /// 表示当前载具的AssetBundle
        /// </summary>
        AssetInfo CurVehicleAssetInfo { get; }
        /// <summary>
        /// 表示玩家是否处于死亡状态
        /// </summary>
        bool IsPlayerDead { get; }
        /// <summary>
        /// 返回索引对应的轮胎是否爆破，同时返回Wheel UI Index 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        bool IsWheelBrokeByIndex(VehiclePartIndex index, out VehicleUiWheelIndex uiIndex);
        /// <summary>
        /// 返回座位是否上有人，同时返回Seat UI Index 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        bool IsSeatOccupied(VehicleSeatIndex index, out VehicleSeatIndex uiIndex);
    }
}
