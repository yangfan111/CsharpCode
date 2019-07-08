using System.Collections.Generic;
using Core.Fsm;
using UnityEngine;
using Utils.Appearance;
using Utils.Appearance.Weapon;

namespace Core.WeaponAnimation
{
    public class WeaponAnimationController
    {
        private static readonly Dictionary<FsmInput, string> MonitoredWeaponAnimP3 =
            new Dictionary<FsmInput, string>(FsmInputEqualityComparer.Instance)
            {
                {FsmInput.FireProgressP3, "Fire"},
                {FsmInput.FireEndProgressP3, "FireEnd"},
                {FsmInput.SightsFireProgressP3, "SightsFire"},
                {FsmInput.ReloadProgressP3, "Reload"},
                {FsmInput.ReloadEmptyProgressP3, "ReloadEmpty"},
                {FsmInput.ReloadStartProgressP3, "ReloadStart"},
                {FsmInput.ReloadLoopProgressP3, "ReloadLoop"},
                {FsmInput.ReloadEndProgressP3, "ReloadEnd"},
                {FsmInput.BuriedBombProgressP3, "Use"}
            };

        private static readonly Dictionary<FsmInput, string> MonitoredWeaponAnimP1 =
            new Dictionary<FsmInput, string>(FsmInputEqualityComparer.Instance)
            {
                {FsmInput.FireProgressP1, "Fire"},
                {FsmInput.FireEndProgressP1, "FireEnd"},
                {FsmInput.SightsFireProgressP1, "SightsFire"},
                {FsmInput.ReloadProgressP1, "Reload"},
                {FsmInput.ReloadEmptyProgressP1, "ReloadEmpty"},
                {FsmInput.ReloadStartProgressP1, "ReloadStart"},
                {FsmInput.ReloadLoopProgressP1, "ReloadLoop"},
                {FsmInput.ReloadEndProgressP1, "ReloadEnd"},
                {FsmInput.BuriedBombProgressP1, "Use"}
            };

        private static readonly Dictionary<FsmInput, string> MonitoredWeaponAnimFinish =
            new Dictionary<FsmInput, string>(FsmInputEqualityComparer.Instance)
            {
                {FsmInput.FireFinished, "Fire"},
                {FsmInput.FireEndFinished, "FireEnd"},
                {FsmInput.ReloadFinished, "Reload"},
                {FsmInput.BuriedBombFinished, "Use"}
            };

        public void FromAvatarAnimToWeaponAnimProgress(IAdaptiveContainer<IFsmInputCommand> commands,
            GameObject weaponP1,
            GameObject weaponP3,
            IWeaponAnimProgress progress)
        {
            progress.FirstPersonAnimName = string.Empty;
            progress.ThirdPersonAnimName = string.Empty;

            for (int i = 0; i < commands.Length; ++i)
            {
                var cmd = commands[i];
                string animName;

                if (MonitoredWeaponAnimP3.TryGetValue(cmd.Type, out animName) &&
                    weaponP3 != null && WeaponAnimationBase.IsAnimationExist(weaponP3, animName))
                {
                    progress.ThirdPersonAnimName = animName;
                    progress.ThirdPersonAnimProgress = cmd.AdditioanlValue;
                }

                if (MonitoredWeaponAnimP1.TryGetValue(cmd.Type, out animName) &&
                    weaponP1 != null && WeaponAnimationBase.IsAnimationExist(weaponP1, animName))
                {
                    progress.FirstPersonAnimName = animName;
                    progress.FirstPersonAnimProgress = cmd.AdditioanlValue;
                }
            }
        }

        public void FromWeaponAnimProgressToWeaponAnim(GameObject weaponP1,
            GameObject weaponP3,
            IWeaponAnimProgress progress)
        {
            if (weaponP1 != null)
            {
                WeaponAnimationBase.FinishedWeaponAnimation(weaponP1);
                WeaponAnimationBase.SetNormalizedTime(weaponP1, progress.FirstPersonAnimName, progress.FirstPersonAnimProgress);
            }

            if (weaponP3 != null)
            {
                WeaponAnimationBase.FinishedWeaponAnimation(weaponP3);
                WeaponAnimationBase.SetNormalizedTime(weaponP3, progress.ThirdPersonAnimName, progress.ThirdPersonAnimProgress);
            }
        }

        public void WeaponAnimFinishedUpdate(IAdaptiveContainer<IFsmInputCommand> commands,
            GameObject weaponP1,
            GameObject weaponP3,
            IWeaponAnimProgress progress)
        {
            for (int i = 0; i < commands.Length; ++i)
            {
                var cmd = commands[i];
                if (MonitoredWeaponAnimFinish.ContainsKey(cmd.Type))
                {
                    WeaponAnimationBase.FinishedWeaponAnimation(weaponP1);
                    WeaponAnimationBase.FinishedWeaponAnimation(weaponP3);
                    progress.FirstPersonAnimProgress = 0;
                    progress.ThirdPersonAnimProgress = 0;
                    return;
                }
            }
        }
    }
}
