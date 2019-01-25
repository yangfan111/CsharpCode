using System.Collections.Generic;
using Sharpen;
using System;

namespace gameplay.gamerule.free.component
{
    [Serializable]
	public class GameComponents : Iterable<UseGameComponent>
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

		public override Sharpen.Iterator<UseGameComponent> Iterator()
		{
			return components.Iterator();
		}
	}
}
