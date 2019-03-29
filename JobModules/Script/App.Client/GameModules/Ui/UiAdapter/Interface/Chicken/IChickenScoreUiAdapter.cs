using App.Shared.Components.Ui;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;

namespace App.Client.GameModules.Ui.UiAdapter
{

    public interface IChickenScoreUiAdapter : IAbstractUiAdapter
    {
        /// <summary>
        /// 表示游戏是否开始
        /// </summary>
        bool IsGameBegin { get; }

        //加入玩家数
        int JoinPlayerCount { get;}

        /// <summary>
        /// 当前游戏中的幸存者(未死亡的玩家)数量
        /// </summary>
        int SurvivalCount { get;}

        /// <summary>
        /// 当前玩家击杀其它玩家的数量
        /// </summary>
        int BeatCount { get; }

    }
}
