using System;
using System.Collections.Generic;
using App.Server.GameModules.GamePlay;
using App.Server.GameModules.GamePlay.free.player;
using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.action;
using com.wd.free.map.position;
using com.wd.free.para;
using com.wd.free.para.exp;
using com.wd.free.skill;
using com.wd.free.unit;
using com.wd.free.util;
using gameplay.gamerule.free.ui;

namespace com.wd.free.map
{
	[System.Serializable]
	public class FreeBuf : BaseGameUnit
	{
		private const long serialVersionUID = 8842439127192517271L;

		private string key;

		private string type;

		private string effect;

		private string vars;

		private string time;

		private string condition;

		private IList<ParaValue> args;

		private bool consume;

		private ISkillTrigger trigger;

		private IMapRegion region;

		private IGameAction enterAction;

		private IGameAction leaveAction;

		private IGameAction eatAction;

		private IGameAction createAction;

		private IGameAction timeOutAction;

		private FreeEffectCreateAction effectAction;

		private bool disable;

		[System.NonSerialized]
		private ICollection<long> ins;

		[System.NonSerialized]
		private FreeEffectShowAction show;

		[System.NonSerialized]
		private PlayerEntity creator;

		[System.NonSerialized]
		private long startTime;

		[System.NonSerialized]
		private ICollection<long> showedIds;

		[System.NonSerialized]
		private string realKey;

		[System.NonSerialized]
		private IParaCondition bufCondition;

		[System.NonSerialized]
		private int realTime;

		public FreeBuf()
		{
		}

		public virtual void OnCreate(IEventArgs skill)
		{
		    startTime = skill.Rule.ServerTime;
			this.ins = new HashSet<long>();
			this.showedIds = new HashSet<long>();
			this.realKey = FreeUtil.ReplaceVar(key, skill);
			this.realTime = FreeUtil.ReplaceInt(time, skill);
            this.paras = new SimpleParaList();

            skill.TempUse("buf", this);
            bool hasCreator = false;
			if (creator != null && creator.hasFreeData)
			{
                skill.TempUse("creator", (FreeData)this.creator.freeData.FreeData);
                hasCreator = true;
			}
			AddParas(skill);
			if (createAction != null)
			{
				createAction.Act(skill);
			}
			if (effectAction != null)
			{
				effectAction.SetSelector(GetPos(region.GetCenter(skill)));
				effectAction.SetKey("bufeffect_" + realKey);
			}
			else
			{
				if (show == null)
				{
					show = new FreeEffectShowAction();
					show.SetKey(effect);
					show.SetPos(GetPos(region.GetCenter(skill)));
				}
			}
            skill.Resume("buf");
			if (creator != null && hasCreator)
			{
                skill.Resume("creator");
			}
			if (!StringUtil.IsNullOrEmpty(condition))
			{
				bufCondition = new ExpParaCondition(FreeUtil.ReplaceVar(condition, skill));
			}
		}

		public virtual string GetRealKey()
		{
			return realKey;
		}

		public virtual void SetEffect(string effect)
		{
			this.effect = effect;
		}

		public virtual void ShowEffect(IEventArgs args, PlayerEntity player)
		{
			if (!showedIds.Contains(player.playerInfo.PlayerId) && !this.disable)
			{
				args.TempUse("buf", this);
				if (effectAction == null)
				{
					show.SetScope(1);
					show.SetTime(0);
					show.SetPlayer("current");
					if (!StringUtil.IsNullOrEmpty(effect))
					{
						show.Act(args);
					}
				}
				else
				{
					effectAction.SetScope(1);
					effectAction.SetPlayer("current");
					effectAction.Act(args);
				}
				args.Resume("buf");
				showedIds.Add(player.playerInfo.PlayerId);
			}
		}

		private void ShowEffectAll(IEventArgs args)
		{
			args.TempUse("buf", this);
			if (effectAction == null)
			{
				show.SetScope(4);
				show.SetTime(0);
				if (!StringUtil.IsNullOrEmpty(effect))
				{
					show.Act(args);
				}
			}
			else
			{
				effectAction.SetScope(4);
				effectAction.Act(args);
			}
			args.Resume("buf");
		}

		private void DeleteEffect(IEventArgs args)
		{
			FreeEffectDeleteAction fda = new FreeEffectDeleteAction();
			if (effectAction == null)
			{
				if (!StringUtil.IsNullOrEmpty(effect))
				{
					fda.SetKey(effect);
				}
				else
				{
					fda.SetKey(string.Empty);
				}
			}
			else
			{
				fda.SetKey("bufeffect_" + realKey);
			}
			fda.SetScope(4);
			fda.Act(args);
		}

		private IPosSelector GetPos(UnitPosition up)
		{
			PosAssignSelector pas = new PosAssignSelector();
			pas.SetX(up.GetX().ToString());
			pas.SetY(up.GetY().ToString());
			pas.SetZ(up.GetZ().ToString());
			return pas;
		}

		private void AddParas(IEventArgs args)
		{
			this.effect = FreeUtil.ReplaceVar(effect, args);
			this.type = FreeUtil.ReplaceVar(type, args);
			this.vars = FreeUtil.ReplaceVar(vars, args);
			this.paras.AddPara(new StringPara("type", type));
			this.paras.AddPara(new StringPara("key", FreeUtil.ReplaceVar(key, args)));
			this.paras.AddPara(new BoolPara("disable", disable));
			if (this.args != null)
			{
				foreach (ParaValue pv in this.args)
				{
					paras.AddPara(pv.GetPara(args));
				}
			}
			if (!StringUtil.IsNullOrEmpty(vars))
			{
				string[] vs = StringUtil.Split(vars, "=");
				for (int i = 0; i < vs.Length - 1; i++)
				{
					string name = vs[i];
					string value = vs[i + 1];
					string[] ns = StringUtil.Split(name, ",");
					string[] values = StringUtil.Split(value, ",");
					if (i < vs.Length - 2)
					{
						if (values.Length > 1)
						{
							string[] lessOne = new string[values.Length - 1];
							System.Array.Copy(values, 0, lessOne, 0, lessOne.Length);
							paras.AddPara(new StringPara(ns[ns.Length - 1], StringUtil.GetStringFromStrings(lessOne, ",")));
						}
					}
					else
					{
						paras.AddPara(new StringPara(ns[ns.Length - 1], value));
					}
				}
			}
		}

		public virtual void Reset(IEventArgs args)
		{
			SetDisable(false);
			showedIds.Clear();
			ShowEffectAll(args);
		}

		private void SetDisable(bool disable)
		{
			this.disable = disable;
			this.paras.AddPara(new BoolPara("disable", disable));
		}

		public virtual void Remove(IEventArgs args)
		{
			this.paras.AddPara(new BoolPara("disable", disable));
			args.TempUse("buf", this);
			if (creator != null && creator.hasFreeData)
			{
				args.TempUse("creator", (FreeData)this.creator.freeData.FreeData);
			}
			Disable(args);
			args.Resume("buf");
			if (creator != null && creator.hasFreeData)
			{
				args.Resume("creator");
			}
		}

		public virtual UnitPosition SelectPosition(IEventArgs skill)
		{
			return region.GetCenter(skill);
		}

		public virtual void Disable(IEventArgs skill)
		{
			SetDisable(true);
			DeleteEffect(skill);
		}

		public virtual void CheckTimeOut(ISkillArgs skill)
		{
			if (realTime > 0 && skill.Rule.ServerTime - startTime >= realTime)
			{
				if (timeOutAction != null)
				{
                    skill.TempUse("buf", this);
					timeOutAction.Act(skill);
                    skill.Resume("buf");
				}
			}

            skill.FreeContext.Bufs.RemoveBuf(skill, this.realKey);
        }

        public virtual void Eat(PlayerEntity player, ISkillArgs skill)
		{
			try
			{
				RealEat(player, skill);
			}
			catch (Exception e)
			{
				Sharpen.Runtime.PrintStackTrace(e);
			}
		}

	    private UnitPosition pos = new UnitPosition();

		private void RealEat(PlayerEntity player, ISkillArgs skill)
		{
			if (!this.disable && (bufCondition == null || bufCondition.Meet(skill)))
			{
				ShowEffect(skill, player);
				skill.TempUse("buf", this);
				if (creator != null)
				{
                    skill.TempUse("creator", (FreeData)this.creator.freeData.FreeData);
				}
                pos.SetX(player.position.Value.x);
			    pos.SetY(player.position.Value.y);
			    pos.SetZ(player.position.Value.z);
                if (region.In(skill, pos))
				{
					if (!ins.Contains(player.playerInfo.PlayerId))
					{
						if (enterAction != null)
						{
							enterAction.Act(skill);
						}
						ins.Add(player.playerInfo.PlayerId);
					}
					if (trigger == null || trigger.Triggered(skill) == ISkillTrigger.TriggerStatus.Success)
					{
						if (eatAction != null)
						{
							eatAction.Act(skill);
							if (consume)
							{
                                skill.FreeContext.Bufs.RemoveBuf(skill, this.realKey);
                            }
						}
					}
				}
				else
				{
					if (ins.Contains(player.playerInfo.PlayerId))
					{
						if (leaveAction != null)
						{
							leaveAction.Act(skill);
						}
						ins.Remove(player.playerInfo.PlayerId);
					}
				}
				if (creator != null)
				{
                    skill.Resume("creator");
				}
                skill.Resume("buf");
			}
		}

		public override string GetKey()
		{
			return key;
		}

		public override void SetKey(string key)
		{
			this.key = key;
		}

		public virtual PlayerEntity GetCreator()
		{
			return creator;
		}

		public virtual void SetCreator(PlayerEntity creator)
		{
			this.creator = creator;
		}

		public virtual string GetType()
		{
			return type;
		}

		public virtual void SetType(string type)
		{
			this.type = type;
		}

		public virtual IMapRegion GetRegion()
		{
			return this.region;
		}

		public virtual bool IsConsume()
		{
			return consume;
		}

		public virtual void SetConsume(bool consume)
		{
			this.consume = consume;
		}

		public virtual ISkillTrigger GetTrigger()
		{
			return trigger;
		}

		public virtual void SetTrigger(ISkillTrigger trigger)
		{
			this.trigger = trigger;
		}

		public virtual IGameAction GetEatAction()
		{
			return eatAction;
		}

		public virtual void SetEatAction(IGameAction eatAction)
		{
			this.eatAction = eatAction;
		}

		public virtual IGameAction GetEnterAction()
		{
			return enterAction;
		}

		public virtual void SetEnterAction(IGameAction enterAction)
		{
			this.enterAction = enterAction;
		}

		public virtual IGameAction GetLeaveAction()
		{
			return leaveAction;
		}

		public virtual void SetLeaveAction(IGameAction leaveAction)
		{
			this.leaveAction = leaveAction;
		}

		public virtual void SetRegion(IMapRegion region)
		{
			this.region = region;
		}

		public virtual void SetTime(int time)
		{
			this.time = time.ToString();
			this.realTime = time;
		}
	}
}
