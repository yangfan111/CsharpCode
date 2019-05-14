using System;
using System.Collections.Generic;
using System.Linq;
using App.Shared.GameModules.HitBox;
using App.Shared.GameModules.Player.Appearance.AnimationEvent;
using App.Shared.Player;
using Core.Animation;
using Core.CharacterState;
using Core.HitBox;
using Core.Utils;
using UnityEngine;
using Utils.Appearance;
using Utils.AssetManager;
using Utils.Singleton;

namespace App.Shared.GameModules.Player.ResourceLoad
{
    public class ThirdPersonModelLoadHandler : ModelLoadHandler
    {
        private IUnityAssetManager _assetManager;
        private static readonly LoggerAdapter _logger = new LoggerAdapter(typeof(ThirdPersonModelLoadHandler));


        public ThirdPersonModelLoadHandler(Contexts contexts) : base(contexts)
        {
            _assetManager = contexts.session.commonSession.AssetManager;
        }

        public void OnLoadSucc(PlayerEntity player, UnityObject unityObj)
        {
            GameObject go = unityObj;

            if (player.hasThirdPersonModel)
            {
                _assetManager.Recycle(player.thirdPersonModel.UnityObjectValue);
                player.RemoveAsset(player.thirdPersonModel.UnityObjectValue);
            }

            player.ReplaceThirdPersonModel(go, unityObj);

            var provider = SingletonManager.Get<HitBoxTransformProviderCache>()
                .GetProvider(player.thirdPersonModel.Value);
            HitBoxComponentUtility.InitHitBoxComponent(player.entityKey.Value, player, provider);

            RemoveRagdollOnServerSide(go, provider.GetHitBoxColliders().Values.ToList());

            HandleLoadedModel(player, go);

            InitCharacterControllerSetting(player);

            player.AddAsset(unityObj);

            go.name = go.name.Replace("(Clone)", "");

            go.transform.SetParent(GetThirdModelParent(player.RootGo().transform));
            go.transform.localPosition = new Vector3(0, 0, 0);
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            Logger.InfoFormat("P3 loaded: {0}", player.entityKey);

            BoneTool.CacheTransform(go);

            player.ReplaceBones(null, null, null);

            player.bones.Head = BoneMount.FindChildBoneFromCache(go, BoneName.CharacterHeadBoneName);
            player.bones.Spine = BoneMount.FindChildBoneFromCache(go, BoneName.CharacterSpineName);

            player.ReplaceThirdPersonAnimator(go.GetComponent<Animator>());

            var ik = go.AddComponent<PlayerIK>();
            ik.SetAnimator(AvatarIKGoal.LeftHand, player.thirdPersonAnimator.UnityAnimator);
            ik.SetIKLayer(AvatarIKGoal.LeftHand, NetworkAnimatorLayer.ThirdPersonIKPassLayer);
            ik.SetAnimator(AvatarIKGoal.RightHand, player.thirdPersonAnimator.UnityAnimator);
            ik.SetIKLayer(AvatarIKGoal.RightHand, NetworkAnimatorLayer.ThirdPersonIKPassLayer);

            if (player.isFlagSelf)
            {
                var animationEvent = go.AddComponent<AnimationClipEvent>();
                animationEvent.Player = player;
                player.animatorClip.ClipManager.SetAnimationCleanEventCallback(animationEvent
                    .InterruptAnimationEventFunc);
            }
            else
            {
                go.AddComponent<ThirdPersonAnimationClipEvent>();
            }

            // 设置大厅传入的roleId和avatarId
            player.appearanceInterface.Appearance.SetRoleModelIdAndInitAvatar(player.playerInfo.RoleModelId,
                player.playerInfo.AvatarIds);

            player.characterControllerInterface.CharacterController.SetCharacterRoot(player.characterContoller.Value
                .gameObject);
            player.appearanceInterface.Appearance.SetThirdPersonCharacter(go);
            player.characterControllerInterface.CharacterController.SetThirdModel(player.thirdPersonModel.Value);

            player.characterBoneInterface.CharacterBone.SetCharacterRoot(player.characterContoller.Value
                .gameObject);
            player.characterBoneInterface.CharacterBone.SetThirdPersonCharacter(go);
            ForceCrouch(player.thirdPersonAnimator.UnityAnimator);
            player.characterBoneInterface.CharacterBone.SetStableCrouchPelvisRotation();
            ForceStand(player.thirdPersonAnimator.UnityAnimator);
            player.characterBoneInterface.CharacterBone.SetStableStandPelvisRotation();

            player.appearanceInterface.Appearance.SetAnimatorP3(player.thirdPersonAnimator.UnityAnimator);

            player.appearanceInterface.Appearance.PlayerReborn();
            player.characterControllerInterface.CharacterController.PlayerReborn();
            if(player.hasStateInterface)
                player.stateInterface.State.PlayerReborn();
            if(player.hasPlayerInfo)
                player.playerInfo.InitTransform();

            player.ReplaceNetworkAnimator(
                NetworkAnimatorUtil.CreateAnimatorLayers(player.thirdPersonAnimator.UnityAnimator),
                NetworkAnimatorUtil.GetAnimatorParams(player.thirdPersonAnimator.UnityAnimator));

            player.networkAnimator.SetEntityName(player.entityKey.ToString());

            player.ReplaceOverrideNetworkAnimator();

            if (SharedConfig.IsServer)
            {
                player.ReplaceNetworkAnimatiorServerTime(0);
            }

            // 禁用非可见状态下的动画更新，在获取Stable状态之后
            if (SharedConfig.IsServer || !player.isFlagSelf)
                player.thirdPersonAnimator.UnityAnimator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
            else
                player.thirdPersonAnimator.UnityAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        }

        private void InitCharacterControllerSetting(PlayerEntity player)
        {
            var rootGo = player.RootGo();
            var characterController = rootGo.GetComponent<CharacterController>();
            if (characterController != null && player.hasCharacterInfo)
            {
                var info = player.characterInfo.CharacterInfoProviderContext;
                characterController.stepOffset = info.GetStepOffset();
                characterController.slopeLimit = info.GetSlopeLimit();
                _logger.DebugFormat("characterController is :{0}, characterinfo:{1}, stepOffset:{2}, slopeLimit:{3}", characterController == null, 
                    player.hasCharacterInfo,
                    info.GetStepOffset(),
                    info.GetSlopeLimit()
                    );
            }
            else
            {
                _logger.DebugFormat("characterController is :{0}, characterinfo:{1}", characterController == null, player.hasCharacterInfo);
            }
        }

        private static void ForceStand(UnityEngine.Animator animator)
        {
            SetPosture(animator, AnimatorParametersHash.Instance.StandValue);
        }
        
        private static void ForceCrouch(UnityEngine.Animator animator)
        {
            SetPosture(animator, AnimatorParametersHash.Instance.CrouchValue);
        }

        private static void SetPosture(Animator animator, float value)
        {
            try
            {
                animator.SetFloat(AnimatorParametersHash.Instance.PostureHash, value);
                animator.Update(0);
            }
            catch (Exception e)
            {
                _logger.ErrorFormat("there is no parameters of {0}, can not force set animator to stand or crouch!!!",
                    AnimatorParametersHash.Instance.PostureName);
            }
        }

        private static Transform GetThirdModelParent(Transform root)
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

        private void RemoveRagdollOnServerSide(GameObject go, List<Collider> except)
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

                foreach (var collider in go.GetComponentsInChildren<Collider>())
                {
                    if (except.Contains(collider)) continue;
                    GameObject.Destroy(collider);
                }
            }
        }
    }
}
