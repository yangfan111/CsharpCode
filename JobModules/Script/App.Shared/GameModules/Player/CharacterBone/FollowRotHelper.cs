using Core.Compare;
using UnityEngine;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;

namespace App.Shared.GameModules.Player.CharacterBone
{
    public static class FollowRotHelper
    {
        public static PlayerEntity Player { set; private get; }
        public const float Spine1RotateMax = 15.0f;
        public const float SpineRotateMax = 5.0f;
        
        private static readonly float HandRotMax = SingletonManager.Get<CharacterStateConfigManager>().HandRotMax;
        private static readonly float HandRotMin = SingletonManager.Get<CharacterStateConfigManager>().HandRotMin;
        
        public static bool NeedReverse()
        {
            return Player.stateInterface.State.GetActionKeepState() == ActionKeepInConfig.Drive;
        }

        private static bool CanRotHead()
        {
            if (Player.cameraStateNew.FreeNowMode != (int) ECameraFreeMode.On) return false;
            var actionKeep = Player.stateInterface.State.GetActionKeepState();
            var leanState = Player.stateInterface.State.GetCurrentLeanState();
            var actionState = Player.stateInterface.State.GetActionState();
            var postureState = Player.stateInterface.State.GetCurrentPostureState();

            switch (postureState)
            {
                case PostureInConfig.Dying:
                    return false;
            }

            switch (actionState)
            {
                case ActionInConfig.MeleeAttack:
                case ActionInConfig.Grenade:
                case ActionInConfig.Reload:
                case ActionInConfig.SpecialReload:
                case ActionInConfig.Props:
                case ActionInConfig.Gliding:
                    return false;
            }

            switch (actionKeep)
            {
                case ActionKeepInConfig.Sight:
                    return false;
            }

            switch (leanState)
            {
                case LeanInConfig.PeekLeft:
                case LeanInConfig.PeekRight:
                    return false;
            }

            return true;
        }

        public static bool IsHeadRotCw()
        {
            return Player.characterBoneInterface.CharacterBone.IsHeadRotCW;
        }

        private static bool CanPitchHead()
        {
            var actionState = Player.stateInterface.State.GetActionState();
            var postureState = Player.stateInterface.State.GetCurrentPostureState();
            switch (actionState)
            {
                case ActionInConfig.Props:
                case ActionInConfig.Gliding:
                    return false;
            }

            switch (postureState)
            {
                case PostureInConfig.Prone:
                case PostureInConfig.Dying:
                    return false;
            }

            return true;
        }

        private static bool CanPitchHand()
        {
            if (!Player.appearanceInterface.Appearance.IsPrimaryWeaponOrSideArm() ||
                Player.characterBone.IsWeaponRotState)
                return false;
            var actionState = Player.stateInterface.State.GetActionState();
            var postureState = Player.stateInterface.State.GetCurrentPostureState();
            var moveState =
                Player.stateInterface.State.GetIMovementInConfig().CurrentMovement(); //=MovementInConfig.Sprint
            if (moveState == MovementInConfig.Sprint)
            {
                return false;
            }

            switch (actionState)
            {
                case ActionInConfig.MeleeAttack:
                case ActionInConfig.SwitchWeapon:
                case ActionInConfig.Reload:
                case ActionInConfig.SpecialReload:
                    return false;
            }

            switch (postureState)
            {
                case PostureInConfig.Prone:
                case PostureInConfig.Dying:
                case PostureInConfig.Climb:
                    return false;
            }

            return true;
        }

        public static bool CanRotWeapon()
        {
            if (!Player.appearanceInterface.Appearance.IsPrimaryWeaponOrSideArm()) return false;

            var actionState = Player.stateInterface.State.GetActionState();
            var keepActionState = Player.stateInterface.State.GetActionKeepState();
            var movementState = Player.stateInterface.State.GetCurrentMovementState();

            switch (actionState)
            {
                case ActionInConfig.Reload:
                case ActionInConfig.SpecialReload:
                case ActionInConfig.Fire:
                case ActionInConfig.SpecialFireEnd:
                case ActionInConfig.SpecialFireHold:
                    return false;
            }

            switch (keepActionState)
            {
                case ActionKeepInConfig.Sight:
                    return false;
            }

            if (MovementInConfig.Idle != movementState) return false;

            return true;
        }

        public static bool ForbidRot()
        {
            return Player.stateInterface.State.GetActionState() == ActionInConfig.Reload ||
                   Player.stateInterface.State.GetActionState() == ActionInConfig.SpecialReload ||
                   Player.stateInterface.State.GetActionState() == ActionInConfig.Props;
        }

        public static float PitchHeadAngle()
        {
            return CanPitchHead() ? CalcAlwaysAllowHeadPitch(Player.orientation.Pitch) : 0.0f;
        }

        public static float YawHeadAngle()
        {
            return CanRotHead() ? Player.characterBoneInterface.CharacterBone.LastHeadRotAngle : 0.0f;
        }

        public static float PitchHandAngle()
        {
            var handPitch = Player.orientation.Pitch + Player.orientation.PunchPitch;
            if (!CanPitchHand()) return CalcAlwaysAllowHandPitch(handPitch);

            var pitchAngle = (handPitch > HandRotMax) ? HandRotMax : handPitch;
            pitchAngle = (pitchAngle < HandRotMin) ? HandRotMin : pitchAngle;

            return pitchAngle;
        }

        private static float CalcAlwaysAllowHeadPitch(float headPitch)
        {
            if (CompareUtility.IsApproximatelyEqual(headPitch, 0)) return 0;

            var direction = headPitch / Mathf.Abs(headPitch);
            var pitch = headPitch - (SpineRotateMax + Spine1RotateMax) * direction;
            if (pitch * direction <= 0) return 0;
            return pitch;
        }

        private static float CalcAlwaysAllowHandPitch(float handPitch)
        {
            if (!Player.appearanceInterface.Appearance.IsPrimaryWeaponOrSideArm() ||
                PostureInConfig.Prone == Player.stateInterface.State.GetCurrentPostureState()) return 0;
            
            if(Mathf.Abs(handPitch) < SpineRotateMax + Spine1RotateMax)
                return handPitch;

            if (handPitch >= 0)
                return SpineRotateMax + Spine1RotateMax;
            return -(SpineRotateMax + Spine1RotateMax);
        }

        public static float HeadRotProcess()
        {
            return (Player.time.ClientTime - Player.characterBoneInterface.CharacterBone.LastHeadRotSlerpTime) /
                   1000.0f;
        }
    }
}
