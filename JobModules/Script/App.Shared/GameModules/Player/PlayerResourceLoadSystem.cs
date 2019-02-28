using System;
using System.Collections.Generic;
using App.Shared.Components.Player;
using App.Shared.GameModules.Common;
using App.Shared.GameModules.HitBox;
using App.Shared.GameModules.Player.Appearance;
using App.Shared.GameModules.Vehicle;
using Core.GameModule.System;
using Core.Utils;
using Core.CharacterState;
using Entitas;
using UnityEngine;
using Object = UnityEngine.Object;
using App.Shared.GameModules.Player.CharacterState;
using BehaviorDesigner.Runtime;
using Core.Animation;
using Core.HitBox;
using Utils.Appearance;
using Core.WeaponAnimation;
using App.Shared.GameModules.Player.Appearance.AnimationEvent;
using App.Shared.GameModules.Player.Appearance.WeaponControllerPackage;
using App.Shared.GameModules.Player.ConcreteCharacterController;
using App.Shared.Player;
using Core.CharacterController;
using Entitas.VisualDebugging.Unity;
using KinematicCharacterController;
using Utils.AssetManager;
using App.Shared.GameModules.Player.CharacterBone;
using Utils.Configuration;
using Utils.Singleton;
using App.Shared.GameModules.Weapon;

namespace App.Shared.GameModules.Player
{
    public class PlayerResourceLoadSystem : ReactiveResourceLoadSystem<PlayerEntity>
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerResourceLoadSystem));
        private readonly PlayerContext _player;
        private readonly FirstPersonModelLoadHandler _p1Handler;
        private readonly ThirdPersonModelLoadHandler _p3Handler;
        private readonly InterceptPool _interceptPool = new InterceptPool();
        
        public PlayerResourceLoadSystem(Contexts contexts) : base(contexts.player)
        {
            _player = contexts.player;
            _p1Handler = new FirstPersonModelLoadHandler(contexts);
            _p3Handler = new ThirdPersonModelLoadHandler(contexts);
        }

        protected override ICollector<PlayerEntity> GetTrigger(IContext<PlayerEntity> context)
        {
            return context.CreateCollector(PlayerMatcher.Position.Added());
        }

        protected override bool Filter(PlayerEntity entity)
        {
            return !entity.hasThirdPersonModel && !entity.hasFirstPersonModel ;
        }

        public override void SingleExecute(PlayerEntity player)
        {
            AssetManager.LoadAssetAsync(
                player,
                AssetConfig.GetCharacterModelAssetInfo(player.playerInfo.ModelName),
                _p3Handler.OnLoadSucc);
            
            if (player.isFlagSelf)
            {
                AssetManager.LoadAssetAsync(
                    player,
                    AssetConfig.GetCharacterHandAssetInfo(player.playerInfo.ModelName),
                    _p1Handler.OnLoadSucc);
            }
                
            _logger.InfoFormat("created client player entity {0}", player.entityKey);               
        }

        public override void OnLoadResources(IUnityAssetManager assetManager)
        {
            base.OnLoadResources(assetManager);
            foreach (var entity in _player.GetEntities())
            {
                if (entity.hasAppearanceInterface)
                {
                    var loadRequests = entity.appearanceInterface.Appearance.GetLoadRequests();
                    foreach (var request in loadRequests)
                    {
                        var intercept = _interceptPool.Get();
                        intercept.SetParam(entity, request.GetHandler<PlayerEntity>());
                        assetManager.LoadAssetAsync(entity, request.AssetInfo, intercept.Call);
                    }

                    var recycleRequests = entity.appearanceInterface.Appearance.GetRecycleRequests();
                    foreach (var request in recycleRequests)
                    {
                        entity.RemoveAsset(request);
                        assetManager.Recycle(request);
                    } 

                    entity.appearanceInterface.Appearance.ClearRequests();
                }
            }
        }

        public class ModelLoadHandler
        {
            private PlayerContext _playerContext;
            private Contexts _contexts;

            public ModelLoadHandler(Contexts contexts)
            {
                _playerContext = contexts.player;
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
                        new UnityCharacterController(cc),
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

                    character.AddComponent<EntityReference>();
                    character.GetComponent<EntityReference>().Init(player.entityAdapter);
                    var comp = character.AddComponent<PlayerVehicleCollision>();
                    comp.AllContext = _contexts;

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
                    player.AddRecycleableAsset(character);
                    player.AddPlayerGameState(PlayerLifeStateEnum.NullState);
                }
            }
        }

        public class FirstPersonModelLoadHandler : ModelLoadHandler
        {
            public FirstPersonModelLoadHandler(Contexts contexts): base(contexts)
            {
                
            }

            public void OnLoadSucc(PlayerEntity player, UnityObject unityObj)
            {
                GameObject go = unityObj;

                HandleLoadedModel(player, go);
                
                player.AddAsset(unityObj);

                player.AddFirstPersonModel(go);

                player.appearanceInterface.FirstPersonAppearance = new FirstPersonAppearanceManager(player.firstPersonAppearance);

                go.name = "P1_" + player.entityKey;
                go.transform.SetParent(player.RootGo().transform);
                go.transform.localPosition = new Vector3(0, player.firstPersonAppearance.FirstPersonHeight, player.firstPersonAppearance.FirstPersonForwardOffset);
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
                _logger.InfoFormat("P1 loaded: {0}", player.entityKey);

                player.AddFirstPersonAnimator(go.GetComponent<Animator>());

                var ik = go.AddComponent<PlayerIK>();
                ik.SetAnimator(AvatarIKGoal.LeftHand, player.firstPersonAnimator.UnityAnimator);
                ik.SetIKLayer(AvatarIKGoal.LeftHand, NetworkAnimatorLayer.FirstPersonIKPassLayer);
                ik.SetAnimator(AvatarIKGoal.RightHand, player.firstPersonAnimator.UnityAnimator);
                ik.SetIKLayer(AvatarIKGoal.RightHand, NetworkAnimatorLayer.FirstPersonIKPassLayer);

                BoneTool.CacheTransform(go);                

                if (player.isFlagSelf)
                {
                    var animationEvent = go.AddComponent<AnimationClipEvent>();
                    animationEvent.Player = player;
                    player.animatorClip.ClipManager.SetAnimationCleanEventCallback(animationEvent.InterruptAnimationEventFunc);
                }
                else
                {
                    go.AddComponent<ThirdPersonAnimationClipEvent>();
                }

                player.firstPersonAnimator.UnityAnimator.Update(0);

                player.appearanceInterface.Appearance.SetFirstPersonCharacter(go);
                player.appearanceInterface.FirstPersonAppearance.SetFirstPersonCharacter(go);

                player.appearanceInterface.Appearance.SetAnimatorP1(player.firstPersonAnimator.UnityAnimator);

                player.stateInterface.State.SetName(player.RootGo().name);

                player.characterBoneInterface.CharacterBone.SetFirstPersonCharacter(go);

                if (!player.hasFpAnimStatus)
                    player.AddFpAnimStatus(NetworkAnimatorUtil.CreateAnimatorLayers(player.firstPersonAnimator.UnityAnimator),
                                           NetworkAnimatorUtil.GetAnimatorParams(player.firstPersonAnimator.UnityAnimator));
                
                // 禁用非可见状态下的动画更新
                if (!player.isFlagSelf)
                    player.firstPersonAnimator.UnityAnimator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
                else
                    player.firstPersonAnimator.UnityAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            }
        }

        public class ThirdPersonModelLoadHandler : ModelLoadHandler
        {
            private IUnityAssetManager _assetManager;
            public ThirdPersonModelLoadHandler(Contexts contexts) : base(contexts)
            {
                _assetManager = contexts.session.commonSession.AssetManager;
            }

            public void OnLoadSucc(PlayerEntity player, UnityObject unityObj)
            {
                GameObject go = unityObj;

                RemoveRagdollOnServerSide(go);
                
                HandleLoadedModel(player, go);
                
                player.AddAsset(unityObj);

                player.AddThirdPersonModel(go);

                go.name = go.name.Replace("(Clone)", "");
                go.transform.SetParent(player.RootGo().transform);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
                _logger.InfoFormat("P3 loaded: {0}", player.entityKey);

                BoneTool.CacheTransform(go);

                if (!player.hasBones)
                {
                    player.AddBones(null, null, null);
                }

                player.bones.Head = BoneMount.FindChildBoneFromCache(go, BoneName.CharacterHeadBoneName);
                player.bones.Spine = BoneMount.FindChildBoneFromCache(go, BoneName.CharacterSpineName);

                player.AddThirdPersonAnimator(go.GetComponent<Animator>());

                var ik = go.AddComponent<PlayerIK>();
                ik.SetAnimator(AvatarIKGoal.LeftHand, player.thirdPersonAnimator.UnityAnimator);
                ik.SetIKLayer(AvatarIKGoal.LeftHand, NetworkAnimatorLayer.ThirdPersonIKPassLayer);
                ik.SetAnimator(AvatarIKGoal.RightHand, player.thirdPersonAnimator.UnityAnimator);
                ik.SetIKLayer(AvatarIKGoal.RightHand, NetworkAnimatorLayer.ThirdPersonIKPassLayer);

                if (player.isFlagSelf)
                {
                    var animationEvent = go.AddComponent<AnimationClipEvent>();
                    animationEvent.Player = player;
                    player.animatorClip.ClipManager.SetAnimationCleanEventCallback(animationEvent.InterruptAnimationEventFunc);
                    // 设置大厅传入的roleId和avatarId
                    player.appearanceInterface.Appearance.SetRoleModelIdAndInitAvatar(player.playerInfo.RoleModelId, player.playerInfo.AvatarIds);
                }
                else
                {
                    go.AddComponent<ThirdPersonAnimationClipEvent>();
                }
                
                player.characterControllerInterface.CharacterController.SetCharacterRoot(player.characterContoller.Value.gameObject);
                player.appearanceInterface.Appearance.SetThirdPersonCharacter(go);
				player.thirdPersonAnimator.UnityAnimator.Update(0);

                player.characterBoneInterface.CharacterBone.SetCharacterRoot(player.characterContoller.Value.gameObject);
                player.characterBoneInterface.CharacterBone.SetThirdPersonCharacter(go);
                player.characterBoneInterface.CharacterBone.SetStablePelvisRotation();

                player.appearanceInterface.Appearance.SetAnimatorP3(player.thirdPersonAnimator.UnityAnimator);

                player.appearanceInterface.Appearance.PlayerReborn();
                player.characterControllerInterface.CharacterController.PlayerReborn();
                
                // 实际应该使用SharedConfig.IsServer，但离线模式中其值为false
                if (!player.hasNetworkAnimator)
                {
                    player.AddNetworkAnimator(NetworkAnimatorUtil.CreateAnimatorLayers(player.thirdPersonAnimator.UnityAnimator),
                                              NetworkAnimatorUtil.GetAnimatorParams(player.thirdPersonAnimator.UnityAnimator));
                    
                    player.networkAnimator.SetEntityName(player.entityKey.ToString());
                }

                if (!player.hasOverrideNetworkAnimator)
                {
                    player.AddOverrideNetworkAnimator();
                }

                if (SharedConfig.IsServer)
                {
                    player.AddNetworkAnimatiorServerTime(0);
                }
                
                // 禁用非可见状态下的动画更新，在获取Stable状态之后
                if (SharedConfig.IsServer || !player.isFlagSelf)
                    player.thirdPersonAnimator.UnityAnimator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
                else
                    player.thirdPersonAnimator.UnityAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

                _assetManager.LoadAssetAsync(player, AssetConfig.GetCharacterHitboxAssetInfo(player.playerInfo.ModelName), new HitboxLoadResponseHandler().OnLoadSucc);
            }

            private void RemoveRagdollOnServerSide(GameObject go)
            {
                if (SharedConfig.IsServer)
                {
                    foreach (var joint in go.GetComponentsInChildren<CharacterJoint>())
                    {
                        GameObject.Destroy(joint);
                    }

                    foreach (var body in go.GetComponentsInChildren<Rigidbody>())
                    {
                        GameObject.Destroy(body);
                    }

                    foreach (var collider in go.GetComponentsInChildren<BoxCollider>())
                    {
                        GameObject.Destroy(collider);
                    }

                    foreach (var collider in go.GetComponentsInChildren<SphereCollider>())
                    {
                        GameObject.Destroy(collider);
                    }

                    foreach (var collider in go.GetComponentsInChildren<CapsuleCollider>())
                    {
                        GameObject.Destroy(collider);
                    }
                }
            }
        }

        public class HitboxLoadResponseHandler
        {
            public void OnLoadSucc(PlayerEntity playerEntity, UnityObject unityObject)
            {
                var hitboxConfig = unityObject;
                HitBoxComponentUtility.InitHitBoxComponent(playerEntity.entityKey.Value, playerEntity, hitboxConfig);
            }
        }

        public class ParachuteLoadResponseHandler
        {
            public void OnLoadSucc(PlayerEntity player, UnityObject unityObj)
            {
                if (player.isFlagDestroy)
                {
                    return;
                }

                var transform =  unityObj.AsGameObject.transform;
                const string anchorName = "Driver_Seat";
                var anchor = transform.FindChildRecursively(anchorName);
                if (anchor == null)
                {
                    throw new Exception(String.Format("Can not find anchor with name {0} for parachute!", anchorName));
                }

                player.playerSkyMove.IsParachuteLoading = false;
                player.playerSkyMove.Parachute = transform;
                player.playerSkyMove.ParachuteAnchor = anchor;
            }
        }

        class LoadResourceIntercept
        {
            private readonly InterceptPool _pool;

            private PlayerEntity _player;
            private Action<PlayerEntity, UnityObject> _handler;

            public LoadResourceIntercept(InterceptPool pool)
            {
                _pool = pool;
            }

            public void SetParam(PlayerEntity player, Action<PlayerEntity, UnityObject> handler)
            {
                _player = player;
                _handler = handler;
            }

            public void Call(PlayerEntity player, UnityObject unityObj)
            {
                _player.AddAsset(unityObj);
                _handler(player, unityObj);
                _pool.Free(this);
            }
        }

        class InterceptPool
        {
            private Queue<LoadResourceIntercept> _pool = new Queue<LoadResourceIntercept>();
            
            public LoadResourceIntercept Get()
            {
                if (_pool.Count <= 0)
                    return new LoadResourceIntercept(this);

                return _pool.Dequeue();
            }

            public void Free(LoadResourceIntercept item)
            {
                _pool.Enqueue(item);
            }
        }
    }
}

