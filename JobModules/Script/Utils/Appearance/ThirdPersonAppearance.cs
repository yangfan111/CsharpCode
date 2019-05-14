using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XmlConfig;

namespace Utils.Appearance
{
    public enum ThirdPersonPosture
    {
        Stand,
        Crouch,
        Prone,
        ProneTransit,
        ProneToStand,
        ProneToCrouch,
        Swim,
        Dive,
        Dying,
        Climb,
        EndOfTheWorld
    }

    public static class ThirdPersonPostureTool
    {
        public static PostureInConfig ConvertToPostureInConfig(ThirdPersonPosture pos)
        {
            PostureInConfig ret = PostureInConfig.Null;
            switch (pos)
            {
                case ThirdPersonPosture.Stand:
                    ret = PostureInConfig.Stand;
                    break;
                case ThirdPersonPosture.Crouch:
                    ret = PostureInConfig.Crouch;
                    break;
                case ThirdPersonPosture.Prone:
                    ret = PostureInConfig.Prone;
                    break;
                case ThirdPersonPosture.ProneTransit:
                    ret = PostureInConfig.ProneTransit;
                    break;
                case ThirdPersonPosture.ProneToStand:
                    ret = PostureInConfig.ProneToStand;
                    break;
                case ThirdPersonPosture.ProneToCrouch:
                    ret = PostureInConfig.ProneToCrouch;
                    break;
                case ThirdPersonPosture.Swim:
                    ret = PostureInConfig.Swim;
                    break;
                case ThirdPersonPosture.Dive:
                    ret = PostureInConfig.Dive;
                    break;
                case ThirdPersonPosture.Dying:
                    ret = PostureInConfig.Dying;
                    break;
                default:
                    break;
                        
            }

            return ret;
        }
    }

    public enum ThirdPersonAction
    {
        Null,
        Reload,
        SpecialReload,
        SwitchWeapon,
        PickUp,
        Drive,
        EndOfTheWorld
    }

    public enum ThirdPersonMovement
    {
        Sprint,
        EndOfTheWorld
    }

}
