using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Core.Prediction;
using Core.Prediction.UserPrediction.Cmd;

namespace Core.Fsm
{
    public class FsmInputEqualityComparer : IEqualityComparer<FsmInput>
    {
        public bool Equals(FsmInput x, FsmInput y)
        {
            return x == y;
        }

        public int GetHashCode(FsmInput obj)
        {
            return (int) obj;
        }
        
        private static FsmInputEqualityComparer _instance = new FsmInputEqualityComparer();
        public static FsmInputEqualityComparer Instance { get { return _instance; } }
    }
    
    public enum FsmInput
    {
        
        // 向左侧身
        PeekLeft,
        // 向右侧身
        PeekRight,
        // 不侧身
        NoPeek,
        
        // force set posture
        PostureStand,
        PostureCrouch,
        PostureProne,

        // 蹲
        Crouch,
        // 趴
        Prone,
        ToProneTransitFinish,
        OutProneTransitFinish,
        // 起跳
        Jump,
        // 落地及缓冲状态
        Land,
        // 落地过程
        LandProgressP3,
        LandProgressP1,
        // 落地动画结束
        JumpEndFinished,
        // 自由落体(跳跃空中循环)
        Freefall,
        // 滑落
        Slide,
        // Slide End
        SlideEnd,
        // 游泳
        Swim,
        // 上岸
        Ashore,
        // 潜泳
        Dive,
        // 濒死
        Dying,
        // 复活
        Revive,
        // 队友救援
        Rescue,
        // 停止队友救援
        RescueEnd,

        // 静止
        Idle,
        // 静走
        Walk,
        // 跑
        Run,
        // 冲刺
        Sprint,
        // 
        DiveMove,
        DiveIdle,
        // 向前
        Forth,
        // 向后
        Back,
        // 向左
        Left,
        // 向右
        Right,
        // 上浮
        Up,
        // 下潜
        Down,
        //
        DiveUpDown,

        // 开火
        Fire,
        // 特殊开火
        SpecialFire,
        // 开火结束
        SpecialFireEnd,
        // 开镜开枪
        SightsFire,
        // 特殊开镜开枪
        SpecialSightsFire,
        // 开火动画结束
        FireFinished,
        // 开火结束动画结束
        FireEndFinished,
        // 受击
        Injury,
        // 受击动画结束
        InjuryFinished,
        // 非空仓换弹
        Reload,
        // 空仓换弹
        ReloadEmpty,
        // 分阶段换弹（霰弹枪）
        SpecialReload,
        // 分阶段换弹中，成功换了一颗子弹
        SpecialReloadTrigger,
        // 换弹动画结束
        ReloadFinished,
        // 收枪
        Unarm,
        // 收枪过程
        HolsterProgressP3,
        HolsterProgressP1,
        // 收枪动画结束
        HolsterFinished,
        // 拔枪
        Draw,
        //拔枪过程
        SelectProgressP3,
        SelectProgressP1,
        // 拔枪动画结束
        SelectFinished,
        // 换枪
        SwitchWeapon,
        // 打断换枪
        InterruptSwitchWeapon,

        // 第一人称
        FirstPerson,
        // 第三人称
        ThirdPerson,

        // 第三人称机瞄
        Sight,
        // 取消第三人称机瞄
        CancelSight,

        // 拾取过程
        PickUpProgressP3,
        PickUpProgressP1,
        // 拾取
        PickUp,
        PickUpEnd,

        // 开门过程
        OpenDoorProgressP3,
        OpenDoorProgressP1,
        //开门
        OpenDoor,
        OpenDoorEnd,

        // 使用道具过程
        PropsProgressP3,
        PropsProgressP1,
        //使用道具
        Props,
        PropsEnd,
        FinishProps,

        //驾驶
        DriveStart,
        DriveEnd,

        // 打断特殊换弹，然后拉栓
        BreakSpecialReload,
        // 打断特殊换弹，不拉栓
        ForceBreakSpecialReload,

        // 近战轻击
        LightMeleeAttackOne,
        LightMeleeAttackTwo,
        // 近战重击
        MeleeSpecialAttack,
        // 出击过程
        MeleeAttackProgressP3,
        MeleeAttackProgressP1,
        MeleeAttackFinished,

        // 滑翔
        Gliding,

        // 跳伞
        Parachuting,
        ParachutingEnd,
        //开伞结束
        ParachuteOpen1Finished,

        //投掷动作
        StartFarGrenade,
        StartNearGrenade,
        ChangeGrenadeDistance,
        FinishGrenade,
        ForceFinishGrenade,
        GrenadeEndFinish,
        

        //C4
        BuriedBomb,
        BuriedBombFinished,
        DismantleBomb,
        DismantleBombFinished,
        
        // 开火动画的过程
        FireProgressP3,
        FireProgressP1,
        FireEndProgressP3,
        FireEndProgressP1,
        SightsFireProgressP3,
        SightsFireProgressP1,
        ReloadProgressP3,
        ReloadProgressP1,
        ReloadEmptyProgressP3,
        ReloadEmptyProgressP1,
        ReloadStartProgressP3,
        ReloadStartProgressP1,
        ReloadLoopProgressP3,
        ReloadLoopProgressP1,
        ReloadEndProgressP3,
        ReloadEndProgressP1,
        // 扔手雷结束过程
        ThrowEndProgressP3,
        ThrowEndProgressP1,
        // C4动画过程
        BuriedBombProgressP3,
        BuriedBombProgressP1,
        DismantleBombProgressP3,
        DismantleBombProgressP1,
        
        // 进入||离开人物姿势状态
        IntoWalkAndProneTransit,
        OutofWalkAndProneTransit,

        // 打断行为
        InterruptAction,

        // 
        Climb,
        Step,
        GenericActionFinished,

        None
        
    }

    public interface IFsmInputCommand
    {
        FsmInput Type { get; set; }
        // 1. value of horizontal or vertical
        // 2. value of interrupted transition
        float AdditioanlValue { get; set; }
        float AlternativeAdditionalValue { get; set; }
        bool Handled { get; set; }
        bool IsMatch(FsmInput type);
      
        void Reset();
    }
}
