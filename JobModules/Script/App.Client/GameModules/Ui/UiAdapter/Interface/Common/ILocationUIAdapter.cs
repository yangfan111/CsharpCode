using System.Collections.Generic;
using App.Shared.Components.Ui;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;
using UnityEngine;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public interface ILocationUiAdapter : IAbstractUiAdapter
    {
        float _CurFaceDirection { get; set; }   //玩家当前面朝向
        List<TeamPlayerMarkInfo> TeamPlayerMarkInfos { get;}  //小队队员当前的 方位和 标记颜色
    }
}
