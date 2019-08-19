using System;
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
            public static readonly string ModeloffsetName = "ModelOffset";
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
                    AddThirdModelOffsetObject(character);
                    CharacterController cc = PlayerEntityUtility.InitCharacterController(character);
                    KinematicCharacterMotor kcc = PlayerEntityUtility.InitKinematicCharacterMotor(character);
                    CharacterControllerContext characterControllerContext = new CharacterControllerContext(
                        new UnityCharacterController(cc, InitActionWithOffset(), !player.isFlagSelf),
                        new Core.CharacterController.ConcreteController.ProneCharacterController(kcc,
                            new ProneController(),
                            InitActionWithNoOffset()),
                        new Core.CharacterController.ConcreteController.DiveCharacterController(kcc,
                            new DiveController(),InitActionWithNoOffset()),
                        new Core.CharacterController.ConcreteController.SwimCharacterController(kcc,
                            new SwimController(),InitActionWithNoOffset())
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
                    characterBone.SetWeaponController(appearanceManager.GetController<NewWeaponController>());
                    var weaponController = (NewWeaponController)appearanceManager.GetController<NewWeaponController>();
                    if (null != weaponController)
                    {
                        weaponController.SetWeaponChangedCallBack(characterBone.CurrentP1WeaponChanged, characterBone.CurrentP3WeaponChanged);
                        weaponController.SetWeaponOrAttachmentDeleteCallBack(characterBone.WeaponOrAttachmentDel);
                        weaponController.SetWeaponOrAttachementAddCallBack(characterBone.WeaponOrAttachmentAdd);
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

            public static Action<Transform> InitActionWithOffset()
            {
                return transform =>
                {
                    var thirdModelOffset = GetThirdModelParent(transform); 
                    AssertUtility.Assert(thirdModelOffset != transform);
                    thirdModelOffset.transform.localPosition = new Vector3(0, -PlayerEntityUtility.CcSkinWidth, 0);
                    thirdModelOffset.transform.localRotation = Quaternion.identity;
                    thirdModelOffset.transform.localScale = Vector3.one;
                };
            }

            public static Action<Transform> InitActionWithNoOffset()
            {
                return transform =>
                {
                    var thirdModelOffset = GetThirdModelParent(transform); 
                    AssertUtility.Assert(thirdModelOffset != transform);
                    thirdModelOffset.transform.localPosition = Vector3.zero;
                    thirdModelOffset.transform.localRotation = Quaternion.identity;
                    thirdModelOffset.transform.localScale = Vector3.one;
                };
            }

            /// <summary>
            /// 在RootGo和第三人称模型之间添加一个节点，在这个节点上给予偏移
            /// 原因：角色的CharacterController有一个SkinWidth，unity文档描述（The character's collision skin width.
            /// Specifies a skin around the character within which contacts will be generated by the physics engine. Use it to avoid numerical precision issues.
            /// This is dependant on the scale of the world, but should be a small, positive non zero value.）
            /// 大意就是与其他物体(比如地面)距离SkinWidth就认为是碰撞了。
            /// 目的：为了解决角色浮空在地面上（浮空的距离是这个SkinWidth的值，角色脚底刚好在CharacterController底部，CharacterController底部距离地面有SkinWidth的空隙）
            /// 之前的方案：把角色模型添加一个-SkinWidth的偏移，使得角色看上去在地面上。
            /// 改进方案：
            /// 添加这个节点的原因：很多时候对第三人称模型会有一个假定，即是单位矩阵，位移，旋转，尺度缩放都为0，由于任何人都可以修改第三人称模型的transform，可能其他维护者以单位矩阵为假定的，并修改它
            /// 之前的方案,角色的local位置不是0,0,0，而是0,-SkinWidth,0，导致这个位移被设置0,0,0为会出现角色浮空现象。
            /// 加入这一个节点好处
            /// 1.对其他用户是隐藏的，不需要知道这个偏移
            /// 2.对原来的逻辑没有影响
            /// 3.不需要知道有偏移这个假定
            /// </summary>
            /// <param name="character"></param>
            private static void AddThirdModelOffsetObject(GameObject character)
            {
                var thirdModelOffset = new GameObject(ModeloffsetName);
                thirdModelOffset.transform.SetParent(character.transform);
                thirdModelOffset.transform.localPosition = new Vector3(0, -PlayerEntityUtility.CcSkinWidth, 0);
                thirdModelOffset.transform.localRotation = Quaternion.identity;
                thirdModelOffset.transform.localScale = Vector3.one;
            }
            
            protected static Transform GetThirdModelParent(Transform root)
            {
                Transform parent = root;

                int childCount = root.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    var tmp = root.GetChild(i);
                    if (tmp.name == ModeloffsetName)
                    {
                        parent = tmp;
                    }
                }

                return parent;
            }
        }
}
