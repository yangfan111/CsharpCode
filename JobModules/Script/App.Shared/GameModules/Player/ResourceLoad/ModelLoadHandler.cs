using App.Shared.Components.Player;
using App.Shared.GameModules.Common;
using App.Shared.GameModules.Player.Appearance;
using App.Shared.GameModules.Player.Appearance.WeaponControllerPackage;
using App.Shared.GameModules.Player.CharacterBone;
using App.Shared.GameModules.Player.ConcreteCharacterController;
using App.Shared.GameModules.Vehicle;
using Core.CharacterController;
using Core.Utils;
using KinematicCharacterController;
using UnityEngine;
using Utils.AssetManager;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Shared.GameModules.Player.ResourceLoad
{
    public class ModelLoadHandler
        {
            protected static LoggerAdapter Logger = new LoggerAdapter(typeof(ModelLoadHandler));
            private Contexts _contexts;

            protected ModelLoadHandler(Contexts contexts)
            {
                _contexts = contexts;
            }

            protected void HandleLoadedModel(PlayerEntity player, GameObject obj)
            {
                obj.layer = UnityLayerManager.GetLayerIndex(EUnityLayerName.Player);
                PlayerEntityUtility.DisableCollider(obj.transform);

                if (!player.hasCharacterContoller)
                {
                    var character = DefaultGo.CreateGameObject(player.entityKey.ToString());
                    character.layer = UnityLayerManager.GetLayerIndex(EUnityLayerName.Player);
                    CharacterController cc = PlayerEntityUtility.InitCharacterController(character);
                    KinematicCharacterMotor kcc = PlayerEntityUtility.InitKinematicCharacterMotor(character);
                    CharacterControllerContext characterControllerContext = new CharacterControllerContext(
                        new UnityCharacterController(cc, !player.isFlagSelf),
                        new Core.CharacterController.ConcreteController.ProneCharacterController(kcc,
                            new ProneController()),
                        new Core.CharacterController.ConcreteController.DiveCharacterController(kcc,
                            new DiveController()),
                        new Core.CharacterController.ConcreteController.SwimCharacterController(kcc,
                            new SwimController())
                    );


                    var curver = character.AddComponent<AirMoveCurve>();
                    curver.AireMoveCurve = SingletonManager.Get<CharacterStateConfigManager>().AirMoveCurve;
                    curver.MovementCurve = SingletonManager.Get<CharacterStateConfigManager>().MovementCurve;
                    curver.PostureCurve = SingletonManager.Get<CharacterStateConfigManager>().PostureCurve;
                    if (character.GetComponent<EntityReference>() == null)
                        character.AddComponentUncheckRequireAndDisallowMulti<EntityReference>();
                    character.GetComponent<EntityReference>().Init(player.entityAdapter);
                    var comp = character.AddComponent<PlayerVehicleCollision>();
                    comp.AllContext = _contexts;

                    var appearanceManager = new AppearanceManager();


                    var characterControllerManager = new CharacterControllerManager();
                    characterControllerManager.SetCharacterController(characterControllerContext);


                    var characterBone = new CharacterBoneManager();
                    characterBone.SetWardrobeController(appearanceManager.GetWardrobeController());
                    characterBone.SetWeaponController(appearanceManager.GetController<WeaponController>());
                    var weaponController = (WeaponController)appearanceManager.GetController<WeaponController>();
                    if (null != weaponController)
                    {
                        weaponController.SetWeaponChangedCallBack(characterBone.CurrentWeaponChanged);
                        weaponController.SetCacheChangeAction(characterBone.CacheChangeCacheAction);
                    }

                    player.AddCharacterControllerInterface(characterControllerManager);
                    player.AddAppearanceInterface(appearanceManager);
                    player.AddCharacterContoller(characterControllerContext);
                    player.AddCharacterBoneInterface(characterBone);
                    player.AddRecycleableAsset(character);
                    player.AddPlayerGameState(PlayerLifeStateEnum.NullState);
                }
            }
        }
}
