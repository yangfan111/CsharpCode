using App.Shared.Components.Player;

namespace App.Shared.GameModules.Weapon
{
    /// <summary>
    /// Defines the <see cref="PlayerEntityWeaponInteract" />
    /// </summary>
    public partial class PlayerEntityWeaponInteract
    {
        protected PlayerEntity entity;

        private bool IsInitialized
        {
            get { return entity != null; }
        }

        public FirePosition RelatedFirePos
        {
            get { return entity.firePosition; }
        }

        public TimeComponent RelatedTime
        {
            get { return entity.time; }
        }

        public CameraFinalOutputNewComponent RelatedCameraFinal
        {
            get { return entity.cameraFinalOutputNew; }
        }

        public StateInterfaceComponent RelatedCharState
        {
            get { return entity.stateInterface; }
        }

        public MeleeAttackInfoSyncComponent RelatedMeleeAttackInfoSync
        {
            get { return entity.meleeAttackInfoSync; }
        }

        public MeleeAttackInfoComponent RelatedMeleeAttackInfo
        {
            get { return entity.meleeAttackInfo; }
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

        public ThrowingActionComponent RelatedThrowAction
        {
            get { return entity.throwingAction; }
        }

        public OrientationComponent RelatedOrient
        {
            get { return entity.orientation; }
        }

        public AppearanceInterfaceComponent RelatedAppearence
        {
            get { return entity.appearanceInterface; }
        }

        public PlayerInfoComponent RelatedPlayerInfo
        {
            get { return entity.playerInfo; }
        }

        public PlayerMoveComponent RelatedPlayerMove
        {
            get { return entity.playerMove; }
        }

        public FreeDataComponent RelatedFreeData
        {
            get { return entity.freeData; }
        }

        public LocalEventsComponent RelatedLocalEvents
        {
            get { return entity.localEvents; }
        }

        public ModeLogicComponent RelatedModelLogic
        {
            get { return entity.modeLogic; }
        }

        public PlayerWeaponAmmunitionComponent RelatedAmmunition
        {
            get { return entity.playerWeaponAmmunition; }
        }

        public CharacterBoneInterfaceComponent RelatedBones
        {
            get { return entity.characterBoneInterface; }
        }

        public PlayerEntityWeaponInteract(PlayerWeaponController in_weaponController, PlayerEntity in_entity)
        {
            entity = in_entity;
        }
    }
}
