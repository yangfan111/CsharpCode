using System.Collections.Generic;
using App.Shared.Components.Ui;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;
using UnityEngine;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public enum CrossHairType
    {
        None = 0,
        Normal = 1,
        Novisible = 2,
        AddBlood = 3,
        CountDown = 4
    };
    public enum CrossHairNormalTypeStatue
    {
        None = 0,
        Move = 1,
        Idel = 2,
        Shot = 3,
        StopShot = 4
    }
    

    public interface ICrossHairUiAdapter : IAbstractUiAdapter
    {
        CrossHairType Type { get; }                 //准心类型
        CrossHairNormalTypeStatue Statue { get; }   //常态类型准心的状态
        bool IsOpenCrossHairMotion { get; set; }   //是否开启准心运动
        int ShootNum { get; }  
        float AttackNum { get; }
        bool IsBurstHeart { get; }
        int WeaponAvatarId { get; }    //当前正在使用的武器类型
        bool IsShowCrossHair { get; }  //是否开启准心 并且同时影响鼠标
    }
}
