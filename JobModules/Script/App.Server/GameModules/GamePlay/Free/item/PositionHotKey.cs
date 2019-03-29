using System.Collections.Generic;
using App.Server.GameModules.GamePlay;
using App.Server.GameModules.GamePlay.free.player;
using Sharpen;
using com.wd.free.@event;
using com.wd.free.action;
using com.wd.free.item;
using com.wd.free.skill;
using gameplay.gamerule.free.rule;

namespace gameplay.gamerule.free.item
{
	public class PositionHotKey : IHotKey
	{
		private IList<ConditionHotKey> keys;

		public virtual string GetHotKey(IEventArgs args, ItemPosition ip)
		{
			ConditionHotKey result = null;
			args.TempUse("item", ip);
			foreach (ConditionHotKey pk in keys)
			{
				if (pk.Meet(args))
				{
					result = pk;
					break;
				}
			}
			args.Resume("item");
			FreeData fd = (FreeData)args.GetUnit("current");
			if (fd != null && result != null)
			{
				fd.GetUnitSkill().RemoveSkill("skill_hotkey_" + result.key);
				PlayerActionSkill skill = new PlayerActionSkill();
				skill.SetKey("skill_hotkey_" + result.key);
				SkillClickTrigger trigger = new SkillClickTrigger();
				trigger.SetKey(int.Parse(result.key));
				skill.SetTrigger(trigger);
				skill.SetEffect(new PositionHotKey.ItemHotkeyAction(this, ip, fd, ip.GetInventory().GetName()));
				fd.GetUnitSkill().AddSkill(skill);
			}
			if (result != null)
			{
				return result.ui;
			}
			else
			{
				return string.Empty;
			}
		}

		[System.Serializable]
		internal class ItemHotkeyAction : AbstractGameAction
		{
			private const long serialVersionUID = 2047134880792143342L;

			internal ItemPosition ip;

			internal FreeData fd;

			internal string inventory;

			public ItemHotkeyAction(PositionHotKey _enclosing)
				: base()
			{
				this._enclosing = _enclosing;
			}

			public ItemHotkeyAction(PositionHotKey _enclosing, ItemPosition ip, FreeData fd, string inventory)
				: base()
			{
				this._enclosing = _enclosing;
				this.ip = ip;
				this.fd = fd;
				this.inventory = inventory;
			}

			// 如果快捷键里面的物品被清除，则为了效率保留技能，不做任何动作
			public override void DoAction(IEventArgs args)
			{
				if (this.ip != null && this.ip.GetInventory() != null)
				{
					if (this.ip.GetCount() > 0 && this.ip.GetInventory().GetName().Equals(this.inventory))
					{
						FreeItemManager.UseItem(this.ip, this.fd, (FreeRuleEventArgs)args);
					}
				}
			}

			private readonly PositionHotKey _enclosing;
		}
	}
}
