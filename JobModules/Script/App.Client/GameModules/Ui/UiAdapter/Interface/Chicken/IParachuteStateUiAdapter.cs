using App.Shared.Components.Ui;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public interface IParachuteStateUiAdapter : IAbstractUiAdapter
    {
        /// <summary>
        /// 表示玩家是否处于降落过程中
        /// </summary>
        bool IsDrop { get; }
        /// <summary>
        /// 表示玩家当前坐标对应的地形高度
        /// </summary>
        float TerrainHeight { get; }
        /// <summary>
        /// 表示玩家降落过程中的即时速度
        /// </summary>
        float DropSpeed { get; }
        /// <summary>
        /// 表示玩家当前距离地面的垂直高度
        /// </summary>
        float CurHeight { get; }
        /// <summary>
        /// 跳伞总高度
        /// </summary>
        float TotalHeight { get; }
        /// <summary>
        /// 强制开伞高度
        /// </summary>
        float ForcedHeight { get; }
    }
}
