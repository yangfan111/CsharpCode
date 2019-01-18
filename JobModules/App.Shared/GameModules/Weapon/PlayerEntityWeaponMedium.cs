using App.Shared.EntityFactory;
using App.Shared.Player;
using App.Shared.Util;
using Assets.Utils.Configuration;
using Core;
using Core.Common;
using Core.GameModeLogic;
using Core.WeaponLogic.Attachment;
using Utils.Appearance;
using Utils.Singleton;
using Utils.Utils;
using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon
{
    public class PlayerEntityWeaponMedium
    {
        private PlayerEntity entity;
        private PlayerWeaponController weaponController;
        public IWeaponProcessListener ProcessListener { private get; set; }

        private bool IsInitialized { get { return entity != null; } }
        public PlayerEntityWeaponMedium(PlayerWeaponController in_weaponController, PlayerEntity in_entity)
        {
            entity = in_entity;
            weaponController = in_weaponController;
        }
        public PlayerEntityWeaponMedium(PlayerWeaponController in_weaponController)
        {
            weaponController = in_weaponController;
        }
        public void SetEntity(PlayerEntity in_entity) { entity = in_entity; }

        public void Appearance_MountP3WeaponOnAlternativeLocator()
        {
            entity.appearanceInterface.Appearance.MountP3WeaponOnAlternativeLocator();
        }
        public void Appearance_MountWeaponToHand(WeaponInPackage pos)
        {
            entity.appearanceInterface.Appearance.MountWeaponToHand(pos);

        }
        public void Appearance_RemountP3WeaponOnRightHand()
        {
            entity.appearanceInterface.Appearance.RemountP3WeaponOnRightHand();
        }
        public void Appearance_UnmountWeaponInPackage(WeaponInPackage pos)
        {
            entity.appearanceInterface.Appearance.UnmountWeaponInPackage(pos);
        }
        public void Appearance_UnmountWeaponFromHand()
        {
            entity.appearanceInterface.Appearance.UnmountWeaponFromHand();
        }
        public void UnmountC4()
        {
            entity.UnmoutC4();
        }
        public void RemoveC4()
        {
            entity.RemoveC4();
        }
        public void CharacterState_ReloadEmpty(System.Action callback)
        {
            entity.stateInterface.State.ReloadEmpty(callback);
        }
        public void MountC4(int weaponId)
        {
            entity.MountC4(weaponId);
        }
        public void CharacterState_SwitchWeapon(System.Action unarmCallback, System.Action drawCallback, float switchParam)
        {
            Core.CharacterState.ICharacterState state = entity.stateInterface.State;
            state.SwitchWeapon(unarmCallback, drawCallback, switchParam);
        }
        public void CharacterState_Draw(System.Action drawCallback, float drawParam)
        {
            Core.CharacterState.ICharacterState state = entity.stateInterface.State;
            state.Draw(drawCallback, drawParam);
        }
        public void CharacterState_DrawInterrupt()
        {
            PlayerStateUtil.AddPlayerState(EPlayerGameState.InterruptItem, entity.gamePlay);
            entity.stateInterface.State.InterruptAction();
            entity.stateInterface.State.ForceFinishGrenadeThrow();
        }
        public void CharacterState_Unmount(System.Action unarm, float unarmParam)
        {
            Core.CharacterState.ICharacterState state = entity.stateInterface.State;
            state.InterruptAction();
            state.ForceFinishGrenadeThrow();
            state.Unarm(unarm, unarmParam);
        }
        public void CharacterState_Interrupt()
        {
            Core.CharacterState.ICharacterState state = entity.stateInterface.State;
            state.InterruptAction();
            state.InterruptSwitchWeapon();
            PlayerStateUtil.AddPlayerState(EPlayerGameState.InterruptItem, entity.gamePlay);
            state.ForceBreakSpecialReload(null);
            state.ForceFinishGrenadeThrow();
            if (entity.hasThrowingAction)
            {
                entity.throwingAction.ActionInfo.ClearState();
            }
        }
        public bool Model_IsSlotAvailable(EWeaponSlotType slot)
        {
            return entity.modeLogic.ModeLogic.IsSlotValid(slot);
        }
        public void ThrowAction_Execute()
        {
            if (!entity.hasThrowingAction) return;
            Core.WeaponLogic.Throwing.ThrowingActionInfo actionInfo = entity.throwingAction.ActionInfo;
            Core.CharacterState.ICharacterState state = entity.stateInterface.State;
            if (actionInfo.IsReady && actionInfo.IsPull)
            {
                //若已拉栓，销毁ThrowingEntity
                actionInfo.IsInterrupt = true;
            }
            //打断投掷动作
            state.ForceFinishGrenadeThrow();
            //清理手雷状态
            actionInfo.ClearState();
        }

        public void Listener_Pickup(EWeaponSlotType slot)
        {
            ProcessListener.OnPickup(entity, slot);
        }
        public int Model_GetReserveBullet(EWeaponSlotType slot)
        {
            return entity.modeLogic.ModeLogic.GetReservedBullet(entity, slot);
        }
        public int Model_GetReserveBullet(EBulletCaliber caliber)
        {
            return entity.modeLogic.ModeLogic.GetReservedBullet(entity, caliber);
        }
        public int Model_SetReservedBullet(EWeaponSlotType slot,int count)
        {
            return entity.modeLogic.ModeLogic.SetReservedBullet(entity, slot,count);
        }
        public int Model_SetReservedBullet(EBulletCaliber caliber,int count)
        {
            return entity.modeLogic.ModeLogic.SetReservedBullet(entity, caliber, count);
        }
        public void Listener_Drop(EWeaponSlotType slot)
        {
            ProcessListener.OnDrop(entity, slot);
        }
        public void Listener_OnExpend(EWeaponSlotType slot)
        {
            ProcessListener.OnExpend(entity, slot);
        }
        public void Player_ClearPlayerWeaponState()
        {
            entity.ClearPlayerWeaponState();
        }
        public void Player_RefreshPlayerWeaponLogic(int id )
        {
            entity.RefreshPlayerWeaponLogic(id);
        }
        public void Weapon_SetAttachment(WeaponPartsStruct attachments)
        {
            entity.weaponLogic.Weapon.SetAttachment(attachments);
        }
        public void Apperance_RefreshABreath()
        {
            var breath = entity.weaponLogic.Weapon.GetBreathFactor();
            entity.appearanceInterface.FirstPersonAppearance.SightShift.SetAttachmentFactor(breath);
        }
        public void Model_RefreshWeaponModel(int weaponId,EWeaponSlotType slot,WeaponPartsStruct attachments)
        {
            // 添加到背包的时候会执行刷新模型逻辑
         //_playerEntity.RefreshWeaponModel(weapon.WeaponId, curWeaponSlot, attachments);
        }
        public void Model_RefreshWeaponParts(int weaponId, EWeaponSlotType slot, WeaponPartsStruct oldAttachment, WeaponPartsStruct newAttachments)
        {
           WeaponPartsUtil.RefreshWeaponPartModels(entity.appearanceInterface.Appearance, weaponId, oldAttachment, newAttachments, slot);
        }
        public void Appearence_ProcessMountWeaponInPackage(WeaponInPackage pos, int weaponId, int avatarId)
        {
            if (SingletonManager.Get<WeaponConfigManager>().IsC4(weaponId))
            {
                entity.AddC4ToBag(weaponId);
            }
            else
            {
                entity.appearanceInterface.Appearance.MountWeaponInPackage(pos, avatarId);
            }
        }
        public void ShowTip(ETipType tip)
        {
            entity.tip.TipType = ETipType.NoWeaponInSlot;
        }
        public Err_WeaponLogicErrCode CanUseGreande()
        {
            if (!entity.hasThrowingAction)
                return Err_WeaponLogicErrCode.Err_PlayerDontHaveThrowingComponent;
            var pull = entity.throwingAction.ActionInfo.IsPull;
            var destroy = entity.throwingAction.ActionInfo.IsInterrupt;
            var fly = entity.throwingUpdate.IsStartFly;
            bool ret = (pull && !destroy && !fly);
            return ret ? Err_WeaponLogicErrCode.Sucess : Err_WeaponLogicErrCode.Err_PlayerGrenadePullVertifyFailed;
        }
    }
}