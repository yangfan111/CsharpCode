using System;
using App.Shared.Audio;
using App.Shared.Components.Player;
using App.Shared.EntityFactory;
using App.Shared.GameModules.Player;
using App.Shared.GameModules.Weapon;
using Assets.Utils.Configuration;
using Core;
using Core.Appearance;
using Core.CharacterState;
using Core.EntityComponent;
using Shared.Scripts;
using UnityEngine;
using Utils.Appearance;
using Utils.CharacterState;
using Utils.Configuration;
using Utils.Singleton;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared
{
    /// <summary>
    ///     定义与Weapon相关连的Player weapon components原子操作
    /// </summary>
    public static class WeaponRelatedExt
    {
        public static bool IsAiming(this CameraStateNewComponent component)
        {
            return component.ViewNowMode == (short) ECameraViewMode.GunSight;
        }

        public static bool IsThird(this CameraStateNewComponent component)
        {
            return component.ViewNowMode == (short) ECameraViewMode.ThirdPerson;
        }

        public static void CleanThrowingState(this ThrowingActionData actionData, bool interrupt = false)
        {
            if (actionData.IsPull)
            {
                //若已拉栓，销毁ThrowingEntity
                ThrowingEntityFactory.DestroyThrowing(actionData.ThrowingEntityKey);
            }

            actionData.IsReady = false;
            actionData.InternalCleanUp(interrupt);
        }

        public static bool CanThrow(this ThrowingActionData throwingActionData)
        {
            return throwingActionData.IsReady && !throwingActionData.IsThrow;
        }

        public static void SetThrow(this ThrowingActionData throwingActionData)
        {
            throwingActionData.IsThrow         = true;
            throwingActionData.IsThrowing      = true;
            throwingActionData.ShowCountdownUI = false;
        }
        public static bool CanReady(this ThrowingActionData throwingActionData)
        {
            return (!throwingActionData.IsReady && throwingActionData.ThrowingEntityKey == EntityKey.Default &&
                            throwingActionData.LastFireWeaponKey == 0);
        }

        public static void SetReady(this ThrowingActionData throwingActionData, int currKey, ThrowingConfig config)
        {
            throwingActionData.IsReady           = true;
            throwingActionData.Config            = config;
            throwingActionData.LastFireWeaponKey = currKey;
        }

        public static bool TrySetSwitch(this ThrowingActionData throwingActionData, int currentTime)
        {
            if (throwingActionData.IsReady && !throwingActionData.IsThrow &&
                (currentTime - throwingActionData.LastSwitchTime) >= GlobalConst.SwitchCdTime)
            {
                throwingActionData.LastSwitchTime = currentTime;
                throwingActionData.IsNearThrow    = !throwingActionData.IsNearThrow;
                return true;
            }

            return false;
        }

        public static bool CanPull(this ThrowingActionData throwingActionData)
        {
            return throwingActionData.IsReady && !throwingActionData.IsPull;
        }

        public static void SetPull(this ThrowingActionData throwingActionData,EntityKey entityKey)
        {
            throwingActionData.IsPull          = true;
            throwingActionData.ShowCountdownUI = true;
            throwingActionData.IsInterrupt     = false;
            throwingActionData.ThrowingEntityKey = entityKey;
        }

        public static void Reset(this OrientationComponent component)
        {
            // 更新枪械时，后坐力重置
            component.PunchPitch         = 0;
            component.PunchYaw           = 0;
            component.AccPunchPitchValue = 0;
            component.AccPunchYawValue   = 0;
            component.FireRoll           = 0;
        }
        public static bool CanExplosion(this ThrowingEntity throwingEntity)
        {
            return throwingEntity.hasLifeTime && (DateTime.Now - throwingEntity.lifeTime.CreateTime).TotalMilliseconds >
                            throwingEntity.throwingData.ThrowConfig.CountdownTime;
        }
        public static bool CanStartFlySimulation(this ThrowingEntity throwingEntity)
        {
            return (throwingEntity.throwingData.IsThrow && !throwingEntity.throwingData.IsFly && !throwingEntity.isFlagDestroy);
        }
        /// <summary>
        ///     ChangeC4ToBag
        /// </summary>
        /// <param name="playerEntity"></param>
        /// <param name="weaponId"></param>
        public static void UnmoutC4(this ICharacterAppearance Appearance, Sex sex)
        {
            Appearance.UnmountWeaponInPackage(WeaponInPackage.TacticWeapon);
            var c4ResId = SingletonManager.Get<RoleAvatarConfigManager>().GetResId(RoleAvatarConfigManager.C4, sex);
            if (SharedConfig.IsServer)
                Appearance.ChangeAvatar(c4ResId);
        }

        /// <summary>
        ///     ChangeBagToC4
        /// </summary>
        /// <param name="Appearance"></param>
        /// <param name="weaponId"></param>
        public static void MountC4(this ICharacterAppearance Appearance, int weaponId)
        {
            var c4AvatarId = SingletonManager.Get<WeaponResourceConfigManager>().GetAvatarByWeaponId(weaponId);
            Appearance.MountWeaponInPackage(WeaponInPackage.TacticWeapon, c4AvatarId);
            Appearance.ClearAvatar(Wardrobe.Bag);
        }

        public static void RemoveC4(this ICharacterAppearance Appearance)
        {
            Appearance.UnmountWeaponInPackage(WeaponInPackage.TacticWeapon);
            Appearance.ClearAvatar(Wardrobe.Bag);
        }

        public static void CharacterUnmount(this ICharacterState State, Action holsterStart, Action holsterEnd,
                                            float unarmParam)
        {
            State.InterruptAction();
            State.ForceFinishGrenadeThrow();
            State.Holster(holsterStart, holsterEnd, unarmParam);
        }

        public static GameObject WeaponHandObject(this ICharacterAppearance Appearance)
        {
            return Appearance.GetWeaponP1InHand();
        }

        public static PlayerWeaponController WeaponController(this EntityKey owner)
        {
            return GameModuleManagement.Get<PlayerWeaponController>(owner.EntityId);
        }

        public static PlayerStateInteractController InteractController(this EntityKey owner)
        {
            return GameModuleManagement.Get<PlayerStateInteractController>(owner.EntityId);
        }

        public static PlayerStatisticsControllerBase StatisticsController(this EntityKey owner)
        {
            return GameModuleManagement.Get<PlayerStatisticsControllerBase>(owner.EntityId);
        }

        public static PlayerAudioControllerBase AudioController(this EntityKey owner)
        {
            return GameModuleManagement.Get<PlayerAudioControllerBase>(owner.EntityId);
        }

        public static GameModeControllerBase ModeController(this EntityKey owner)
        {
            return GameModuleManagement.Get<GameModeControllerBase>(owner.EntityId);
        }
    }
}