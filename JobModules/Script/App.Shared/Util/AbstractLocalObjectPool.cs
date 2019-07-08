using System.Collections.Generic;
using UnityEngine;
using XmlConfig;

namespace App.Shared.Util
{
    public abstract class AbstractLocalObjectPool<T> where T : IClientObjectEmitter
    {
        public readonly HashSet<T> playingEffects = new HashSet<T>();

        protected readonly Stack<T> reusableEffects = new Stack<T>(50);
        protected uint accumulator;

        public Transform poolFolder { get; protected set; }

        public override string ToString()
        {
            return string.Format("playing :{0},resume:{1}", playingEffects.Count, reusableEffects.Count);
        }

        public void Reusable(T ele)
        {
            ele.Recycle();
            playingEffects.Remove(ele);
            reusableEffects.Push(ele);
        }

        public abstract void PreLoad();
        protected ClientEffectCommonConfigItem cfgItem;
    }
}
