using System;
using System.Collections.Generic;
using Sharpen;
using com.wd.free.@event;
using com.wd.free.item;
using gameplay.gamerule.free.ui;
using gameplay.gamerule.free.ui.component;

namespace gameplay.gamerule.free.item
{
    [Serializable]
	public class SimpleBackUI
	{
		[System.NonSerialized]
		private FreeUICreateAction fui;

		internal IList<IFreeComponent> components;

		internal int background;

		public SimpleBackUI()
			: base()
		{
		}

		public virtual void Draw(IEventArgs args, ItemInventory inventory)
		{
			if (fui == null)
			{
				fui = new FreeUICreateAction();
				Sharpen.Collections.AddAll(fui.GetComponents(), components);
			}
			fui.SetKey(inventory.GetUIKey());
			fui.SetShow(inventory.IsOpen());
			fui.SetScope(1);
			fui.SetPlayer("current");
			fui.Act(args);
		}
	}
}
