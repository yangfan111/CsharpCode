using Core;
using Core.Fsm;
using System;
using XmlConfig;

namespace Core.CharacterState.Action
{
    public interface ICharacterAction
    {
        bool CanFire();
        void Fire();
        void SpecialFire(System.Action callBack);
        void SpecialFireEnd();
        void SightsFire();
        void SpecialSightsFire(System.Action callBack);
        void Reload(System.Action callBack);
        void ReloadEmpty(System.Action callBack);
        void BeenHit();
        bool CanBeenHit();

        void Unarm(System.Action callBack, float unarmParam);
        void Draw(System.Action callBack, float drawParam);
        void SwitchWeapon(System.Action unarmCallback, System.Action drawCallBack, float switchParam);
        void InterruptSwitchWeapon();

        void SetDiveUpDownValue(float value);

        void SetPostureStand();
        void SetPostureCrouch();
        void SetPostureProne();

        void Stand();
        void Crouch();
        void Rescue();
        void RescueEnd();
        void Swim();
        void Dive();
        void Ashore();
        void Freefall();
        void Slide();
        void SlideEnd();

        void DriveStart(int seatId, int postureId);
        void DriveEnd();

        // 第三人称机瞄
        void SetSight(float speedRatio);
        // 取消第三人称机瞄
        void CancelSight(float speedRatio);
        // 特殊换弹
        void SpecialReload(System.Action callBack, int count, System.Action finishReloadCallBack);
        // 打断特殊换弹
        void BreakSpecialReload();
        void ForceBreakSpecialReload(System.Action callBack);
        // 拾取物品的动作
        void PickUp();
        // 开门动作
        void OpenDoor();
        // 使用道具
        void UseProps(float propKind);
        void FinishProps();

        void LightMeleeAttackOne(System.Action callBack);
        void LightMeleeAttackTwo(System.Action callback);
        void MeleeSpecialAttack(System.Action callBack);

        void Gliding();

        void Parachuting(System.Action callBack);
        void ParachutingEnd();
        //投掷动作
        void StartNearGrenadeThrow();
        void StartFarGrenadeThrow();
        //0--near  1--far
        void ChangeThrowDistance(float weight);
        void FinishGrenadeThrow();
        void ForceFinishGrenadeThrow();

        void C4Animation(System.Action callBack);
        void BuriedBomb(System.Action callBack);
        void DismantleBomb(System.Action callBack);

        // Action
        void StartClimb(float kind, System.Action callBack);

        void InterruptAction();
        bool NeedInterruptRescue(PostureInConfig posture);
    }
}
