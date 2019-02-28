using System.Collections.Generic;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;

namespace App.Shared.Components.Ui.UiAdapter
{
    public interface IKillMessageUiAdapter : IAbstractUiAdapter
    {
        /// <summary>
        /// 是否有新的击杀信息产生
        /// </summary>
        bool KillMessageChanged { get; set; }

        void UpdateKillInfo();

        List<IKillInfoItem> KillInfos { get; }

        long SelfTeamId { get; }
    }
}
