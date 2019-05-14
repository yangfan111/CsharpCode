
using App.Shared.Components.Player;
using App.Shared.Components.Weapon;
using App.Shared.Player;
using Core;
using Core.Appearance;
using Core.Attack;
using Core.CharacterBone;
using Core.CharacterState;
using Core.Common;
using Core.EntityComponent;
using Core.Event;
using Core.Free;
using Core.Statistics;
using Core.WeaponLogic.Throwing;
using System;
using System.Collections.Generic;
using App.Server.GameModules.GamePlay.free.player;
using App.Shared.GameModules.Player;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared.GameModules.Weapon
{

    /// <summary>
    /// Defines the <see cref="PlayerComponentsReference" />
    /// </summary>
    public partial class PlayerWeaponController
    {
        private PlayerEntity entity;

        private void SetEnity(PlayerEntity in_entity) 
        {
            entity = in_entity;
            Owner = entity.entityKey.Value;
        }
        #region//reference getter
        public FirePosition RelatedFirePos
        {
            get { return entity.firePosition; }
        }

        public int JobAttribute
        {
            get{ if (entity.hasPlayerInfo) return entity.playerInfo.JobAttribute; return 0; }
        }

        public int RelatedTime
        {
            get { return entity.time.ClientTime; }
        }
       
        public GamePlayComponent RelatedGamePlay
        {
            get { return entity.gamePlay; }
        }
        public CameraFinalOutputNewComponent RelatedCameraFinal
        {
            get { return entity.cameraFinalOutputNew; }
        }

        public ICharacterState RelatedCharState
        {
            get { return entity.stateInterface.State; }
        }

        public MeleeAttackInfoSyncComponent RelatedMeleeAttackInfoSync
        {
            get { return entity.meleeAttackInfoSync; }
        }

        public MeleeAttackInfo RelatedMeleeAttackInfo
        {
            get { return entity.meleeAttackInfo.AttackInfo; }
            set { entity.meleeAttackInfo.AttackInfo = value; }
        }
        public MeleeFireLogicConfig RelatedMeleeAttackInfoCfg
        {
            set { entity.meleeAttackInfo.AttackConfig = value; }
        }
        public ThrowingUpdateComponent RelatedThrowUpdate
        {
            get { return entity.throwingUpdate; }
        }

        public StatisticsData RelatedStatisticsData
        {
            get { return entity.statisticsData.Statistics; }
        }

        public CameraStateNewComponent RelatedCameraSNew
        {
            get { return entity.cameraStateNew; }
        }

        public ThrowingActionInfo RelatedThrowAction
        {
            get
            {
                if (entity.hasThrowingAction)
                    return entity.throwingAction.ActionInfo;
                return null;
            }
        }

        public OrientationComponent RelatedOrientation
        {
            get { return entity.orientation; }
        }

        public ICharacterFirstPersonAppearance RelatedFstAappearence
        {
            get { return entity.appearanceInterface.FirstPersonAppearance; }
        }
        public ICharacterAppearance RelatedAppearence
        {
            get { return entity.appearanceInterface.Appearance; }
        }

        public PlayerInfoComponent RelatedPlayerInfo
        {
            get { return entity.playerInfo; }
        }

        public PlayerMoveComponent RelatedPlayerMove
        {
            get { return entity.playerMove; }
        }

        public FreeData RelatedFreeData
        {
            get { return entity.freeData.FreeData as FreeData; }
        }

        public PlayerEvents RelatedLocalEvents
        {
            get { return entity.localEvents.Events; }
        }

        public PlayerWeaponAmmunitionComponent RelatedAmmunition
        {
            get { return entity.playerWeaponAmmunition; }
        }

        public ICharacterBone RelatedBones
        {
            get { return entity.characterBoneInterface.CharacterBone; }
        }
        public PlayerWeaponBagSetComponent RelatedBagSet
        {
            get { return entity.playerWeaponBagSet; }
        }
        public PlayerWeaponServerUpdateComponent RelatedServerUpdate
        {
            get { return entity.playerWeaponServerUpdate; }
        }
        public PlayerWeaponAuxiliaryComponent RelatedWeaponAux
        {
            get
            {
                if (!entity.hasPlayerWeaponAuxiliary)
                    entity.AddPlayerWeaponAuxiliary();
                return entity.playerWeaponAuxiliary;
            }
        }

        public PlayerWeaponCustomizeComponent RelatedCustomize
        {
            get { return entity.playerWeaponCustomize; }
        }
        public GrenadeCacheDataComponent RelatedGrenadeCache
        {
            get { return entity.grenadeCacheData; }
        }
        public PlayerClientUpdateComponent RelatedClientUpdate
        {
            get
            {
                if(!entity.hasPlayerClientUpdate)
                    entity.AddPlayerClientUpdate();
                return entity.playerClientUpdate;
            }
        }
        #endregion

        #region//reference modify wrapper
//        public int? AutoFire
//        {
//            get
//            {
//
//                if (RelatedWeaponAux.HasAutoAction)
//                    return RelatedWeaponAux.AutoFire;
//                return null;
//            }
//            set { RelatedWeaponAux.AutoFire = value.Value; }
//        }
        public bool? AutoThrowing
        {
            get
            {
                if (RelatedWeaponAux.HasAutoAction)
                    return RelatedWeaponAux.AutoThrowing;
                return null;
            }
            set { RelatedWeaponAux.AutoThrowing = value.Value; }
        }
        public void AddAuxBullet(PlayerBulletData bulletData)
        {
            if (RelatedWeaponAux.BulletList == null) return;
            RelatedWeaponAux.BulletList.Add(bulletData);
        }
        public void AddAuxEffect()
        {
            RelatedWeaponAux.ClientInitialize = true;
            RelatedWeaponAux.EffectList = new List<EClientEffectType>();
        }
        public void AddAuxBullet()
        {
            RelatedWeaponAux.BulletList = new List<PlayerBulletData>();
        }

        public void AddAuxEffect(EClientEffectType effectType)
        {
            if (RelatedWeaponAux.EffectList != null)
                RelatedWeaponAux.EffectList.Add(effectType);
        }
        public void RemoveBagWeapon(EWeaponSlotType slot)
        {
            var slotData = RelatedBagSet[0][slot];
            slotData.Remove(RelatedCustomize.EmptyConstWeaponkey);//player slot 数据移除
        }

        public void ClearBagPointer()
        {
            RelatedBagSet.ClearPointer();
        }

        public void SyncBagWeapon(EWeaponSlotType slot, EntityKey key)
        {
            RelatedBagSet[0][slot].Sync(key);
        }

        public void SetBagHeldSlotType(EWeaponSlotType nowSlot)
        {
            var slot = (int)nowSlot;
            var bag = RelatedBagSet[0];
            if (bag.HeldSlotPointer == slot)
                return;
            if (!WeaponUtil.VertifyEweaponSlotIndex(slot, true))
                return;
            bag.ChangeSlotPointer(slot);
        }

        public Func<EntityKey> GenerateBagWeaponKeyExtractor(EWeaponSlotType slotType)
        {
            return () => { return RelatedBagSet[0][slotType].WeaponKey; };
        }

        public Func<EntityKey> GenerateBagEmptyKeyExtractor()
        {
            return () => { return RelatedCustomize.EmptyConstWeaponkey; };
        }
        public void UnArmC4()
        {
            RelatedAppearence.UnmoutC4(entity.GetSex());
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
            RelatedCharState.Select(drawCallback, drawParam);
        }

        public void CharacterDrawInterrupt()
        {
            PlayerStateUtil.AddPlayerState(EPlayerGameState.InterruptItem, entity.gamePlay);
            RelatedCharState.InterruptAction();
            RelatedCharState.ForceFinishGrenadeThrow();
        }

        public void CharacterUnarm(System.Action holsterStartFinished, System.Action holsterEndFinished, float unarmParam)
        {
            RelatedCharState.InterruptAction();
            RelatedCharState.ForceFinishGrenadeThrow();
            RelatedCharState.Holster(holsterStartFinished, holsterEndFinished, unarmParam);
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

        public bool CanUseGrenade
        {
            get
            {
                if (!entity.hasThrowingAction)
                    return false;
                var pull = RelatedThrowAction.IsPull;
                var destroy = RelatedThrowAction.IsInterrupt;
                //  DebugUtil.MyLog("destroy :" + destroy + " pull:" + pull, DebugUtil.DebugColor.Default);
                return (!pull && !destroy);
            }
        }



        #endregion
    }
}
