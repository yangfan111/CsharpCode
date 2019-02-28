using App.Shared.Components.Ui;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;
using UnityEngine;
using UserInputManager.Lib;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public class RangingInfo
    {
        public RangingInfo(long distance, Vector3 contactPos)
        {
            this.distance = distance;
            this.contactPos = contactPos;
        }

        public long distance;                          //自己距离碰撞对象的距离
        public Vector3 contactPos;                       //射线碰撞的对象位置        
    }

    public interface IRangingUiAdapter : IAbstractUiAdapter
    {
        RangingInfo RangeInfo { get; set; }
        PlayerEntity GetPlayerEntity();
    }
}
