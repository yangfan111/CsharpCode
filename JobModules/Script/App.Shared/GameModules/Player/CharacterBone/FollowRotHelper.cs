using Core;
using Core.CameraControl.NewMotor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;

namespace App.Shared.GameModules.Player.CharacterBone
{
    public static class FollowRotHelper
    {
        public static PlayerEntity Player { set; private get; }
        private static readonly float HandRotMax = SingletonManager.Get<CharacterStateConfigManager>().HandRotMax;
        private static readonly float HandRotMin = SingletonManager.Get<CharacterStateConfigManager>().HandRotMin;

        public static bool NeedReverse()
        {
            return Player.stateInterface.State.GetActionKeepState() == ActionKeepInConfig.Drive;
        }

        private static bool CanRotHead()
        {
            if (Player.cameraStateNew.FreeNowMode != (int)ECameraFreeMode.On) return false;
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
            var heldType = Player.WeaponController().HeldSlotType;
            if (heldType != EWeaponSlotType.PrimeWeapon &&
                heldType != EWeaponSlotType.SecondaryWeapon &&
                heldType != EWeaponSlotType.PistolWeapon)
                return false;
            var actionState = Player.stateInterface.State.GetActionState();
            var postureState = Player.stateInterface.State.GetCurrentPostureState();
            var moveState = Player.stateInterface.State.GetIMovementInConfig().CurrentMovement();//=MovementInConfig.Sprint
            if (moveState == MovementInConfig.Sprint)
            {
                return false;
            }
            switch (actionState)
            {
                case ActionInConfig.MeleeAttack:
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

        public static bool ForbidRot()
        {
            return Player.stateInterface.State.GetActionState() == ActionInConfig.Reload ||
                Player.stateInterface.State.GetActionState() == ActionInConfig.SpecialReload ||
                Player.stateInterface.State.GetActionState() == ActionInConfig.Props;
        }

        public static float PitchHeadAngle()
        {
            return CanPitchHead() ? Player.orientation.Pitch : 0.0f;
        }

        public static float YawHeadAngle()
        {
            return CanRotHead() ? Player.characterBoneInterface.CharacterBone.LastHeadRotAngle : 0.0f;
        }

        public static float PitchHandAngle()
        {
            if (!CanPitchHand()) return 0;
            var handPitch = Player.orientation.Pitch;
            

            float pitchAngle = (handPitch > HandRotMax) ? HandRotMax : handPitch;
            pitchAngle = (pitchAngle < HandRotMin) ? HandRotMin : pitchAngle;

            return pitchAngle;
        }

        public static float HeadRotProcess()
        {
            return (Player.time.ClientTime - Player.characterBoneInterface.CharacterBone.LastHeadRotSlerpTime) / 1000.0f;
        }
    }
}
