using System.Collections.Generic;
using App.Server.GameModules.GamePlay;
using App.Server.GameModules.GamePlay.free.player;
using Sharpen;
using com.wd.free.@event;
using com.wd.free.action;
using com.wd.free.skill;
using com.wd.free.util;
using com.wd.free.unit;
using Core.Free;
using App.Shared.FreeFramework.Free.player;
using App.Server.GameModules.GamePlay.Free.map.position;

namespace com.wd.free.map
{
	public class PosManager
	{
		private MyDictionary<int, RandomBit> map;

		public PosManager(IEventArgs args)
		{
            this.map = new MyDictionary<int, RandomBit>();
            args.Triggers.AddTrigger(FreeTriggerConstant.PLAYER_EAT_BUF, new ResetPosAction());
        }

		public virtual RandomBit AddRandomBit(int type, int cnt)
		{
            RandomBit v = null;
            if (!map.TryGetValue(type, out v)) {
                v = new RandomBit(cnt);
                map[type] = v;
            }
            return v;
		}

        public virtual int GetRandomBit(int type, int cnt) {
            RandomBit v = null;
            if (map.TryGetValue(type, out v)) {
                return v.Random();
            }
            return 0;
        }

        public virtual void Remove(int type, int pos) {
            RandomBit v = null;
            if (map.TryGetValue(type, out v)) {
                int indexOfPos = 0;
                for (int i = 0, maxi = (v.array == null ? 0 : v.array.Length); i < maxi; i++)
                {
                    if (v.array[i] == pos)
                        indexOfPos = i;
                }
                v.index++;
                int r = v.array[v.index];
                v.array[v.index] = pos;
                v.array[indexOfPos] = r;
            }
        }

        public virtual bool ExsitIndex(int type, int pos)
        {
            RandomBit v = null;
            if (map.TryGetValue(type, out v))
            {
                for (int i = 0; i <= v.index; i++)
                {
                    if (v.array[i] == pos)
                        return true;
                }
            }
            return false;
        }

        public virtual void Clear(int type) {
            RandomBit v = null;
            if (map.TryGetValue(type, out v))
            {
                int len = (v.array == null ? 0 : v.array.Length);
                v.index = len - 1;
                for (int i = 0; i < len; i++)
                {
                    v.array[i] = i;
                }
            }
        }
    }
}
