using App.Shared.Components.Player;
using Core.Appearance;
using Core.Attack;
using Core.CharacterBone;
using Core.CharacterState;
using Core.Event;
using Core.Free;
using Core.WeaponLogic.Throwing;
using WeaponConfigNs;

namespace App.Shared.GameModules
{

    /// <summary>
    /// Defines the <see cref="PlayerComponentsReference" />
    /// </summary>
    public  class PlayerComponentsReference:AbstractPlayerComponentsReference
    {
        public PlayerComponentsReference(PlayerEntity in_entity) : base(in_entity)
        {
        }

        public FirePosition RelatedFirePos
        {
            get { return entity.firePosition; }
        }

        public int RelatedTime
        {
            get { return entity.time.ClientTime; }
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
            set{ entity.meleeAttackInfo.AttackInfo = value; }
        }
        public MeleeFireLogicConfig RelatedMeleeAttackInfoCfg
        {
            set {  entity.meleeAttackInfo.AttackConfig = value; }
        }
        public ThrowingUpdateComponent RelatedThrowUpdate
        {
            get { return entity.throwingUpdate; }
        }

        public StatisticsDataComponent RelatedStatistics
        {
            get { return entity.statisticsData; }
        }

        public CameraStateNewComponent RelatedCameraSNew
        {
            get { return entity.cameraStateNew; }
        }

        public ThrowingActionInfo RelatedThrowAction
        {
            get { return entity.throwingAction.ActionInfo; }
        }

        public OrientationComponent RelatedOrient
        {
            get { return entity.orientation; }
        }

        public ICharacterFirstPersonAppearance RelatedFstAappearence
        {
            get { return entity.appearanceInterface.FirstPersonAppearance; }
        }
        public ICharacterAppearance RelatedAappearence
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

        public IFreeData RelatedFreeData
        {
            get { return entity.freeData.FreeData; }
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

    }
}
