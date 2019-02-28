using App.Shared.GameModules.Common;
using Core.Animation;
using Core.Utils;
using UnityEngine;
using App.Shared.Components.Player;
using Utils.Appearance;
using Utils.Configuration;
using XmlConfig;
using Object = UnityEngine.Object;
using App.Shared.GameModules.Player.Appearance;
using App.Shared.Player;
using App.Shared.GameModules.Player.ConcreteCharacterController;
using Core.CharacterController;
using KinematicCharacterController;
using Utils.CharacterState;
using Core.CameraControl.NewMotor;
using App.Shared.GameModules.Player.CharacterBone;
using System.Collections.Generic;
using Utils.Compare;
using Utils.Singleton;
using App.Shared.GameModules.Weapon;
using Poly2Tri;

namespace App.Shared.GameModules.Player
{
    public static class PlayerEntityUtility
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerEntityUtility));

        public static Vector3 BulletEmittorOffset = new Vector3(0, 0, 0.1f);
        public static Vector3 BulletDirection = Vector3.forward;
        public static Vector3 CameraOffset = new Vector3(0.3f, 1.8f, -0.6f);
        public static Vector3 ThrowingEmittorFirstOffset = new Vector3(0.3f, -0.4f, 1.2f);
        public static Vector3 ThrowingEmittorThirdOffset = new Vector3(0, -0.2f, 3.0f);
        private static GameObject _go;

        private static GameObject go
        {
            get
            {
                if (_go == null)
                {
                    _go = new GameObject();
                    _go.name = "PlayerEntityUtil";
                }

                return _go;
            }
        }

        public static CharacterController InitCharacterController(GameObject go)
        {
            var config = SingletonManager.Get<CharacterStateConfigManager>()
                .GetCharacterControllerCapsule(PostureInConfig.Stand);
            float height = config.Height;
            CharacterController cc = go.GetComponent<CharacterController>();
            Object.DestroyImmediate(cc);
            cc = go.AddComponent<CharacterController>();
            cc.center = new Vector3(0, height / 2, 0);
            cc.slopeLimit = 45f;
            cc.stepOffset = 0.3f;
            //太小会导致跳跃角色卡主,Use it to avoid numerical precision issues.
            //Two colliders can penetrate each other as deep as their Skin Width. Larger Skin Widths reduce jitter. Low Skin Width can cause the character to get stuck. A good setting is to make this value 10% of the Radius.
            cc.skinWidth = 0.03f;
            cc.minMoveDistance = 0.001f;
            cc.radius = config.Radius;
            cc.height = height;

            go.AddComponent<PlayerScript>();

            return cc;
        }

        public static KinematicCharacterMotor InitKinematicCharacterMotor(GameObject go)
        {
            KinematicCharacterMotor kcc = go.GetComponent<KinematicCharacterMotor>();
            Object.DestroyImmediate(kcc);
            kcc = go.AddComponent<KinematicCharacterMotor>();
            return kcc;
        }

        public static void DisableCollider(Transform go)
        {
            foreach (Transform t in go)
            {
                var c = t.GetComponent<Collider>();
                if (c != null)
                    c.enabled = false;
                DisableCollider(t);
            }
        }

        public static bool TryGetMeleeAttackPosition(this PlayerEntity playerEntity, out Vector3 pos)
        {
            if (playerEntity.hasCameraStateNew)
            {
                pos = playerEntity.cameraFinalOutputNew.PlayerFocusPosition;
                return true;
            }

            pos = Vector3.zero;
            return false;
        }

        public static bool TryGetMeleeAttackRotation(this PlayerEntity playerEntity, out Quaternion rotation)
        {
            if (!playerEntity.hasOrientation)
            {
                rotation = Quaternion.identity;
                return false;
            }

            var yaw = playerEntity.orientation.Yaw - playerEntity.orientation.NegPunchYaw * 2;
            var pitch = playerEntity.orientation.Pitch - playerEntity.orientation.NegPunchPitch * 2;
            rotation = Quaternion.Euler(pitch, yaw, 0);
            return true;
        }

        public static Vector3 GetCameraBulletEmitPosition(PlayerEntity playerEntity)
        {
            go.transform.rotation = Quaternion.Euler(playerEntity.cameraFinalOutputNew.EulerAngle);
            go.transform.position = playerEntity.cameraFinalOutputNew.Position;
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("camera pos is {0}", playerEntity.cameraFinalOutputNew.Position.ToStringExt());
            }

            var rc = go.transform.TransformPoint(BulletEmittorOffset);
            if (playerEntity.cameraStateNew.ViewNowMode == (int) ECameraViewMode.GunSight)
            {
                var _sightLocationP1 = playerEntity.characterBoneInterface.CharacterBone.GetLocation(
                    Utils.CharacterState.SpecialLocation.SightsLocatorPosition,
                    Utils.CharacterState.CharacterView.FirstPerson);
                if (_sightLocationP1 != null)
                {
                    rc = _sightLocationP1.transform.position;
                }
                else
                {
                    Logger.ErrorFormat("no sight locator in weapon");
                }
            }

            return rc;
        }

        public static bool GetGunBulletEmitPosition(PlayerEntity playerEntity, out Vector3 pos)
        {
            var appearance = playerEntity.appearanceInterface.Appearance;
            var characterBone = playerEntity.characterBoneInterface.CharacterBone;
            var muzzleTrans = characterBone.GetLocation(SpecialLocation.MuzzleEffectPosition,
                appearance.IsFirstPerson ? CharacterView.FirstPerson : CharacterView.ThirdPerson);
            if (null != muzzleTrans)
            {
                pos = muzzleTrans.position;
                return true;
            }
            else
            {
                pos = Vector3.zero;
                Logger.ErrorFormat("no muzzle effect anchor found");
                return false;
            }
        }
        //TODO:controller util
        public static Vector3 GetThrowingEmitPosition(PlayerWeaponController controller)
        {
            go.transform.rotation = Quaternion.Euler(controller.RelatedCameraFinal.EulerAngle);
            go.transform.position = controller.RelatedCameraFinal.Position;
            Vector3 rc;
            if (controller.RelatedAppearence.IsFirstPerson)
            {
                rc = go.transform.TransformPoint(ThrowingEmittorFirstOffset);
            }
            else
            {
                rc = go.transform.TransformPoint(ThrowingEmittorThirdOffset);
            }

            return rc;
        }

        public static Vector3 GetHandWeaponPosition(this PlayerEntity playerEntity)
        {
            Vector3 ret = playerEntity.position.Value;
            Transform tran;
            if (playerEntity.appearanceInterface.Appearance.IsFirstPerson)
            {
                if (null == playerEntity.playerInfo.FirstPersonRightHand)
                {
                    var root = playerEntity.RootGo();
                    playerEntity.playerInfo.FirstPersonRightHand =
                        BoneMount.FindChildBoneFromCache(root, BoneName.CharRightHand);
                }

                tran = playerEntity.playerInfo.FirstPersonRightHand;
            }
            else
            {
                if (null == playerEntity.playerInfo.ThirdPersonRightHand)
                {
                    var root = playerEntity.RootGo();
                    playerEntity.playerInfo.ThirdPersonRightHand =
                        BoneMount.FindChildBoneFromCache(root, BoneName.CharRightHand);
                }

                tran = playerEntity.playerInfo.ThirdPersonRightHand;
            }

            if (null != tran)
            {
                ret = tran.position;
            }

            return ret;
        }

        public static Vector3 GetPlayerTopPosition(PlayerEntity playerEntity)
        {
            Vector3 ret = playerEntity.position.Value;
            Transform tran;
            if (!playerEntity.hasAppearanceInterface)
            {
                return Vector3.zero;
            }

            if (playerEntity.appearanceInterface.Appearance.IsFirstPerson)
            {
                if (null == playerEntity.playerInfo.FirstPersonHead)
                {
                    var root = playerEntity.RootGo();
                    playerEntity.playerInfo.FirstPersonHead =
                        BoneMount.FindChildBoneFromCache(root, BoneName.CharacterHeadBoneName);
                }

                tran = playerEntity.playerInfo.FirstPersonHead;
            }
            else
            {
                if (null == playerEntity.playerInfo.ThirdPersonHead)
                {
                    var root = playerEntity.RootGo();
                    playerEntity.playerInfo.ThirdPersonHead =
                        BoneMount.FindChildBoneFromCache(root, BoneName.CharacterHeadBoneName);
                }

                tran = playerEntity.playerInfo.ThirdPersonHead;
            }

            if (null != tran)
            {
                ret = tran.position;
                ret.y += 0.3f;
            }

            return ret;
        }


        public static PlayerEntity GetPlayerFromChildCollider(Collider collider)
        {
            var transform = collider.transform;
            while (transform.parent != null && transform.GetComponent<CharacterController>() == null)
            {
                transform = transform.parent.transform;
            }

            AssertUtility.Assert(transform.GetComponent<CharacterController>() != null);

            return (PlayerEntity) transform.gameObject.GetComponent<EntityReference>().Reference;
        }

        /// <summary>
        /// 这个函数没有应用PeekDegree等boneRigging.Update
        /// </summary>
        /// <param name="player"></param>
        public static void UpdateAnimatorTransform(PlayerEntity player)
        {
            if (!player.hasNetworkAnimator && !player.hasThirdPersonAnimator)
            {
                return;
            }

            var networkAnimator = player.networkAnimator;
            var animatorP3 = player.thirdPersonAnimator.UnityAnimator;
            var previousMode = animatorP3.cullingMode;
            animatorP3.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            // Animation
            PoseReplayer.ReplayPose(networkAnimator.AnimatorLayers, networkAnimator.AnimatorParameters,
                animatorP3);
            animatorP3.cullingMode = previousMode;
        }

        private static readonly AnimatorPoseReplayer PoseReplayer = new AnimatorPoseReplayer();

        public static void UpdateTransform(PlayerEntity player, NetworkAnimatorComponent networkAnimator,
            PredictedAppearanceComponent appearance, OrientationComponent orientation)
        {
            if (!player.hasAppearanceInterface)
            {
                return;
            }

            //Logger.InfoFormat("replay pose, networkAnimatior:BaseClientTime{0}, server Time:{1}", networkAnimator.BaseClientTime, networkAnimator.BaseServerTime);

            // Animation
            Animator.ClearAnimatorJobContainer();

            PoseReplayer.ReplayPose(networkAnimator.AnimatorLayers, networkAnimator.AnimatorParameters,
                player.thirdPersonAnimator.UnityAnimator);

            Animator.BatchUpdate();
            Animator.ClearAnimatorJobContainer();

            // equipment(pan)
            var appearanceInterface = player.appearanceInterface;
            appearanceInterface.Appearance.SyncLatestFrom(player.latestAppearance);
            appearanceInterface.Appearance.SyncPredictedFrom(appearance);
            appearanceInterface.Appearance.TryRewind();

            var characterBoneInterface = player.characterBoneInterface;
            var thirdPersonAppearance = player.thirdPersonAppearance;
            var characterBone = player.characterBone;
            var characterControllerInterface = player.characterControllerInterface;

            characterBoneInterface.CharacterBone.Peek(thirdPersonAppearance.PeekDegree);
            var param = new CodeRigBoneParam
            {
                PitchAmplitude = orientation.Pitch,
                OverlayAnimationWeight =
                    networkAnimator.AnimatorLayers[NetworkAnimatorLayer.PlayerUpperBodyOverlayLayer].Weight,
                PostureWhenOverlay = thirdPersonAppearance.Posture,
                // 预测时，IK不生效
                IKActive = ActiveIK(thirdPersonAppearance.Action, thirdPersonAppearance.Posture,
                    thirdPersonAppearance.NextPosture, thirdPersonAppearance.Movement),
                HeadPitch = characterBone.PitchHeadAngle,
                HeadYaw = characterBone.RotHeadAngle,
                HandPitch = characterBone.PitchHandAngle,
                HeadRotProcess = characterBone.HeadRotProcess,
                WeaponRot = characterBone.WeaponRot,
                IsProne = thirdPersonAppearance.Posture == ThirdPersonPosture.Prone,
                IsHeadRotCW = characterBone.IsHeadRotCW,

                FirstPersonPositionOffset = characterBone.FirstPersonPositionOffset,
                FirstPersonRotationOffset = characterBone.FirstPersonRotationOffset,
                FirstPersonSightOffset = characterBone.FirstPersonSightOffset
            };
            // code controlled pose
            characterBoneInterface.CharacterBone.WeaponRotPlayback(param);
            characterBoneInterface.CharacterBone.Update(param);

            // 更新包围盒

            if (thirdPersonAppearance.NeedUpdateController)
            {
                characterControllerInterface.CharacterController.SetCharacterControllerHeight(thirdPersonAppearance
                    .CharacterHeight);
                characterControllerInterface.CharacterController.SetCharacterControllerCenter(thirdPersonAppearance
                    .CharacterCenter);
                characterControllerInterface.CharacterController.SetCharacterControllerRadius(thirdPersonAppearance
                    .CharacterRadius);
            }

            player.characterContoller.Value.SetCurrentControllerType(
                ThirdPersonPostureTool.ConvertToPostureInConfig(thirdPersonAppearance.Posture));
        }

        public static bool ActiveIK(ThirdPersonAction action, ThirdPersonPosture posture,
            ThirdPersonPosture nextPosture, ThirdPersonMovement movement)
        {
            return action == ThirdPersonAction.EndOfTheWorld &&
                   !IKFilter.IsInThirdPersonPostureFilters(posture) &&
                   !IKFilter.IsInThirdPersonPostureFilters(nextPosture) &&
                   movement != ThirdPersonMovement.Sprint;
        }

        public static void GetCapsule(PlayerEntity player, Vector3 position, out Vector3 pBottom, out Vector3 pUp,
            out float radius)
        {
            var controller = player.characterContoller.Value;
            var height = controller.height;
            radius = controller.radius;
            var midHeight = height - 2 * radius;

            var direction = controller.direction;
            pBottom = controller.center - midHeight * 0.5f * direction;
            pUp = pBottom + direction * midHeight;

            var rotation = controller.transform.rotation;
            pBottom = position + rotation * pBottom;
            pUp = position + rotation * pUp;
        }

        public static Vector3 GetFootPosition(PlayerEntity player)
        {
            var controller = player.characterContoller.Value;
            var midPoint = controller.height * 0.5f - controller.radius;
            return controller.transform.position - midPoint * Vector3.up;
        }

        public static void SetActive(PlayerEntity player, bool active)
        {
            player.RootGo().SetActive(active);
        }
    }
}