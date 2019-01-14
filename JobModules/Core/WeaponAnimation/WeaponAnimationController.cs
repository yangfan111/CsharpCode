using System.Collections.Generic;
using Core.Fsm;
using Core.Utils;
using UnityEngine;
using Utils.Appearance;

namespace Core.WeaponAnimation
{
    public class WeaponAnimationController
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(WeaponAnimationController));
        
        private static readonly Dictionary<FsmInput, string> MonitoredWeaponAnimP3 = new Dictionary<FsmInput, string>(FsmInputEqualityComparer.Instance)
        {
            { FsmInput.FireProgressP3,                  "Fire" },
            { FsmInput.FireEndProgressP3,               "FireEnd" },
            { FsmInput.SightsFireProgressP3,            "SightsFire" },
            { FsmInput.ReloadProgressP3,                "Reload" },
            { FsmInput.ReloadEmptyProgressP3,           "ReloadEmpty" },
            { FsmInput.ReloadStartProgressP3,           "ReloadStart" },
            { FsmInput.ReloadLoopProgressP3,            "ReloadLoop" },
            { FsmInput.ReloadEndProgressP3,             "ReloadEnd" },
            { FsmInput.BuriedBombProgressP3,            "Use" }
        };

        private static readonly Dictionary<FsmInput, string> MonitoredWeaponAnimP1 = new Dictionary<FsmInput, string>(FsmInputEqualityComparer.Instance)
        {
            { FsmInput.FireProgressP1,                  "Fire" },
            { FsmInput.FireEndProgressP1,               "FireEnd" },
            { FsmInput.SightsFireProgressP1,            "SightsFire" },
            { FsmInput.ReloadProgressP1,                "Reload" },
            { FsmInput.ReloadEmptyProgressP1,           "ReloadEmpty" },
            { FsmInput.ReloadStartProgressP1,           "ReloadStart" },
            { FsmInput.ReloadLoopProgressP1,            "ReloadLoop" },
            { FsmInput.ReloadEndProgressP1,             "ReloadEnd" },
            { FsmInput.BuriedBombProgressP1,            "Use" }
        };

        private static readonly Dictionary<FsmInput, string> MonitoredWeaponAnimFinish = new Dictionary<FsmInput, string>(FsmInputEqualityComparer.Instance)
        {
            { FsmInput.FireFinished,                    "Fire" },
            { FsmInput.FireEndFinished,                 "FireEnd" },
            { FsmInput.ReloadFinished,                  "Reload" },
            { FsmInput.BuriedBombFinished,              "Use" }
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

                if(MonitoredWeaponAnimP3.TryGetValue(cmd.Type, out animName) &&
                    weaponP3 != null && IsAnimationExist(weaponP3, animName))
                {
                    progress.ThirdPersonAnimName = animName;
                    progress.ThirdPersonAnimProgress = cmd.AdditioanlValue;
                }

                if(MonitoredWeaponAnimP1.TryGetValue(cmd.Type, out animName) &&
                    weaponP1 != null && IsAnimationExist(weaponP1, animName))
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
                SetNormalizedTime(weaponP1, progress.FirstPersonAnimName, progress.FirstPersonAnimProgress);
            }
            if (weaponP3 != null)
            {
                SetNormalizedTime(weaponP3, progress.ThirdPersonAnimName, progress.ThirdPersonAnimProgress);
            }
        }

        public void WeaponAnimFinishedUpdate(IAdaptiveContainer<IFsmInputCommand> commands,
                                                       GameObject weaponP1,
                                                       GameObject weaponP3)
        {
            for (int i = 0; i < commands.Length; ++i)
            {
                var cmd = commands[i];
                if (MonitoredWeaponAnimFinish.ContainsKey(cmd.Type))
                {
                    FinishedWeaponAnimation(weaponP1);
                    FinishedWeaponAnimation(weaponP3);
                    return;
                }
            }
        }

        private bool IsAnimationExist(GameObject go, string name)
        {
            if (null == go) return false;
            var allAnim = go.GetComponent<UnityEngine.Animation>();
            if (allAnim != null)
            {
                foreach (AnimationState state in allAnim)
                {
                    if (state.clip.name == name)
                    {
                        return true;
                    }
                }
            }
            else
            {
                Logger.WarnFormat("Animation component not exist in {0}", go.name);
            }

            return false;
        }

        private void FinishedWeaponAnimation(GameObject go)
        {
            if (null == go) return;
            var allAnim = go.GetComponent<UnityEngine.Animation>();
            if (null == allAnim) return;
            foreach (AnimationState anim in allAnim)
            {
                allAnim.Play(anim.name);
                anim.normalizedTime = 1;
                allAnim.Sample();
            }
        }

        private void SetNormalizedTime(GameObject go, string name, float normalizedTime)
        {
            if (null == go) return;
            var allAnim = go.GetComponent<UnityEngine.Animation>();
            if (allAnim != null)
            {
                // 没有动画的时候，把所有的动画的nomalizedTime为0
                if (string.IsNullOrEmpty(name))
                {
                    if (allAnim.isPlaying)
                    {
                        allAnim.Stop();
                        foreach (AnimationState anim in allAnim)
                        {
                            anim.normalizedTime = 0;
                        }
                        allAnim.Sample();
                    }
                }
                else
                {
                    var anim = allAnim[name];
                    if (anim != null)
                    {
                        allAnim.Play(name);
                        anim.normalizedTime = normalizedTime;
                        allAnim.Sample();
                    }
                    else
                    {
                        Logger.ErrorFormat("Animation {0} not exist in {1}", name, go.name);
                    }
                }
            }
        }
    }
}