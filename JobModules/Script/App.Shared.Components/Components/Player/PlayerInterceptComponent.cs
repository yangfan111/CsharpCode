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
        private Dictionary<int, int> lastDic;
        private List<int> remove;

        public KeyTime()
        {
            startDic = new Dictionary<int, int>();
            lastDic = new Dictionary<int, int>();
            remove = new List<int>();
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

        public List<int> Frame()
        {
            remove.Clear();
            if (lastDic.Count == 0) return remove;
            foreach (var key in lastDic.Keys)
            {
                if (startDic[key] + lastDic[key] - (int) DateTime.Now.Ticks / 10000 <= 0)
                {
                    remove.Add(key);
                }
            }

            if (remove.Count == 0) return remove;
            foreach (int key in remove)
            {
                lastDic.Remove(key);
                startDic.Remove(key);
            }
            return remove;
        }

        public void AddKeyTime(int key, int time)
        {
            if (startDic.ContainsKey(key))
            {
                startDic.Remove(key);
            }
            startDic.Add(key, (int) DateTime.Now.Ticks / 10000);

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
