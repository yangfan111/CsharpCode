using Utils.Appearance;
using XmlConfig;

namespace App.Shared.GameModules.Player.CharacterBone
{
    public class IKFilter
    {
        private static readonly ActionInConfig[] ActionStateFilters =
        {
            ActionInConfig.Reload,
            ActionInConfig.SpecialReload,
            ActionInConfig.OpenDoor,
            ActionInConfig.SwitchWeapon,
            ActionInConfig.PickUp,
            ActionInConfig.SpecialFireEnd
        };

        private static readonly ActionKeepInConfig[] KeepActionStateFilters =
        {
            ActionKeepInConfig.Rescue
        };

        private static readonly PostureInConfig[] PostureStateFilters =
        {
            PostureInConfig.ProneTransit,
            PostureInConfig.ProneToCrouch,
            PostureInConfig.ProneToStand,
            PostureInConfig.Prone,
            PostureInConfig.Climb
        };

        private static readonly MovementInConfig[] MovementStateFilters =
        {
            MovementInConfig.Sprint
        };

        private static readonly ThirdPersonPosture[] ThirdPersonPostureFilters =
        {
            ThirdPersonPosture.Swim, ThirdPersonPosture.Dive,
            ThirdPersonPosture.Prone,
            ThirdPersonPosture.ProneTransit,
            ThirdPersonPosture.ProneToCrouch,
            ThirdPersonPosture.ProneToStand,
            ThirdPersonPosture.Climb
        };

        public static bool FilterPlayerIk(ActionInConfig action, ActionInConfig nextAction, 
            ActionKeepInConfig keepAction, PostureInConfig posture,
            PostureInConfig nextPosture, MovementInConfig movement)
        {
            return !(IsStateInActionFilter(action) ||
                     IsStateInActionFilter(nextAction) ||
                     IsStateInKeepActionFilter(keepAction) ||
                     IsStateInPostureFilter(posture) ||
                     IsStateInPostureFilter(nextPosture) ||
                     IsStateInMovementFilter(movement));
        }

        private static bool IsStateInActionFilter(ActionInConfig state)
        {
            for (var i = 0; i < ActionStateFilters.Length; ++i)
            {
                if (ActionStateFilters[i] == state)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsStateInKeepActionFilter(ActionKeepInConfig state)
        {
            for (var i = 0; i < KeepActionStateFilters.Length; ++i)
            {
                if (KeepActionStateFilters[i] == state)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsStateInPostureFilter(PostureInConfig state)
        {
            for (var i = 0; i < PostureStateFilters.Length; ++i)
            {
                if (PostureStateFilters[i] == state)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsStateInMovementFilter(MovementInConfig state)
        {
            for (var i = 0; i < MovementStateFilters.Length; ++i)
            {
                if (MovementStateFilters[i] == state)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsInThirdPersonPostureFilters(ThirdPersonPosture posture)
        {
            for (var i = 0; i < ThirdPersonPostureFilters.Length; ++i)
            {
                if (ThirdPersonPostureFilters[i] == posture)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
