using App.Shared.GameModules.Player.Appearance;
using App.Shared.GameModules.Player.Appearance.AnimationEvent;
using App.Shared.Player;
using Core.Animation;
using UnityEngine;
using Utils.Appearance;
using Utils.AssetManager;

namespace App.Shared.GameModules.Player.ResourceLoad
{
    public class FirstPersonModelLoadHandler : ModelLoadHandler
    {
        private IUnityAssetManager _assetManager;

        public FirstPersonModelLoadHandler(Contexts contexts) : base(contexts)
        {
            _assetManager = contexts.session.commonSession.AssetManager;
        }

        public void OnLoadSucc(PlayerEntity player, UnityObject unityObj)
        {
            GameObject go = unityObj;

            HandleLoadedModel(player, go);

            if (player.hasFirstPersonModel)
            {
                _assetManager.Recycle(player.firstPersonModel.UnityObjectValue);
                player.RemoveAsset(player.firstPersonModel.UnityObjectValue);
            }

            player.ReplaceFirstPersonModel(go, unityObj);

            player.AddAsset(unityObj);

            player.appearanceInterface.FirstPersonAppearance =
                new FirstPersonAppearanceManager(player.firstPersonAppearance);

            go.name = "P1_" + player.entityKey;
            go.transform.SetParent(player.RootGo().transform);
            go.transform.localPosition = new Vector3(0, player.firstPersonAppearance.FirstPersonHeight,
                player.firstPersonAppearance.FirstPersonForwardOffset);
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            Logger.InfoFormat("P1 loaded: {0}", player.entityKey);

            player.ReplaceFirstPersonAnimator(go.GetComponent<Animator>());

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
                player.animatorClip.ClipManager.SetAnimationCleanEventCallback(animationEvent
                    .InterruptAnimationEventFunc);
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

            player.ReplaceFpAnimStatus(
                NetworkAnimatorUtil.CreateAnimatorLayers(player.firstPersonAnimator.UnityAnimator),
                NetworkAnimatorUtil.GetAnimatorParams(player.firstPersonAnimator.UnityAnimator));

            // 禁用非可见状态下的动画更新
            if (!player.isFlagSelf)
                player.firstPersonAnimator.UnityAnimator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
            else
                player.firstPersonAnimator.UnityAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        }
    }
}
