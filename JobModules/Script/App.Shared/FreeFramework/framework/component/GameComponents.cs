using System.Collections.Generic;
using Sharpen;
using System;
using Core.Free;

namespace gameplay.gamerule.free.component
{
    [Serializable]
	public class GameComponents : Iterable<UseGameComponent>, IRule
    {
		private IList<UseGameComponent> components;

		public GameComponents()
		{
			this.components = new List<UseGameComponent>();
		}

		public virtual void AddComponent(UseGameComponent component)
		{
			this.components.Add(component);
		}

        public int GetRuleID()
        {
            return (int)ERuleIds.GameComponents;
        }

        public override Sharpen.Iterator<UseGameComponent> Iterator()
		{
			return components.Iterator();
		}
	}
}
