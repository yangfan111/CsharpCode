using Entitas;
using Entitas.CodeGeneration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
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
        [DontInitilize] public KeyTime PressKeys;
        [DontInitilize] public KeyTime InterceptKeys;
        [DontInitilize] public KeyTime RealPressKeys;
        [DontInitilize] public int AttackPlayerId;
    }

    public class KeyTime
    {
        private Dictionary<int, int> startDic;
        private Dictionary<int, long> lastDic;
        private List<int> remove;

        public KeyTime()
        {
            startDic = new Dictionary<int, int>();
            lastDic = new Dictionary<int, long>();
            remove = new List<int>();
        }

        public void Clear()
        {
            startDic.Clear();
            lastDic.Clear();
            remove.Clear();
        }

        public bool Empty
        {
            get { return lastDic.Count == 0; }
        }

        public int[] Keys
        {
            get { return lastDic.Keys.ToArray(); }
        }

        public int Axis(int key)
        {
            if (startDic.ContainsKey(key))
                return startDic[key];
            return 0;
        }

        public void Frame()
        {
            if (lastDic.Count == 0) return;
            remove.Clear();
            foreach (var key in lastDic.Keys)
            {
                if (lastDic[key] - DateTime.Now.Ticks / 10000L <= 0)
                {
                    remove.Add(key);
                }
            }

            if (remove.Count == 0) return;
            foreach (int key in remove)
            {
                lastDic.Remove(key);
                startDic.Remove(key);
            }
        }

        public void AddKeyTime(int key, long time, int axis = 0)
        {
            if (startDic.ContainsKey(key))
                startDic.Remove(key);
            startDic.Add(key, axis);

            long now = DateTime.Now.Ticks / 10000L;
            if (lastDic.ContainsKey(key))
            {
                if (time < lastDic[key] - now)
                    return;
                lastDic.Remove(key);
            }
            lastDic.Add(key, time + now);
        }

        public void Release(int key)
        {
            if (lastDic.ContainsKey(key))
            {
                lastDic.Remove(key);
                startDic.Remove(key);
            }
        }
    }
}
