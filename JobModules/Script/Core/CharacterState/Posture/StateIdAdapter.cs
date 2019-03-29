using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.CharacterState.Action;
using Core.CharacterState.Movement;
using Core.Utils;
using Utils.Utils;
using XmlConfig;

namespace Core.CharacterState.Posture
{
    static class StateIdAdapter
    {
        static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(StateIdAdapter));
        internal static PostureInConfig GetPostureStateId(PostureStateId stateId)
        {
            PostureInConfig ret = PostureInConfig.Null;
            switch (stateId)
            {
                case PostureStateId.Stand:
                    ret = PostureInConfig.Stand;
                    break;
                case PostureStateId.Crouch:
                    ret = PostureInConfig.Crouch;
                    break;
                case PostureStateId.Prone:
                    ret = PostureInConfig.Prone;
                    break;
                case PostureStateId.Dying:
                    ret = PostureInConfig.Dying;
                    break;
                case PostureStateId.Dive:
                    ret = PostureInConfig.Dive;
                    break;
                // go through
                case PostureStateId.JumpStart:
                case PostureStateId.Freefall:
                    ret = PostureInConfig.Jump;
                    break;
                case PostureStateId.JumpEnd:
                    ret = PostureInConfig.Land;
                    break;
                case PostureStateId.Swim:
                    ret = PostureInConfig.Swim;
                    break;
                case PostureStateId.ProneTransit:
                    ret = PostureInConfig.ProneTransit;
                    break;
                case PostureStateId.ProneToCrouch:
                    ret = PostureInConfig.ProneToCrouch;
                    break;
                case PostureStateId.ProneToStand:
                    ret = PostureInConfig.ProneToStand;
                    break;
                case PostureStateId.Climb:
                    ret = PostureInConfig.Climb;
                    break;
                case PostureStateId.Slide:
                    ret = PostureInConfig.Slide;
                    break;
                default:
                    ret = PostureInConfig.Null;
                    Logger.ErrorFormat("can not convert PostureStateId type:{0} to PostureInConfig type", stateId);
                    break;
            }

            return ret;
        }

        internal static LeanInConfig GetLeanStateId(PostureStateId stateId)
        {
            LeanInConfig ret = LeanInConfig.NoPeek;
            switch (stateId)
            {
                case PostureStateId.NoPeek:
                    ret = LeanInConfig.NoPeek;
                    break;
                case PostureStateId.PeekLeft:
                    ret = LeanInConfig.PeekLeft;
                    break;
                case PostureStateId.PeekRight:
                    ret = LeanInConfig.PeekRight;
                    break;
                default:
                    ret = LeanInConfig.NoPeek;
                    Logger.ErrorFormat("can not convert PostureStateId type:{0} to MovementInConfig type", stateId);
                    break;
            }
            return ret;
        }
        
        private static Dictionary<ActionStateId, ActionInConfig> _actionIdMap = new Dictionary<ActionStateId, ActionInConfig>(ActionStateIdComparer.Instance)
        {
            { ActionStateId.Fire,                   ActionInConfig.Fire },
            { ActionStateId.SpecialFire,            ActionInConfig.Fire },
            { ActionStateId.SpecialFireHold,        ActionInConfig.SpecialFireHold },
            { ActionStateId.SpecialFireEnd,         ActionInConfig.SpecialFireEnd },
            { ActionStateId.Injury,                 ActionInConfig.Injury },
            { ActionStateId.Reload,                 ActionInConfig.Reload },
            { ActionStateId.SpecialReload,          ActionInConfig.SpecialReload },
            
            { ActionStateId.Unarm,                  ActionInConfig.SwitchWeapon },
            { ActionStateId.Draw,                   ActionInConfig.SwitchWeapon },
            { ActionStateId.SwitchWeapon,           ActionInConfig.SwitchWeapon },
            { ActionStateId.PickUp,                 ActionInConfig.PickUp },
            { ActionStateId.MeleeAttack,            ActionInConfig.MeleeAttack },
            { ActionStateId.Grenade,                ActionInConfig.Grenade },
            
            { ActionStateId.Gliding,                ActionInConfig.Gliding },
            { ActionStateId.Parachuting,            ActionInConfig.Parachuting },
			{ ActionStateId.OpenDoor,               ActionInConfig.OpenDoor},
            { ActionStateId.Props,                  ActionInConfig.Props},
        };

        internal static ActionInConfig GetActionStateId(ActionStateId stateId)
        {
            ActionInConfig ret = ActionInConfig.Null;
            
            if (_actionIdMap.ContainsKey(stateId))
                ret = _actionIdMap[stateId];

            return ret;
        }

        internal static ActionKeepInConfig GetActionKeepStateId(ActionStateId stateId)
        {
            ActionKeepInConfig ret = ActionKeepInConfig.Null;

            switch (stateId)
            {
                case ActionStateId.Drive:
                    ret = ActionKeepInConfig.Drive;
                    break;
                case ActionStateId.Sight:
                    ret = ActionKeepInConfig.Sight;
                    break;
                case ActionStateId.Rescue:
                    ret = ActionKeepInConfig.Rescue;
                    break;
                default:
                    ret = ActionKeepInConfig.Null;
                    break;
            }
            return ret;
        }

        internal static ActionKeepInConfig GetActionKeepStateId(ActionStateId curStateId, ActionStateId nextStateId)
        {
            ActionKeepInConfig ret = ActionKeepInConfig.Null;
            if (curStateId == ActionStateId.Sight || nextStateId == ActionStateId.Sight)
            {
                ret = ActionKeepInConfig.Sight;
            }
            return ret;
        }

        internal static MovementInConfig GetMovementStateId(MovementStateId stateId)
        {
            MovementInConfig ret = MovementInConfig.Null;
            switch (stateId)
            {
                case MovementStateId.Idle:
                    ret = MovementInConfig.Idle;
                    break;
                case MovementStateId.Walk:
                    ret = MovementInConfig.Walk;
                    break;
                case MovementStateId.Run:
                    ret = MovementInConfig.Run;
                    break;
                case MovementStateId.Sprint:
                    ret = MovementInConfig.Sprint;
                    break;
                case MovementStateId.DiveMove:
                    ret = MovementInConfig.DiveMove;
                    break;
                // 以下暂时没有用到
                //case MovementStateId.Swim:
                //    ret = MovementInConfig.Swim;
                //    break;
                //case MovementStateId.Dive:
                //    ret = MovementInConfig.Dive;
                //    break;
                //case MovementStateId.Injured:
                //    ret = MovementInConfig.Injured;
                //    break;
                default:
                    ret = MovementInConfig.Null;
                    break;
            }
            return ret;
        }
    }
}
