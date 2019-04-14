﻿using App.Shared.Components.Ui;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;
using Core.Enums;

namespace App.Client.GameModules.Ui.UiAdapter.Interface
{
    public interface IBlastScoreUiAdapter : IAbstractUiAdapter
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

        /// <summary>
        /// C4安装的状态，Installing表示安装中，显示图标；Completed表示完成，暂停倒计时；安装中断或者游戏重新开始设为None
        /// </summary>
        EUIBombInstallState C4InstallState { get; set; }
        /// <summary>
        /// 每个阵营的人数上限,上限为0时界面不会更新
        /// </summary>
        int PlayerCapacityPerCamp { get;}
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