using App.Shared.Components.Ui;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;
using Core.Enums;

namespace App.Client.GameModules.Ui.UiAdapter.Interface
{
    public interface IRoundOverUiAdapter : IAbstractUiAdapter
    {
        /// <summary>
        /// 当前游戏的即时回合数
        /// </summary>
        int CurRoundCount { get; }
        /// <summary>
        /// 返回每个阵营的得分
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        int GetScoreByCamp(EUICampType type);
    }
}
