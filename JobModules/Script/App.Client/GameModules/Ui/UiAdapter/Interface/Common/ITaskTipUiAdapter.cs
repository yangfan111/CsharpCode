using System.Collections.Generic;
using App.Shared.Components.Ui;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;

namespace App.Client.GameModules.Ui.UiAdapter.Interface
{
    /// <summary>
    /// 可通过设置屏蔽任务提示，修改Enable即可
    /// </summary>
    public interface ITaskTipUiAdapter : IAbstractUiAdapter
    {
        /// <summary>
        /// 变动任务列表，UI会一次性依次播完所有数据，并清空列表
        /// 当前玩家不处于战斗状态才允许添加
        /// 战斗状态:3秒内发生玩家攻击，受伤，瞄准
        /// 同时发生的任务变动事件以ID排序加入
        /// </summary>
        List<ITaskTipData> TaskTipDataList { get; }
    }
}
