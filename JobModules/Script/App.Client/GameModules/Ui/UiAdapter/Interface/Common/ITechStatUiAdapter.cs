using App.Shared.Components.Ui;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;
using Core.Statistics;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public interface ITechStatUiAdapter : IAbstractUiAdapter
    {   
        /// <summary>
        /// 是否显示面板
        /// </summary>
        //bool IsShow { get; }

        //战斗技术统计
        BattleData BattleData { get; }
        /// <summary>
        /// 是否允许救援
        /// </summary>
        bool CanRescue { get; }
    }
}
