using App.Shared.Player;
using App.Shared.Util;
using Core;
using Core.Attack;
using Core.Common;
using Core.Utils;
using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon
{
    /// <summary>
    /// Defines the <see cref="PlayerComponentsReference" />
    /// </summary>
    public partial class WeaponPlayerComponentsAgent : PlayerWeaponComponentsReference
    {
        public WeaponPlayerComponentsAgent(PlayerEntity in_entity) : base(in_entity)
        {
        }

        public void UnArmC4()
        {
            RelatedAappearence.UnmoutC4(entity.GetSex());
        }

        public void CreateSetMeleeAttackInfoSync(int in_Sync)
        {
            if (entity.hasMeleeAttackInfoSync)
            {
                RelatedMeleeAttackInfoSync.AttackTime = in_Sync;
                RelatedMeleeAttackInfoSync.BeforeAttackTime = RelatedTime;
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
                RelatedMeleeAttackInfo = attackInfo;
                RelatedMeleeAttackInfoCfg = config;
            }
            else
            {
                entity.AddMeleeAttackInfo();
                RelatedMeleeAttackInfo = attackInfo;
                RelatedMeleeAttackInfoCfg = config;
            }
        }

        public void CharacterSwitchWeapon(System.Action unarmCallback, System.Action drawCallback, float switchParam)
        {
            RelatedCharState.SwitchWeapon(unarmCallback, drawCallback, switchParam);
        }

        public void CharacterDraw(System.Action drawCallback, float drawParam)
        {
            RelatedCharState.Draw(drawCallback, drawParam);
        }

        public void CharacterDrawInterrupt()
        {
            PlayerStateUtil.AddPlayerState(EPlayerGameState.InterruptItem, entity.gamePlay);
            RelatedCharState.InterruptAction();
            RelatedCharState.ForceFinishGrenadeThrow();
        }

        public void CharacterUnarm(System.Action unarm, float unarmParam)
        {
            RelatedCharState.InterruptAction();
            RelatedCharState.ForceFinishGrenadeThrow();
            RelatedCharState.Unarm(unarm, unarmParam);
        }

        public void CharacterInterrupt()
        {
            RelatedCharState.InterruptAction();
            RelatedCharState.InterruptSwitchWeapon();
            PlayerStateUtil.AddPlayerState(EPlayerGameState.InterruptItem, entity.gamePlay);
            RelatedCharState.ForceBreakSpecialReload(null);
            RelatedCharState.ForceFinishGrenadeThrow();
            if (entity.hasThrowingAction)
            {
                RelatedThrowAction.ClearState();
            }
        }



        public void ThrowActionExecute()
        {
            if (!entity.hasThrowingAction) return;
            Core.WeaponLogic.Throwing.ThrowingActionInfo actionInfo = RelatedThrowAction;
            if (actionInfo.IsReady && actionInfo.IsPull)
            {
                //若已拉栓，销毁ThrowingEntity
                actionInfo.IsInterrupt = true;
            }
            //打断投掷动作
            RelatedCharState.ForceFinishGrenadeThrow();
            //清理手雷状态
            actionInfo.ClearState();
        }

        public void ApperanceRefreshABreath(float breath)
        {
            //TODO 动态获取
            RelatedFstAappearence.SightShift.SetAttachmentFactor(breath);
        }

        public void ModelRefreshWeaponModel(int weaponId, EWeaponSlotType slot, WeaponPartsStruct attachments)
        {
        }


        public void ShowTip(ETipType tip)
        {
            entity.tip.TipType = tip;
        }

        public bool CanUseGreande
        {
            get
            {
                if (!entity.hasThrowingAction)
                    return false;
                var pull = RelatedThrowAction.IsPull;
                var destroy = RelatedThrowAction.IsInterrupt;
                DebugUtil.MyLog("destroy :" + destroy + " pull:" + pull, DebugUtil.DebugColor.Default);
                return (!pull && !destroy);
            }
        }

    }
}
