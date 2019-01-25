using System.Collections.Generic;
using Sharpen;
using com.wd.free.action;
using com.wd.free.action.function;

namespace gameplay.gamerule.free.action
{
	[System.Serializable]
	public class CommonGameAction
	{
		private const long serialVersionUID = -790144672493625524L;

		private string scope;

		private string key;

		private string name;

		private string desc;

		private string group;

		private IList<FuncArg> args;

		private IGameAction action;

		public CommonGameAction()
			: base()
		{
			this.args = new List<FuncArg>();
		}

		public CommonGameAction(string key)
			: base()
		{
			this.key = key;
			this.args = new List<FuncArg>();
		}

		public virtual GameFunc ToGameFunc()
		{
			GameFunc gf = new GameFunc();
			gf.SetKey(key);
			gf.SetName(name);
			gf.SetDesc(desc);
			gf.SetAction(action);
			gf.SetArgs(args);
			return gf;
		}

		public virtual string GetScope()
		{
			return scope;
		}

		public virtual void SetScope(string scope)
		{
			this.scope = scope;
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

		public virtual string GetKey()
		{
			return key;
		}

		public virtual void SetKey(string key)
		{
			this.key = key;
		}

		public virtual void AddFuncArg(FuncArg arg)
		{
			this.args.Add(arg);
		}

		public virtual IGameAction GetAction()
		{
			return action;
		}

		public virtual void SetAction(IGameAction action)
		{
			this.action = action;
		}

		public virtual string GetDesc()
		{
			return desc;
		}

		public virtual void SetDesc(string desc)
		{
			this.desc = desc;
		}
	}
}
