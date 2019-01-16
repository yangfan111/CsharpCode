using App.Shared.Player;
using Core.Bag;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Utils.Appearance;

namespace App.Shared.WeaponLogic
{
    public class WeaponEntityFacade
    {
        private PlayerEntity entity;
        private PlayerWeaponController weaponController;

        private bool IsInitialized { get { return entity != null; } }
        public WeaponEntityFacade(PlayerWeaponController in_weaponController, PlayerEntity in_entity)
        {
            entity = in_entity;
            weaponController = in_weaponController;
        }
        public WeaponEntityFacade(PlayerWeaponController in_weaponController)
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
            state.SwitchWeapon(drawCallback, drawCallback, switchParam);
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

    }
}