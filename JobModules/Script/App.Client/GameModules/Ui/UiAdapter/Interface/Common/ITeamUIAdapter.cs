using System.Collections.Generic;
using App.Shared.Components.Ui;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public interface ITeamUiAdapter : IAbstractUiAdapter
    {
        List<MiniMapTeamPlayInfo> TeamPlayerInfos { get; }  //小队队员当前的 编号 名字 颜色 血量  是否标记 状态 
        int ModeID { get; }       //模式ID  单人模式或者组队模式
    }
}
