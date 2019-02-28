using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;
using Core.Enums;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public interface IGroupScoreUiAdapter : IAbstractUiAdapter
    {
        /// <summary>
        /// 完成每局游戏所需要的时间(毫秒)
        /// </summary>
        int GameTime { get; set; }
        /// <summary>
        /// 胜利所需的击杀数(限时模式则返回0）
        /// </summary>
        int KillCountForWin { get; }
        /// <summary>
        /// 阵营对应的击杀数
        /// </summary>
        /// <param name="campType"></param>
        /// <returns></returns>
        int GetKillCountByCampType(EUICampType campType);
        bool NeedPause { get; }

    }
}
