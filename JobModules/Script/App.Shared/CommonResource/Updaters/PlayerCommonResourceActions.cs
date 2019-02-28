using System;
using App.Shared.CommonResource.Base;
using App.Shared.Components.Player;
using App.Shared.GameModules.HitBox;
using App.Shared.GameModules.Player;
using App.Shared.GameModules.Player.Appearance;
using App.Shared.GameModules.Player.Appearance.AnimationEvent;
using App.Shared.Player;
using Core.Animation;
using Core.CommonResource;
using Core.HitBox;
using Core.Utils;
using Entitas;
using Shared.Scripts;
using UnityEngine;
using Utils.Appearance;
using Object = UnityEngine.Object;

namespace App.Shared.CommonResource.Updaters
{
    public static class PlayerCommonResourceExtensions
    {
        public static int GetCommonResourceIndex(this Wardrobe wardrobe)
        {
            return (int) EPlayerCommonResourceType.WardrobeStart + (int) wardrobe - 1;
        }

        public static int GetCommonResourceIndex(this LatestWeaponStateIndex latestWeaponState)
        {
            return (int) EPlayerCommonResourceType.LatestWeaponStateStart + (int) latestWeaponState - 1;
        }

        public static bool IsThirdAssetLoaded(this PlayerEntity player)
        {
            return player.playerResource.GetResource(EPlayerCommonResourceType.ThirdPlayer).Status == EAssetLoadStatus.Loaded;
        }
    }

    public class PlayerFirstAction : ICommonResourceActions
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerFirstAction));

        public void OnLoadFailed(IEntity entity, AssetStatus status)
        {
            var player = entity as PlayerEntity;
            if (player != null) Logger.InfoFormat("P1 LoadFailed: {0}", player.entityKey);
        }

        public void Recycle(AssetStatus status)
        {
            if (status.Object.AsGameObject !=null) status.Object.AsGameObject.transform.SetParent(null, false);
        }

        public void Init(IEntity entity, AssetStatus status)
        {
            var player = entity as PlayerEntity;
            var obj = status.Object.AsGameObject;
            obj.layer = UnityLayerManager.GetLayerIndex(EUnityLayerName.Player);
            PlayerEntityUtility.DisableCollider(obj.transform);
            var go = obj;


            player.AddFirstPersonModel(go);

            player.appearanceInterface.FirstPersonAppearance = new FirstPersonAppearanceManager(player.firstPersonAppearance);

            go.name = "P1_" + player.entityKey;
            go.transform.SetParent(player.playerResource.Root.transform);
            go.transform.localPosition = new Vector3(0, player.firstPersonAppearance.FirstPersonHeight, player.firstPersonAppearance.FirstPersonForwardOffset);
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            Logger.InfoFormat("P1 loaded: {0}", player.entityKey);

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

        public bool CanInit(IEntity entity, AssetStatus status)
        {
            var player = entity as PlayerEntity;
            return player.IsThirdAssetLoaded();
        }
    }

    public class PlayerThirdAction : ICommonResourceActions
    {
        private static readonly LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerFirstAction));

        public void OnLoadFailed(IEntity entity, AssetStatus status)
        {
            var player = entity as PlayerEntity;
            _logger.InfoFormat("P1 LoadFailed: {0}", player.entityKey);
        }

        public void Recycle(AssetStatus status)
        {
            if (status.Object.AsGameObject!=null) status.Object.AsGameObject.transform.SetParent(null, false);
        }

        public void Init(IEntity entity, AssetStatus status)
        {
            var go = status.Object.AsGameObject;
            var player = entity as PlayerEntity;

            RemoveRagdollOnServerSide(go);


            player.AddThirdPersonModel(go);

            go.name = go.name.Replace("(Clone)", "");
            go.transform.SetParent(player.RootGo().transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            _logger.InfoFormat("P3 loaded: {0}", player.entityKey);

           
            BoneTool.CacheTransform(go);

            if (!player.hasBones) player.AddBones(null, null, null);

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

            if (SharedConfig.IsServer) player.AddNetworkAnimatiorServerTime(0);

            // 禁用非可见状态下的动画更新，在获取Stable状态之后
            if (SharedConfig.IsServer || !player.isFlagSelf)
                player.thirdPersonAnimator.UnityAnimator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
            else
                player.thirdPersonAnimator.UnityAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        }

        public bool CanInit(IEntity entity, AssetStatus status)
        {
            return true;
        }

        private void RemoveRagdollOnServerSide(GameObject go)
        {
            if (SharedConfig.IsServer)
            {
                foreach (var joint in go.GetComponentsInChildren<CharacterJoint>()) Object.Destroy(joint);

                foreach (var body in go.GetComponentsInChildren<Rigidbody>()) Object.Destroy(body);

                foreach (var collider in go.GetComponentsInChildren<BoxCollider>()) Object.Destroy(collider);

                foreach (var collider in go.GetComponentsInChildren<SphereCollider>()) Object.Destroy(collider);

                foreach (var collider in go.GetComponentsInChildren<CapsuleCollider>()) Object.Destroy(collider);
            }
        }
    }

    public class PlayerHitboxsAction : ICommonResourceActions
    {
        private static readonly LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerFirstAction));

        public void OnLoadFailed(IEntity entity, AssetStatus status)
        {
            var player = entity as PlayerEntity;
            _logger.InfoFormat("P1 LoadFailed: {0}", player.entityKey);
        }

        public void Recycle(AssetStatus status)
        {
        }


        public void Init(IEntity entity, AssetStatus status)
        {
            var playerEntity = entity as PlayerEntity;
            var hitboxConfig = status.Object.AsGameObject;
            HitBoxComponentUtility.InitHitBoxComponent(playerEntity.entityKey.Value, playerEntity, hitboxConfig);
        }

        public bool CanInit(IEntity entity, AssetStatus status)
        {
            var player = entity as PlayerEntity;
            return player.IsThirdAssetLoaded();
        }
    }

    public class PlayerParachuteAaction : ICommonResourceActions
    {
        private static readonly LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerFirstAction));

        public void OnLoadFailed(IEntity entity, AssetStatus status)
        {
            var player = entity as PlayerEntity;
            _logger.InfoFormat("P1 LoadFailed: {0}", player.entityKey);
        }

        public void Recycle(AssetStatus status)
        {
            throw new NotImplementedException();
        }

        public void Init(IEntity entity, AssetStatus status)
        {
            var player = entity as PlayerEntity;
            var transform = status.Object.AsGameObject.transform;
            const string anchorName = "Driver_Seat";
            var anchor = transform.FindChildRecursively(anchorName);
            if (anchor == null) throw new Exception(string.Format("Can not find anchor with name {0} for parachute!", anchorName));

            player.playerSkyMove.IsParachuteLoading = false;
            player.playerSkyMove.Parachute = transform;
            player.playerSkyMove.ParachuteAnchor = anchor;
        }

        public bool CanInit(IEntity entity, AssetStatus status)
        {
            var player = entity as PlayerEntity;
            return player.IsThirdAssetLoaded();
        }
    }
}