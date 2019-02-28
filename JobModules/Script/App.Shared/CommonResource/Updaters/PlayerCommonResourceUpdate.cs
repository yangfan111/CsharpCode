using System.Collections.Generic;
using App.Shared.CommonResource.Base;
using App.Shared.Components.Player;
using App.Shared.GameModules.Common;
using App.Shared.GameModules.Player;
using App.Shared.GameModules.Player.Appearance;
using App.Shared.GameModules.Player.Appearance.WeaponControllerPackage;
using App.Shared.GameModules.Player.CharacterBone;
using App.Shared.GameModules.Player.ConcreteCharacterController;
using App.Shared.GameModules.Vehicle;
using App.Shared.GameModules.Weapon;
using Core.CharacterController;
using Core.CharacterController.ConcreteController;
using Core.CommonResource;
using Core.EntityComponent;
using Core.Utils;
using Entitas;
using Utils.AssetManager;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Shared.CommonResource.Updaters
{
    public class PlayerCommonResourceUpdate : AbstractCommonResourceLoadSystem<PlayerEntity>
    {
        private const int ResourceLength = (int) EPlayerCommonResourceType.End - 1;
        private readonly ICommonResourceActions[] _actions;
        private readonly PlayerContext _playerContext;

        public PlayerCommonResourceUpdate(Contexts contexts, Queue<RemoveAssetStatus> oldResource) : base(contexts, oldResource)
        {
            _playerContext = contexts.player;
            _actions = new ICommonResourceActions[ResourceLength];

            _actions[(int) EPlayerCommonResourceType.FirstPlayer] = new PlayerFirstAction();

            _actions[(int) EPlayerCommonResourceType.ThirdPlayer] = new PlayerThirdAction();

            _actions[(int) EPlayerCommonResourceType.HitBoxs] = new PlayerHitboxsAction();

            _actions[(int) EPlayerCommonResourceType.Parachute] = new CommonResourceActions(CommonResourceActions.DefaultCanInitFunc,
                CommonResourceActions.DefaultInitFunc, CommonResourceActions.DefaultRecycle, CommonResourceActions.DefaultOnLoadFailed);
            for (var i = (int) EPlayerCommonResourceType.WardrobeStart; i < (int) EPlayerCommonResourceType.LatestWeaponStateStart; i++)
                _actions[i] = new CommonResourceActions(CommonResourceActions.DefaultCanInitFunc,
                    CommonResourceActions.DefaultInitFunc, CommonResourceActions.DefaultRecycle, CommonResourceActions.DefaultOnLoadFailed);
        }

        protected override IGroup<PlayerEntity> GetInitializedGroup(IContexts contexts)
        {
            var c = contexts as Contexts;
            return c.player.GetGroup(PlayerMatcher.AllOf(PlayerMatcher.Position, PlayerMatcher.EntityKey, PlayerMatcher.PlayerResource));
        }

        protected override IGroup<PlayerEntity> GetUninitializedGroup(IContexts contexts)
        {
            var c = contexts as Contexts;
            return c.player.GetGroup(PlayerMatcher.AllOf(PlayerMatcher.Position, PlayerMatcher.EntityKey).NoneOf(PlayerMatcher.PlayerResource));
        }

        protected override ICommonResourceComponent GetCommonResource(PlayerEntity entity)
        {
            return entity.playerResource;
        }

        protected override PlayerEntity GetEntity(EntityKey key)
        {
            return _playerContext.GetEntityWithEntityKey(key);
        }

        protected override EntityKey GetEntityKey(PlayerEntity entity)
        {
            return entity.entityKey.Value;
        }

        protected override ICommonResourceActions GetActions(int index)
        {
            AssertUtility.Assert(index < ResourceLength);
            return _actions[index];
        }

        protected override int GetActionsLength()
        {
            return ResourceLength;
        }

        protected override void InitComponent(PlayerEntity player, IContexts contexts)
        {
            var character = DefaultGo.CreateGameObject(player.entityKey.ToString());
            character.layer = UnityLayerManager.GetLayerIndex(EUnityLayerName.Player);
            var cc = PlayerEntityUtility.InitCharacterController(character);
            var kcc = PlayerEntityUtility.InitKinematicCharacterMotor(character);
            var characterControllerContext = new CharacterControllerContext(
                new UnityCharacterController(cc),
                new ProneCharacterController(kcc,
                    new ProneController()),
                new DiveCharacterController(kcc,
                    new DiveController()),
                new SwimCharacterController(kcc,
                    new SwimController())
            );


            var curver = character.AddComponent<AirMoveCurve>();
            curver.AireMoveCurve = SingletonManager.Get<CharacterStateConfigManager>().AirMoveCurve;

            character.AddComponent<EntityReference>();
            character.GetComponent<EntityReference>().Init(player.entityAdapter);
            var comp = character.AddComponent<PlayerVehicleCollision>();
            comp.AllContext = contexts as Contexts;

            var appearanceManager = new AppearanceManager();


            var characterControllerManager = new CharacterControllerManager();
            characterControllerManager.SetCharacterController(characterControllerContext);


            var characterBone = new CharacterBoneManager();
            characterBone.SetWardrobeController(appearanceManager.GetWardrobeController());
            characterBone.SetWeaponController(appearanceManager.GetController<WeaponController>());
            var weaponController = appearanceManager.GetController<WeaponController>() as WeaponController;
            if (null != weaponController)
            {
                weaponController.SetWeaponChangedCallBack(characterBone.CurrentWeaponChanged);
                weaponController.SetCacheChangeAction(characterBone.CacheChangeCacheAction);
            }
            player.AddCharacterControllerInterface(characterControllerManager);
            player.AddAppearanceInterface(appearanceManager);
            player.AddCharacterContoller(characterControllerContext);
            player.AddCharacterBoneInterface(characterBone);
            player.AddPlayerGameState(PlayerLifeStateEnum.NullState);
            player.AddPlayerResource(character);
        }
    }
}