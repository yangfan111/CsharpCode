using System.Collections.Generic;
using App.Server.GameModules.GamePlay;
using App.Server.GameModules.GamePlay.free.player;
using Sharpen;
using com.wd.free.@event;
using com.wd.free.action;
using com.wd.free.skill;
using com.wd.free.util;

namespace com.wd.free.map
{
	public class FreeBufManager
	{
		private MyDictionary<string, FreeBuf> map;

		private long lastTime;

		public FreeBufManager()
		{
			this.map = new MyDictionary<string, FreeBuf>();
			this.lastTime = 0;
		}

		public virtual void AddFreeBuf(FreeBuf buf, IEventArgs args)
		{
			string key = FreeUtil.ReplaceVar(buf.GetKey(), args);
			if (map.ContainsKey(key))
			{
				FreeLog.Error("buf '" + key + "' 覆盖了原有的BUF", null);
			}
			this.map[key] = buf;
		}

		public virtual void ShowBuf(PlayerEntity player, IEventArgs args)
		{
			args.TempUse("current", (FreeData)player.freeData.FreeData);
			foreach (KeyValuePair<string, FreeBuf> buf in map)
			{
				buf.Value.ShowEffect(args, player);
			}
			args.Resume("current");
		}

		public virtual void CheckTimeOut(ISkillArgs skill)
		{
			if (Runtime.CurrentTimeMillis() - lastTime > 1000)
			{
				foreach (FreeBuf buf in map.Values)
				{
					buf.CheckTimeOut(skill);
				}
			}
		}

		public virtual void Eat(PlayerEntity player, ISkillArgs skill)
		{
			foreach (FreeBuf buf in map.Values)
			{
                FreeLog.SetTrigger(buf);
				buf.Eat(player, skill);
			}
		}

		public virtual void Reset(IEventArgs args, string key)
		{
			FreeBuf buf = map[key];
			if (buf != null)
			{
				buf.Reset(args);
			}
		}

		public virtual void RemoveBuf(IEventArgs args, string key)
		{
			FreeBuf buf = map[key];
			if (buf != null)
			{
				buf.Remove(args);
				map.Remove(key);
			}
		}
	}
}
