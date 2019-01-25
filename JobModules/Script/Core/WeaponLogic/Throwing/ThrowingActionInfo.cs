using Core.EntityComponent;
using UnityEngine;
using WeaponConfigNs;

namespace Core.WeaponLogic.Throwing
{
    public class ThrowingActionInfo
    {
        public EntityKey ThrowingEntityKey;
        public bool IsReady;

        public bool IsPull;
        public bool IsThrow;
        public bool IsNearThrow;

        public int LastFireTime;
        public int ReadyTime;
        public int LastSwitchTime;
        public int LastPullTime;

        public Vector3 Pos;
        public Vector3 Vel;
        public float Gravity;
        public float Decay;
        public int CountdownTime;

        public bool ShowCountdownUI;
        public bool IsInterrupt;

        //Draw throwing line
        public ThrowingConfig Config;

        public void ClearState()
        {
            IsReady = false;
            IsPull = false;
            IsThrow = false;
            IsNearThrow = false;
            LastSwitchTime = 0;
            ShowCountdownUI = false;
        }
    }
}
