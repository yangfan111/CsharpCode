using System.Collections.Generic;
using Sharpen;
using com.wd.free.action;
using com.wd.free.para.exp;
using com.wd.free.skill;
using gameplay.gamerule.free.ui.component;
using com.wd.free.map.position;
using System;

namespace gameplay.gamerule.free.component
{
    [Serializable]
    public class GameComponentMap
	{
		private IDictionary<string, Stack<ISkillTrigger>> triggerMap;

		private IDictionary<string, Stack<IGameAction>> actionMap;

		private IDictionary<string, Stack<ISkill>> skillMap;

		private IDictionary<string, Stack<IParaCondition>> conditionMap;

		private IDictionary<string, Stack<IFreeComponent>> uiMap;

		private IDictionary<string, Stack<IFreeEffect>> effectMap;

		private IDictionary<string, Stack<IPosSelector>> posMap;

		public GameComponentMap()
		{
			this.triggerMap = new Dictionary<string, Stack<ISkillTrigger>>();
			this.actionMap = new Dictionary<string, Stack<IGameAction>>();
			this.skillMap = new Dictionary<string, Stack<ISkill>>();
			this.conditionMap = new Dictionary<string, Stack<IParaCondition>>();
			this.uiMap = new Dictionary<string, Stack<IFreeComponent>>();
			this.effectMap = new Dictionary<string, Stack<IFreeEffect>>();
			this.posMap = new Dictionary<string, Stack<IPosSelector>>();
		}

		public virtual gameplay.gamerule.free.component.GameComponentMap Clone()
		{
			gameplay.gamerule.free.component.GameComponentMap map = new gameplay.gamerule.free.component.GameComponentMap();
			foreach (KeyValuePair<string, Stack<ISkill>> entry in skillMap)
			{
				map.skillMap[entry.Key] = new Stack<ISkill>();
				foreach (ISkill skill in entry.Value)
				{
					map.skillMap[entry.Key].Push(skill);
				}
			}
			foreach (KeyValuePair<string, Stack<IGameAction>> entry_1 in actionMap)
			{
				map.actionMap[entry_1.Key] = new Stack<IGameAction>();
				foreach (IGameAction skill in entry_1.Value)
				{
					map.actionMap[entry_1.Key].Push(skill);
				}
			}
			foreach (KeyValuePair<string, Stack<ISkillTrigger>> entry_2 in triggerMap)
			{
				map.triggerMap[entry_2.Key] = new Stack<ISkillTrigger>();
				foreach (ISkillTrigger skill in entry_2.Value)
				{
					map.triggerMap[entry_2.Key].Push(skill);
				}
			}
			foreach (KeyValuePair<string, Stack<IParaCondition>> entry_3 in conditionMap)
			{
				map.conditionMap[entry_3.Key] = new Stack<IParaCondition>();
				foreach (IParaCondition skill in entry_3.Value)
				{
					map.conditionMap[entry_3.Key].Push(skill);
				}
			}
			foreach (KeyValuePair<string, Stack<IFreeComponent>> entry_4 in uiMap)
			{
				map.uiMap[entry_4.Key] = new Stack<IFreeComponent>();
				foreach (IFreeComponent skill in entry_4.Value)
				{
					map.uiMap[entry_4.Key].Push(skill);
				}
			}
			foreach (KeyValuePair<string, Stack<IFreeEffect>> entry_5 in effectMap)
			{
				map.effectMap[entry_5.Key] = new Stack<IFreeEffect>();
				foreach (IFreeEffect skill in entry_5.Value)
				{
					map.effectMap[entry_5.Key].Push(skill);
				}
			}
			foreach (KeyValuePair<string, Stack<IPosSelector>> entry_6 in posMap)
			{
				map.posMap[entry_6.Key] = new Stack<IPosSelector>();
				foreach (IPosSelector skill in entry_6.Value)
				{
					map.posMap[entry_6.Key].Push(skill);
				}
			}
			return map;
		}

		public virtual void Merge(GameComponents components)
		{
			foreach (UseGameComponent ugc in components)
			{
				if (ugc.GetActions() != null)
				{
					foreach (ComponentAction ca in ugc.GetActions())
					{
						if (!this.actionMap.ContainsKey(ca.GetName()))
						{
							this.AddAction(ca.GetName(), ca.GetDefaultAction());
						}
					}
				}
				if (ugc.GetTriggers() != null)
				{
					foreach (ComponentTrigger ca in ugc.GetTriggers())
					{
						if (!this.triggerMap.ContainsKey(ca.GetName()))
						{
							this.AddSkillTrigger(ca.GetName(), ca.GetDefaultTrigger());
						}
					}
				}
				if (ugc.GetConditions() != null)
				{
					foreach (ComponentParaCondition ca in ugc.GetConditions())
					{
						if (!this.conditionMap.ContainsKey(ca.GetName()))
						{
							this.AddCondition(ca.GetName(), ca.GetDefaultCondition());
						}
					}
				}
				if (ugc.GetSkills() != null)
				{
					foreach (ComponentSkill ca in ugc.GetSkills())
					{
						if (!this.skillMap.ContainsKey(ca.GetName()))
						{
							this.AddSkill(ca.GetName(), ca.GetDefaultSkill());
						}
					}
				}
				if (ugc.GetUis() != null)
				{
					foreach (ComponentUIComponent ca in ugc.GetUis())
					{
						if (!this.uiMap.ContainsKey(ca.GetName()))
						{
							this.AddUI(ca.GetName(), ca.GetComponent());
						}
					}
				}
				if (ugc.GetPoss() != null)
				{
					foreach (ComponentPosSelector ca in ugc.GetPoss())
					{
						if (!this.posMap.ContainsKey(ca.GetName()))
						{
							this.AddPosSelecotr(ca.GetName(), ca.GetDefaultPos());
						}
					}
				}
				if (ugc.GetEffects() != null)
				{
					foreach (ComponentFreeEffect ca in ugc.GetEffects())
					{
						if (!this.effectMap.ContainsKey(ca.GetName()))
						{
							this.AddEffect(ca.GetName(), ca.GetDefaultEffect());
						}
					}
				}
			}
		}

		public virtual void TempUseUI(string name, IFreeComponent ui)
		{
			AddUI(name, ui);
		}

		public virtual void ResumeUI(string name)
		{
			if (uiMap.ContainsKey(name))
			{
				uiMap[name].Pop();
			}
		}

		public virtual void TempUsePos(string name, IPosSelector pos)
		{
			AddPosSelecotr(name, pos);
		}

		public virtual void ResumePos(string name)
		{
			if (posMap.ContainsKey(name))
			{
				posMap[name].Pop();
			}
		}

		public virtual void TempUseEffect(string name, IFreeEffect effect)
		{
			AddEffect(name, effect);
		}

		public virtual void ResumeEffect(string name)
		{
			if (effectMap.ContainsKey(name))
			{
				effectMap[name].Pop();
			}
		}

		public virtual void TempUseAction(string name, IGameAction action)
		{
			AddAction(name, action);
		}

		public virtual void ResumeAction(string name)
		{
			if (actionMap.ContainsKey(name))
			{
				actionMap[name].Pop();
			}
		}

		public virtual void TempUseSkill(string name, ISkill skill)
		{
			AddSkill(name, skill);
		}

		public virtual void ResumeSkill(string name)
		{
			if (skillMap.ContainsKey(name))
			{
				skillMap[name].Pop();
			}
		}

		public virtual void TempUseSkillTrigger(string name, ISkillTrigger trigger)
		{
			AddSkillTrigger(name, trigger);
		}

		public virtual void ResumeSkillTrigger(string name)
		{
			if (triggerMap.ContainsKey(name))
			{
				triggerMap[name].Pop();
			}
		}

		public virtual void TempUseCondition(string name, IParaCondition condition)
		{
			AddCondition(name, condition);
		}

		public virtual void ResumCondition(string name)
		{
			if (conditionMap.ContainsKey(name))
			{
				conditionMap[name].Pop();
			}
		}

		public virtual void AddUI(string name, IFreeComponent ui)
		{
			if (!uiMap.ContainsKey(name))
			{
				uiMap[name] = new Stack<IFreeComponent>();
			}
			this.uiMap[name].Push(ui);
		}

		public virtual void AddPosSelecotr(string name, IPosSelector pos)
		{
			if (!posMap.ContainsKey(name))
			{
				posMap[name] = new Stack<IPosSelector>();
			}
			this.posMap[name].Push(pos);
		}

		public virtual void AddEffect(string name, IFreeEffect effect)
		{
			if (!effectMap.ContainsKey(name))
			{
				effectMap[name] = new Stack<IFreeEffect>();
			}
			this.effectMap[name].Push(effect);
		}

		public virtual void AddSkillTrigger(string name, ISkillTrigger trigger)
		{
			if (!triggerMap.ContainsKey(name))
			{
				triggerMap[name] = new Stack<ISkillTrigger>();
			}
			this.triggerMap[name].Push(trigger);
		}

		public virtual void AddAction(string name, IGameAction action)
		{
			if (!actionMap.ContainsKey(name))
			{
				actionMap[name] = new Stack<IGameAction>();
			}
			this.actionMap[name].Push(action);
		}

		public virtual void AddCondition(string name, IParaCondition condition)
		{
			if (!conditionMap.ContainsKey(name))
			{
				conditionMap[name] = new Stack<IParaCondition>();
			}
			this.conditionMap[name].Push(condition);
		}

		public virtual void AddSkill(string name, ISkill skill)
		{
			if (!skillMap.ContainsKey(name))
			{
				skillMap[name] = new Stack<ISkill>();
			}
			this.skillMap[name].Push(skill);
		}

		public virtual IFreeComponent GetUI(string name)
		{
			if (uiMap.ContainsKey(name))
			{
				return uiMap[name].Peek();
			}
			return null;
		}

		public virtual IGameAction GetAction(string name)
		{
			if (actionMap.ContainsKey(name))
			{
				return actionMap[name].Peek();
			}
			return null;
		}

		public virtual ISkill GetSkill(string name)
		{
			if (skillMap.ContainsKey(name))
			{
				return skillMap[name].Peek();
			}
			return null;
		}

		public virtual IParaCondition GetCondition(string name)
		{
			if (conditionMap.ContainsKey(name))
			{
				return conditionMap[name].Peek();
			}
			return null;
		}

		public virtual IPosSelector GetPos(string name)
		{
			if (posMap.ContainsKey(name))
			{
				return posMap[name].Peek();
			}
			return null;
		}

		public virtual IFreeEffect GetEffect(string name)
		{
			if (effectMap.ContainsKey(name))
			{
				return effectMap[name].Peek();
			}
			return null;
		}

		public virtual ISkillTrigger GetSkillTrigger(string name)
		{
			if (triggerMap.ContainsKey(name))
			{
				return triggerMap[name].Peek();
			}
			return null;
		}
	}
}
