using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;
using Core.Enums;

namespace App.Shared.Components.Ui.UiAdapter
{

    public interface IGameTitleUiAdapter : IAbstractUiAdapter
    {
        /// <summary>
        /// 当前玩家的游戏头衔，多状态（使用(2^GameTitleType)异或|处理,如同时满足KD王和ACE，那么该值为2|4=6）
        /// </summary>
        int GameTitle { get; }
    }
}