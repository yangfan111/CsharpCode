using Core.Appearance;
using Core.CharacterState;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.Appearance;
using XmlConfig;

namespace App.Shared.GameModules.Player.Appearance
{
    static class ThirdPersonAppearanceUtils
    {
        public static ThirdPersonPosture GetPosture(PostureInConfig posture)
        {
            ThirdPersonPosture ret;
            switch (posture)
            {
                case PostureInConfig.Stand:
                    ret = ThirdPersonPosture.Stand;
                    break;
                case PostureInConfig.Crouch:
                    ret = ThirdPersonPosture.Crouch;
                    break;
                case PostureInConfig.Prone:
                    ret = ThirdPersonPosture.Prone;
                    break;
                case PostureInConfig.ProneTransit:
                    ret = ThirdPersonPosture.ProneTransit;
                    break;
                case PostureInConfig.ProneToStand:
                    ret = ThirdPersonPosture.ProneToStand;
                    break;
                case PostureInConfig.ProneToCrouch:
                    ret = ThirdPersonPosture.ProneToCrouch;
                    break;
                case PostureInConfig.Swim:
                    ret = ThirdPersonPosture.Swim;
                    break;
                case PostureInConfig.Dive:
                    ret = ThirdPersonPosture.Dive;
                    break;
                case PostureInConfig.Dying:
                    ret = ThirdPersonPosture.Dying;
                    break;
                case PostureInConfig.Climb:
                    ret = ThirdPersonPosture.Climb;
                    break;
                default:
                    ret = ThirdPersonPosture.EndOfTheWorld;
                    break;
            }

            return ret;
        }

        public static ThirdPersonAction GetAction(ActionInConfig action)
        {
            ThirdPersonAction ret;
            switch (action)
            {
                case ActionInConfig.SwitchWeapon:
                    ret = ThirdPersonAction.SwitchWeapon;
                    break;
                case ActionInConfig.PickUp:
                    ret = ThirdPersonAction.PickUp;
                    break;
                case ActionInConfig.Reload:
                    ret = ThirdPersonAction.Reload;
                    break;
                case ActionInConfig.SpecialReload:
                    ret = ThirdPersonAction.SpecialReload;
                    break;
                default:
                    ret = ThirdPersonAction.EndOfTheWorld;
                    break;
            }

            return ret;
        }

        public static ThirdPersonMovement GetMovement(MovementInConfig movement)
        {
            ThirdPersonMovement ret;
            switch (movement)
            {
                case MovementInConfig.Sprint:
                    ret = ThirdPersonMovement.Sprint;
                    break;
                default:
                    ret = ThirdPersonMovement.EndOfTheWorld;
                    break;
            }

            return ret;
        }
    }
}