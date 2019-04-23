using com.wd.free.skill;
using Core.Prediction.UserPrediction.Cmd;

namespace App.Server.GameModules.GamePlay.free.player
{
    public class SimplePlayerInput : IPlayerInput
    {
        private IUserCmd _cmd;

        public const int FORWARD = 0;
        public const int BACKWARD = 1;
        public const int LEFT = 2;
        public const int RIGHT = 3;

        public const int SWITCHWALK = 4;
        public const int JUMP = 5;
        public const int ATTACK = 6;
        public const int DUCK = 7;
        public const int RELOAD = 8;
        public const int ATTACK2 = 9;
        public const int THROWWEAPON = 10;
        public const int DEMOLITION = 11;
        public const int SKILL = 12;
        public const int SPRAY = 13;
        public const int Q = 14;
        public const int Y = 15;
        public const int SHOWGUN = 16;
        public const int P = 17;
        public const int C = 18;
        public const int TAB = 20;
        public const int BACKQUOTE = 21;
        public const int B = 22;
        public const int X = 23;
        public const int PageDown = 24;
        public const int PageUp = 25;
        public const int Z = 26;
        public const int EQUAL = 27;
        public const int SHIFT = 28;
        public const int CTRL = 29;

        private const int NumberDelta = 1000;

        public SimplePlayerInput()
        {
        }

        public void SetUserCmd(IUserCmd cmd)
        {
            this._cmd = cmd;
        }

        public static void PressKey(IUserCmd cmd, int key)
        {
            SetKey(cmd, key, true);
        }

        public static void ReleaseKey(IUserCmd cmd, int key)
        {
            SetKey(cmd, key, false);
        }

        private static void SetKey(IUserCmd cmd, int key, bool value)
        {
            if(key > NumberDelta)
            {
                cmd.CurWeapon = key - NumberDelta;
            }
            switch (key)
            {
                case SKILL:
                    cmd.IsF = value;
                    break;
                case DEMOLITION:
                    cmd.IsPeekRight = value;
                    break;
                case Q:
                    cmd.IsPeekLeft = value;
                    break;
                case JUMP:
                    cmd.IsJump = value;
                    break;
                case C:
                    cmd.IsCrouch = value;
                    break;
                case RELOAD:
                    cmd.IsReload = value;
                    break;
                case P:
                    cmd.IsPDown = value;
                    break;
                case Y:
                    cmd.IsYDown = value;
                    break;
                case THROWWEAPON:
                    cmd.IsDropWeapon = value;
                    break;
                case TAB:
                    cmd.IsTabDown = value;
                    break;
                case FORWARD:
                    cmd.MoveVertical = value ? 1 : 0;
                    break;
                case BACKWARD:
                    cmd.MoveVertical = value ? -1 : 0;
                    break;
                case LEFT:
                    cmd.MoveHorizontal = value ? -1 : 0;
                    break;
                case RIGHT:
                    cmd.MoveHorizontal = value ? 1 : 0;
                    break;
                case ATTACK:
                    cmd.IsLeftAttack = value;
                    break;
                case ATTACK2:
                    cmd.IsRightAttack = value;
                    break;
                case X:
                    cmd.IsDrawWeapon = value;
                    break;
                case Z:
                    cmd.IsProne = value;
                    break;
                case EQUAL:
                    cmd.IsSwitchAutoRun = value;
                    break;
                case SHIFT:
                    cmd.IsRun = value;
                    break;
                case CTRL:
                    cmd.IsSlightWalk = value;
                    break;
                case SHOWGUN:
                    cmd.ChangeCamera = value;
                    break;
                case B:
                    cmd.IsSwitchFireMode = value;
                    break;
                case PageUp:
                    cmd.IsScopeIn = value;
                    break;
                case PageDown:
                    cmd.IsScopeOut = value;
                    break;
                default:
                    break;
            }
        }

        public bool IsPressed(int key)
        {
            if(key > NumberDelta)
            {
                return _cmd.CurWeapon + NumberDelta == key;
            }
            switch (key)
            {
                case SKILL:
                    return _cmd.IsF;
                case DEMOLITION:
                    return _cmd.IsPeekRight;
                case Q:
                    return _cmd.IsPeekLeft;
                case JUMP:
                    return _cmd.IsJump;
                case C:
                    return _cmd.IsCrouch;
                case RELOAD:
                    return _cmd.IsReload;
                case P:
                    return _cmd.IsPDown;
                case Y:
                    return _cmd.IsYDown;
                case THROWWEAPON:
                    return _cmd.IsDropWeapon;
                case TAB:
                    return _cmd.IsTabDown;
                case FORWARD:
                    return _cmd.MoveVertical > 0;
                case ATTACK:
                    return _cmd.IsLeftAttack;
                case ATTACK2:
                    return _cmd.IsRightAttack;
                case BACKWARD:
                    return _cmd.MoveVertical < 0;
                case LEFT:
                    return _cmd.MoveHorizontal > 0;
                case RIGHT:
                    return _cmd.MoveHorizontal < 0;
                case X:
                    return _cmd.IsDrawWeapon;
                case Z:
                    return _cmd.IsProne;
                case EQUAL:
                    return _cmd.IsSwitchAutoRun;
                case SHIFT:
                    return _cmd.IsRun;
                case CTRL:
                    return _cmd.IsSlightWalk;
                case SHOWGUN:
                    return _cmd.ChangeCamera;
                case B:
                    return _cmd.IsSwitchFireMode;
                case PageUp:
                    return _cmd.IsScopeIn;
                case PageDown:
                    return _cmd.IsScopeOut;
;               default:
                    return false;
            }
        }
    }
}