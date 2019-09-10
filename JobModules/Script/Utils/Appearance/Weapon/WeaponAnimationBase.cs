using Core.Utils;
using UnityEngine;

namespace Utils.Appearance.Weapon
{
    public class WeaponAnimationBase
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(WeaponAnimationBase));
        public static void FinishedWeaponAnimation(GameObject go)
        {
            if (null == go) return;
            var allAnim = go.GetComponent<Animation>();
            if (null == allAnim) return;
            foreach (AnimationState anim in allAnim)
            {
                allAnim.Play(anim.name);
                anim.normalizedTime = 0;
                allAnim.Sample();
            }
            allAnim.Stop();
        }
        
        public static bool IsAnimationExist(GameObject go, string name)
        {
            if (null == go) return false;
            var allAnim = go.GetComponent<Animation>();
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

        public static void SetNormalizedTime(GameObject go, string name, float normalizedTime)
        {
            if (null == go) return;
            var allAnim = go.GetComponent<Animation>();
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
