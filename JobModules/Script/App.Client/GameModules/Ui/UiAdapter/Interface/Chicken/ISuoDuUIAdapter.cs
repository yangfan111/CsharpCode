using App.Shared.Components.Ui;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;
using Core.Ui.Map;
using UnityEngine;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public interface ISuoDuUiAdapter : IAbstractUiAdapter
    {
        MapFixedVector3 CurPosition { get; }   //玩家当前位置
        DuQuanInfo CurDuquan { get; }  //当前毒圈
        DuQuanInfo NextDuquan { get; }  //下一个毒圈
        int OffLineNum { get; }
    }
}
