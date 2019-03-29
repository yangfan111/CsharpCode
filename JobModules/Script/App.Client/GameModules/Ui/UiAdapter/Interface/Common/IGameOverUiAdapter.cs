using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;
using Core.Enums;

namespace App.Shared.Components.Ui.UiAdapter
{

    public interface IGameOverUiAdapter : IAbstractUiAdapter
    {
        /// <summary>
        /// 游戏结果，胜利or失败
        /// </summary>
        EUIGameResultType GameResult { get;}
        /// <summary>
        /// 游戏结束，进入大厅结算
        /// </summary>
        void GameOver();

        void SetCrossVisible(bool isVisible);
    }
}
