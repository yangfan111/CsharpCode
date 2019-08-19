using Core.EntityComponent;
using UnityEngine;
using WeaponConfigNs;

namespace Core
{
    public class ThrowingActionData
    {
        public EntityKey ThrowingEntityKey;
        public bool      IsReady;
        public bool IsPull;
        public bool IsThrow;
        public bool IsThrowing;
        public bool IsNearThrow;
        public Vector3 throwBackupPos;
        public int LastSwitchTime;

        public Vector3 Pos;
        public Vector3 Vel;
        public float   Gravity;
        public float   Decay;
        public int     CountdownTime;

        public bool ShowCountdownUI;
        public bool IsInterrupt;
        //上次开火武器Key值
        public int LastFireWeaponKey;

        //Draw throwing line
        public ThrowingConfig Config;

        public bool ThrowingPrepare
        {
            get { return IsReady && !IsThrow; }
        }
        public void InternalCleanUp(bool interrupt = false)
        {
            if (!interrupt || IsReady || ShowCountdownUI)
            {
                IsReady         = false;
                IsPull          = false;
                IsThrow         = false;
                IsNearThrow     = false;
                LastSwitchTime  = 0;
                ShowCountdownUI = false;
            }
        }

    }
}