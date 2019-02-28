using App.Shared.Player;
using App.Shared.Util;
using Core;
using Core.Attack;
using Core.Common;
using Core.WeaponLogic.Attachment;
using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon
{
    /// <summary>
    /// Defines the <see cref="PlayerEntityWeaponInteract" />
    /// </summary>
    public partial class PlayerEntityWeaponInteract
    {
        public void SetEntity(PlayerEntity in_entity)
        {
            entity = in_entity;
        }

        public void UnmountC4()
        {
            RelatedAppearence.Appearance.UnmoutC4(entity.GetSex());
        }

        public void CreateSetMeleeAttackInfoSync(int in_Sync)
        {
            if (entity.hasMeleeAttackInfoSync)
            {
                RelatedMeleeAttackInfoSync.AttackTime = in_Sync;
            }
            else
            {
                entity.AddMeleeAttackInfoSync(in_Sync);
            }
        }

        public void CreateSetMeleeAttackInfo(MeleeAttackInfo attackInfo, MeleeFireLogicConfig config)
        {
            if (entity.hasMeleeAttackInfo)
            {
                RelatedMeleeAttackInfo.AttackInfo = attackInfo;
                RelatedMeleeAttackInfo.AttackConfig = config;
            }
            else
            {
                entity.AddMeleeAttackInfo();
                RelatedMeleeAttackInfo.AttackInfo = attackInfo;
                RelatedMeleeAttackInfo.AttackConfig = config;
            }
        }

        public void CharacterSwitchWeapon(System.Action unarmCallback, System.Action drawCallback, float switchParam)
        {
            RelatedCharState.State.SwitchWeapon(unarmCallback, drawCallback, switchParam);
        }

        public void CharacterDraw(System.Action drawCallback, float drawParam)
        {
            RelatedCharState.State.Draw(drawCallback, drawParam);
        }

        public void CharacterDrawInterrupt()
        {
            PlayerStateUtil.AddPlayerState(EPlayerGameState.InterruptItem, entity.gamePlay);
            RelatedCharState.State.InterruptAction();
            RelatedCharState.State.ForceFinishGrenadeThrow();
        }

        public void CharacterUnmount(System.Action unarm, float unarmParam)
        {
            RelatedCharState.State.InterruptAction();
            RelatedCharState.State.ForceFinishGrenadeThrow();
            RelatedCharState.State.Unarm(unarm, unarmParam);
        }

        public void CharacterInterrupt()
        {
            RelatedCharState.State.InterruptAction();
            RelatedCharState.State.InterruptSwitchWeapon();
            PlayerStateUtil.AddPlayerState(EPlayerGameState.InterruptItem, entity.gamePlay);
            RelatedCharState.State.ForceBreakSpecialReload(null);
            RelatedCharState.State.ForceFinishGrenadeThrow();
            if (entity.hasThrowingAction)
            {
                RelatedThrowAction.ActionInfo.ClearState();
            }
        }

        public bool ModelIsSlotAvailable(EWeaponSlotType slot)
        {
            return RelatedModelLogic.ModeLogic.IsSlotValid(slot);
        }

        public void ThrowActionExecute()
        {
            if (!entity.hasThrowingAction) return;
            Core.WeaponLogic.Throwing.ThrowingActionInfo actionInfo = RelatedThrowAction.ActionInfo;
            if (actionInfo.IsReady && actionInfo.IsPull)
            {
                //若已拉栓，销毁ThrowingEntity
                actionInfo.IsInterrupt = true;
            }
            //打断投掷动作
            RelatedCharState.State.ForceFinishGrenadeThrow();
            //清理手雷状态
            actionInfo.ClearState();
        }

        public void ApperanceRefreshABreath(float breath)
        {
            //TODO 动态获取
            RelatedAppearence.FirstPersonAppearance.SightShift.SetAttachmentFactor(breath);
        }

        public void ModelRefreshWeaponModel(int weaponId, EWeaponSlotType slot, WeaponPartsStruct attachments)
        {
        }

        public void ModelRefreshWeaponParts(int weaponId, EWeaponSlotType slot, WeaponPartsStruct oldAttachment, WeaponPartsStruct newAttachments)
        {
            WeaponPartsUtil.RefreshWeaponPartModels(RelatedAppearence.Appearance, weaponId, oldAttachment, newAttachments, slot);
        }

        public void ShowTip(ETipType tip)
        {
            entity.tip.TipType = ETipType.NoWeaponInSlot;
        }

        public bool CanUseGreande()
        {
            if (!entity.hasThrowingAction)
                return false;
            var pull = RelatedThrowAction.ActionInfo.IsPull;
            var destroy = RelatedThrowAction.ActionInfo.IsInterrupt;
            var fly = RelatedThrowUpdate.IsStartFly;
            return (!pull && !destroy && !fly);
        }
    }
}
