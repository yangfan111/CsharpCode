using System.Collections.Generic;
using App.Shared.Components.Player;
using Core;
using Core.Animation;
using Core.CharacterState;
using Core.CharacterState.Posture;
using Core.Components;
using Core.EntityComponent;
using Core.Event;
using Core.UpdateLatest;
using UnityEngine;

namespace App.Shared.Components.Serializer
{
    public class FieldMergeUtil
    {
        public static List<T> Merge<T>(List<T> to,
            List<T> patch) where T : class, IPatchClass<T>, new()
        {
            var count = patch.Count;
            var lastCount = to.Count;
            to = FieldSerializeUtil.Resize(to, count);
            for (int i = 0; i < count; i++)
            {
                if (patch[i].HasValue)
                {
                    to[i].MergeFromPatch(patch[i]);
                }
            }

            return to;
        }

        public static List<int> Merge(List<int> to,
            List<int> patch)
        {
            to.Clear();
            to.AddRange(patch);
            return to;
        }
        /// <summary>
        /// size是不会变的，否则重新生成
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dest"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public static T[] Merge<T>(T[] dest,
        T[] origin) where T : class, IPatchClass<T>, new()
        {
            var oc = origin.Length;
            var dc = dest.Length;
            dest = FieldSerializeUtil.Resize(dest, oc);
            for (int i = 0; i < oc; i++)
            {
                if (origin[i].HasValue)
                {
                    dest[i].MergeFromPatch(origin[i]);
                }
            }

            return dest;
        }
       
        public static int Merge(int basevalue, int patchvalue)
        {
            return patchvalue;
        }

        public static uint Merge(uint basevalue, uint patchvalue)
        {
            return patchvalue;
        }

        public static byte Merge(byte basevalue, byte patchvalue)
        {
            return patchvalue;
        }

        public static short Merge(short basevalue, short patchvalue)
        {
            return patchvalue;
        }

        public static bool Merge(bool basevalue, bool patchvalue)
        {
            return patchvalue;
        }

        public static double Merge(double basevalue, double patchvalue)
        {
            return patchvalue;
        }

        public static float Merge(float basevalue, float patchvalue)
        {
            return patchvalue;
        }

        public static string Merge(string basevalue, string patchvalue)
        {
            return patchvalue;
        }

        public static EntityKey Merge(EntityKey basevalue, EntityKey patchvalue)
        {
            return patchvalue;
        }

        public static Vector3 Merge(Vector2 basevalue, Vector2 patchvalue)
        {
            return patchvalue;
        }

        public static Vector3 Merge(Vector3 basevalue, Vector3 patchvalue)
        {
            return patchvalue;
        }

        public static FixedVector3 Merge(FixedVector3 basevalue, FixedVector3 patchvalue)
        {
            return patchvalue;
        }
        public static InterruptData Merge(InterruptData basevalue, InterruptData patchvalue)
        {
            return patchvalue;
        }
        public static Quaternion Merge(Quaternion basevalue, Quaternion patchvalue)
        {
            return patchvalue;
        }

        public static long Merge(long basevalue, long patchvalue)
        {
            return patchvalue;
        }

        public static PlayerEvents Merge(PlayerEvents basevalue, PlayerEvents patchvalue)
        {
            basevalue.ReInit();
            basevalue.CopyFrom(patchvalue);
            return basevalue;
        }

        public static StateInterCommands Merge(StateInterCommands basevalue, StateInterCommands patchvalue)
        {
            basevalue.Reset();
            basevalue.CopyFrom(patchvalue);
            return basevalue;
        }
        
        public static UnityAnimationEventCommands Merge(UnityAnimationEventCommands basevalue, UnityAnimationEventCommands patchvalue)
        {
            basevalue.Reset();
            basevalue.CopyFrom(patchvalue);
            return basevalue;
        }

      
    }
}