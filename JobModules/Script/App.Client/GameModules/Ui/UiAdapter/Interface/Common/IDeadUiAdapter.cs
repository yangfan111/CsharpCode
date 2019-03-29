using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;

namespace App.Client.GameModules.Ui.UiAdapter.Interface.Common
{

    public interface IDeadUiAdapter : IAbstractUiAdapter
    {
        /// <summary>
        /// 是否显示死亡界面上的按钮组
        /// </summary>
        bool DeadButtonShow { get; }
        /// <summary>
        /// 观战
        /// </summary>
        void Observe();
        /// <summary>
        /// 返回大厅
        /// </summary>
        void BackToHall();
        /// <summary>
        /// 当前玩家是否有存活队友
        /// </summary>
        bool HaveAliveTeammate { get; }
        /// <summary>
        /// 控制准心(UI层)
        /// </summary>
        /// <param name="isVisible"></param>
        void SetCrossVisible(bool isVisible);

    }
}
