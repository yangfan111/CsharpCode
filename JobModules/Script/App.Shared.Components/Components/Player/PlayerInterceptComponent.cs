using Entitas;
using Entitas.CodeGeneration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace App.Shared.Components.Player
{
    [Player]
    public class PlayerInterceptComponent : IComponent
    {
        [DontInitilize] public int InterceptType;
        [DontInitilize] public Vector3 MovePos;
        [DontInitilize] public Vector3 FacePos;
        [DontInitilize] public Vector3 AimPos;
        [DontInitilize] public KeyTime PresssKeys;
        [DontInitilize] public KeyTime InterceptKeys;
        [DontInitilize] public int AttackPlayerId;
    }

    public class KeyTime
    {
        private Dictionary<int, long> startDic;
        private Dictionary<int, int> lastDic;

        private Dictionary<int, int> remove;

        public KeyTime()
        {
            startDic = new Dictionary<int, long>();
            lastDic = new Dictionary<int, int>();
            remove = new Dictionary<int, int>();
        }

        public bool Empty
        {
            get { return lastDic.Count == 0; }
        }

        public void Clear()
        {
            startDic.Clear();
            lastDic.Clear();
            remove.Clear();
        }

        public int[] Keys
        {
            get { return lastDic.Keys.ToArray(); }
        }

        public void Frame()
        {
            if (lastDic.Count == 0)
            {
                return;
            }

            remove.Clear();

            long now = DateTime.Now.Ticks / 10000;
            foreach (var key in lastDic)
            {
                remove.Add(key.Key, (int)(key.Value - now + startDic[key.Key]));
            }

            foreach (var key in remove)
            {
                if (key.Value <= 0)
                {
                    lastDic.Remove(key.Key);
                    startDic.Remove(key.Key);
                }
            }
        }

        public void AddKeyTime(int key, int time)
        {
            if (startDic.ContainsKey(key))
            {
                startDic.Remove(key);
            }
            startDic.Add(key, DateTime.Now.Ticks / 10000);

            if (lastDic.ContainsKey(key))
            {
                if (time < lastDic[key])
                {
                    time = lastDic[key];
                }
                lastDic.Remove(key);
            }

            lastDic.Add(key, time);
        }
    }
}
