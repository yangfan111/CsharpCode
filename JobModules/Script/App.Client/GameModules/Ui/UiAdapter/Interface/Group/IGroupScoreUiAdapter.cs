using System.Collections.Generic;
using App.Shared.Components.Ui;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;
using Core.Enums;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public interface IGroupScoreUiAdapter : IAbstractUiAdapter
    {
        /// <summary>
        /// 完成每回合游戏所需要的时间(毫秒)
        /// </summary>
        int GameTime { get; set; }
        /// <summary>
        /// 阵营对应的玩家总人数，死亡人数
        /// </summary>
        /// <param name="campType"></param>
        /// <returns></returns>
        IPlayerCountData GetDataByCampType(EUICampType campType);
        List<IGroupBattleData> GetBattleDataListByCampType(EUICampType type);

        /// <summary>
        /// 获胜所需的分数
        /// </summary>
        int ScoreForWin { get; }
        /// <summary>
        /// 每个阵营的分数
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        int GetScoreByCamp(EUICampType type);
        bool NeedPause { get; }

    }
}
