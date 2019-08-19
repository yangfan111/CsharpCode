using App.Shared.Components.Ui;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;
using UnityEngine;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public interface IPlayerStateUiAdapter : IAbstractUiAdapter
    {
        int CurrentHp { get; }
        int MaxHp { get; }
        int MayRecoverHp { get; }
        int FirstOrThirdView { get; }  //玩家现在的视角 第一或者第三视角
        int CurPose { get; }           //玩家现在的姿势 蹲 趴 站
        int CurUIModel { get; }        //当前玩家 选中的UI模式， 
        bool SpeedBufActive { get; }    //加速buff
        bool RecureBufActive { get; }   //回血buff
        float CurO2 { get; }           //当前含氧量
        float MaxCurO2 { get; }        //最大含氧量
        float CurPower { get; }        //当前能量值

        float maxHelmet { get; }          //头盔最大值
        float curHelmet { get; }          //头盔当前值

        //防弹衣
        bool IsDead { get; }
        float maxArmor { get; }      //防弹衣最大值         
        float curArmor { get; }      //防弹衣当前值

        bool IsInHurtedState { get; } //是否在受伤状态
        int CurrentHpInHurtedState { get; }
        ParticleSystem MyParticle { get; }  //掉血特效

        int ArmorLevel { get; }
        int HelmetLevel { get; }
    }
}
