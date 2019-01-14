using System.Collections.Generic;
using Sharpen;
using com.wd.free.@event;
using com.wd.free.action;
using com.wd.free.action.function;
using com.wd.free.para;
using com.wd.free.trigger;
using System;

namespace gameplay.gamerule.free.component
{
    [Serializable]
    public class UseGameComponent
	{
		private string key;

		private string name;

		private string desc;

		private string group;

		private IList<ArgValue> args;

		private IList<ComponentAction> actions;

		private IList<ComponentTrigger> triggers;

		private IList<ComponentParaCondition> conditions;

		private IList<ComponentSkill> skills;

		private IList<ComponentUIComponent> uis;

		private IList<ComponentFreeEffect> effects;

		private IList<ComponentPosSelector> poss;

		public UseGameComponent()
		{
		}

		public virtual string GetKey()
		{
			return key;
		}

		public virtual void SetKey(string key)
		{
			this.key = key;
		}

		public virtual string GetName()
		{
			return name;
		}

		public virtual void SetName(string name)
		{
			this.name = name;
		}

		public virtual string GetGroup()
		{
			return group;
		}

		public virtual void SetGroup(string group)
		{
			this.group = group;
		}

		public virtual string GetDesc()
		{
			return desc;
		}

		public virtual void SetDesc(string desc)
		{
			this.desc = desc;
		}

		public virtual IList<ComponentAction> GetActions()
		{
			return actions;
		}

		public virtual IList<ComponentTrigger> GetTriggers()
		{
			return triggers;
		}

		public virtual IList<ComponentParaCondition> GetConditions()
		{
			return conditions;
		}

		public virtual IList<ComponentSkill> GetSkills()
		{
			return skills;
		}

		public virtual IList<ComponentUIComponent> GetUis()
		{
			return uis;
		}

		public virtual IList<ComponentFreeEffect> GetEffects()
		{
			return effects;
		}

		public virtual void SetEffects(IList<ComponentFreeEffect> effects)
		{
			this.effects = effects;
		}

		public virtual IList<ComponentPosSelector> GetPoss()
		{
			return poss;
		}

		public virtual void SetPoss(IList<ComponentPosSelector> poss)
		{
			this.poss = poss;
		}

		public virtual void Merge(TriggerList triggers)
		{
			GameTrigger iniTrigger = null;
			foreach (GameTrigger gt in triggers)
			{
				if (gt.GetKey() == 1)
				{
					iniTrigger = gt;
					break;
				}
			}
			if (iniTrigger == null)
			{
				iniTrigger = new GameTrigger(1);
                triggers.AddTrigger(iniTrigger);
			}
			iniTrigger.AddAction(new UseGameComponent.AddComponentParaAction(this, this.name, args));
		}

		[System.Serializable]
		internal class AddComponentParaAction : AbstractGameAction
		{
			private const long serialVersionUID = -8318145779075011619L;

			private string xml;

			private IList<ArgValue> args;

			public AddComponentParaAction(UseGameComponent _enclosing, string xml, IList<ArgValue> args)
				: base()
			{
				this._enclosing = _enclosing;
				this.args = args;
				this.xml = xml;
			}

			public override void DoAction(IEventArgs args)
			{
				if (this.args != null)
				{
					foreach (ArgValue av in this.args)
					{
						ParaValue pv = new ParaValue();
						pv.SetName(av.GetName());
						pv.SetValue(av.GetValue());
						IPara para = args.GetDefault().GetParameters().Get(av.GetName());
						if (para != null)
						{
							if (para is IntPara)
							{
								pv.SetType("int");
							}
							if (para is StringPara)
							{
								pv.SetType("string");
							}
							if (para is BoolPara)
							{
								pv.SetType("bool");
							}
							if (para is FloatPara)
							{
								pv.SetType("float");
							}
							if (para is DoublePara)
							{
								pv.SetType("double");
							}
						}
						// 如果为空，则说明在父规则中定义了当前规则的组件，所以忽略当前设置，直接使用父规则中的设置
						// 如 bioPushcar引用了 bioComponent, reliveComponent,
						// bioComponent中也引用了reliveComponent,当bioComponent设置组件值时,reliveComponent还未初始化
						// 则bioPushcar中的reliveComponent将在最后赋值，也是最终的值
						IPara paraValue = pv.GetPara(args);
						if (paraValue != null)
						{
							args.GetDefault().GetParameters().AddPara(paraValue);
						}
					}
				}
			}

			private readonly UseGameComponent _enclosing;
		}
	}
}
