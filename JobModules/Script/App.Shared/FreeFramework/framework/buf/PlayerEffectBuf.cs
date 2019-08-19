using Assets.App.Server.GameModules.GamePlay.Free;
using com.wd.free.@event;
using Core.Free;
using Free.framework;
using System;
using System.Collections.Generic;

namespace App.Shared.FreeFramework.framework.buf
{
    public class PlayerEffectBuf
    {
        private PlayerEntity player;

        private static Dictionary<string, EffectBuf> bufs = new Dictionary<string, EffectBuf>();

        private Dictionary<string, List<EffectBuf>> currentBufs;

        private float[] lastEffects;

        public PlayerEffectBuf(PlayerEntity player)
        {
            this.player = player;
            currentBufs = new Dictionary<string, List<EffectBuf>>();
            lastEffects = new float[(int)EffectType.End];
        }

        public static void RegisterEffectBuf(EffectBuf buf)
        {
            if (bufs.ContainsKey(buf.key))
            {
                bufs.Remove(buf.key);
            }

            bufs.Add(buf.key, buf);
        }

        public void AddEffect(string effect, float level, int time, IEventArgs args)
        {
            EffectBuf buf = bufs[effect].Clone(args);
            buf.level = level;
            buf.time = time;

            if (!currentBufs.ContainsKey(effect))
            {
                currentBufs.Add(effect, new List<EffectBuf>());
            }

            EffectBuf removed = null;
            foreach (EffectBuf eb in currentBufs[effect])
            {
                if (eb.level == level)
                {
                    removed = eb;
                    break;
                }
            }

            if (removed != null)
            {
                currentBufs[effect].Remove(removed);
            }

            currentBufs[effect].Add(buf);

            currentBufs[effect].Sort();
        }

        public void Update(IEventArgs args)
        {
            foreach (string key in currentBufs.Keys)
            {
                List<EffectBuf> list = currentBufs[key];
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    if (args.Rule.ServerTime - list[i].startTime > list[i].time && list[i].time > 0)
                    {
                        list.Remove(list[i]);
                    }
                }
            }

            float[] effects = new float[lastEffects.Length];

            foreach (string key in currentBufs.Keys)
            {
                List<EffectBuf> list = currentBufs[key];
                if (list.Count > 0)
                {
                    EffectBuf buf = currentBufs[key][0];
                    effects[(int)buf.type] += buf.level;
                }
            }

            for (int i = 0; i < effects.Length; i++)
            {
                float last = lastEffects[i];
                float current = effects[i];
                if (last != current)
                {
                    SimpleProto sp = null;

                    switch ((EffectType)i)
                    {
                        case EffectType.SpeedUp:
                            player.stateInterface.State.SetSpeedAffect(current);
                            sp = FreePool.Allocate();
                            sp.Key = FreeMessageConstant.PlayerMoveSpeedSet;
                            sp.Fs.Add(current);
                            FreeMessageSender.SendMessage(player, sp);
                            break;
                        case EffectType.SlowDown:
                            player.stateInterface.State.SetSpeedAffect(current * -1);
                            sp = FreePool.Allocate();
                            sp.Key = FreeMessageConstant.PlayerMoveSpeedSet;
                            sp.Fs.Add(current * -1);
                            FreeMessageSender.SendMessage(player, sp);
                            break;
                        default:
                            break;
                    }
                }
            }

            lastEffects = effects;
        }

        public void RemoveEffect(string effect, float level)
        {
            if (currentBufs.ContainsKey(effect))
            {
                EffectBuf removed = null;
                foreach (EffectBuf buf in currentBufs[effect])
                {
                    if (buf.level == level)
                    {
                        removed = buf;
                    }
                }
                currentBufs[effect].Remove(removed);
            }
        }
    }

    public enum EffectType
    {
        SpeedUp, SlowDown, End
    }

    public class EffectBuf : IComparable<EffectBuf>
    {
        public string key;
        public EffectType type;
        public float level;
        public int time;
        public long startTime;

        public EffectBuf()
        {

        }

        public EffectBuf(string name, EffectType type)
        {
            this.key = name;
            this.type = type;
        }

        public EffectBuf Clone(IEventArgs args)
        {
            EffectBuf buf = new EffectBuf();
            buf.key = key;
            buf.type = type;
            buf.level = level;
            buf.time = time;
            buf.startTime = args.Rule.ServerTime;

            return buf;
        }

        public int CompareTo(EffectBuf other)
        {
            if (level - other.level < 0)
            {
                return 1;
            }
            else if (level == other.level)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
    }
}
