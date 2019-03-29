using System.Collections.Generic;
using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.map.position;
using com.wd.free.unit;
using com.wd.free.util;
using Free.framework;
using gameplay.gamerule.free.ui.component;

namespace gameplay.gamerule.free.ui
{
	[System.Serializable]
	public class FreeEffectCreateAction : SendMessageAction
	{
		private const long serialVersionUID = -1188091456904593121L;

		private string key;

		private bool show;

		private bool pvs;

		private IPosSelector selector;

		private string scale;

		private string rotation;

		private string img;

		private IList<IFreeEffect> effects;

		private IList<IFreeUIAuto> autos;

		private string desc;

		private string size;

		public FreeEffectCreateAction()
		{
			this.effects = new List<IFreeEffect>();
			this.autos = new List<IFreeUIAuto>();
		}

		public virtual void AddAuto(IFreeUIAuto auto)
		{
			this.autos.Add(auto);
		}

		public virtual IList<IFreeUIAuto> GetAutos()
		{
			return autos;
		}

        public IList<IFreeEffect> GetEffects()
        {
            return effects;
        }

		public virtual bool IsPvs()
		{
			return pvs;
		}

		public virtual void SetPvs(bool pvs)
		{
			this.pvs = pvs;
		}

		public virtual void AddEffect(IFreeEffect effect)
		{
			this.effects.Add(effect);
		}

		public virtual string GetKey()
		{
			return key;
		}

		public virtual void SetKey(string key)
		{
			this.key = key;
		}

		public virtual IPosSelector GetSelector()
		{
			return selector;
		}

		public virtual void SetSelector(IPosSelector selector)
		{
			this.selector = selector;
		}

		public virtual string GetScale()
		{
			return scale;
		}

		public virtual void SetScale(string scale)
		{
			this.scale = scale;
		}

		public virtual string GetRotation()
		{
			return rotation;
		}

		public virtual void SetRotation(string rotation)
		{
			this.rotation = rotation;
		}

	    protected override void BuildMessage(IEventArgs args)
	    {
	        Build(builder, args, FreeUtil.ReplaceVar(key, args), FreeUtil.ReplaceVar(img, args), show, pvs, GetXyz(args), scale, rotation, effects.AsIterable(), autos, selector, size);
	    }

        public virtual bool IsShow()
		{
			return show;
		}

		public virtual void SetShow(bool show)
		{
			this.show = show;
		}

		public virtual string GetXyz(IEventArgs args)
		{
			if (selector != null)
			{
				UnitPosition up = selector.Select(args);
				if (up != null)
				{
					return up.GetX() + "," + up.GetY() + "," + up.GetZ();
				}
			}
			return string.Empty;
		}

		public static void Build(SimpleProto builder, IEventArgs args, string key, string img, bool show, bool pvs, string xyz, string scale, string rotaion, Iterable<IFreeEffect> effects, IList<IFreeUIAuto> autos, IPosSelector selector, string size)
		{
			builder.Key = 60;
			builder.Bs.Add(show);
			builder.Bs.Add(pvs);
			AddPosition(xyz, scale, rotaion, builder);
			builder.Ks.Add(1);
			builder.Ss.Add(key);
			if (img == null)
			{
				builder.Ss.Add(string.Empty);
			}
			else
			{
				builder.Ss.Add(img);
			}
			builder.Ss.Add(GetAutoString(autos, args));
			if (StringUtil.IsNullOrEmpty(size))
			{
				builder.Ss.Add(string.Empty);
			}
			else
			{
				builder.Ss.Add(FreeUtil.ReplaceVar(size, args));
			}
			if (effects != null)
			{
				foreach (IFreeEffect com in effects)
				{
					builder.Ks.Add(com.GetKey(args));
					AddPosition(com.GetXyz(args, selector), com.GetScale(args), com.GetRotation(args, selector), builder);
					builder.Ss.Add(com.GetStyle(args, key));
					builder.Ss.Add(GetAutoString(com.GetAutos(args), args));
				}
			}
		}

		private static string GetAutoString(IList<IFreeUIAuto> autos, IEventArgs args)
		{
			IList<string> list = new List<string>();
			if (autos != null)
			{
				foreach (IFreeUIAuto auto in autos)
				{
					list.Add(auto.GetField() + "=" + auto.ToConfig(args));
				}
			}
			return StringUtil.GetStringFromStrings(list, "|||");
		}

		private static void AddPosition(string xyz, string scale, string rotaion, SimpleProto fs)
		{
			if (StringUtil.IsNullOrEmpty(xyz))
			{
				fs.Fs.Add(0);
				fs.Fs.Add(0);
				fs.Fs.Add(0);
			}
			else
			{
				foreach (string r in StringUtil.Split(xyz, ","))
				{
					fs.Fs.Add(float.Parse(r));
				}
			}
			if (StringUtil.IsNullOrEmpty(scale))
			{
				fs.Fs.Add(1);
				fs.Fs.Add(1);
				fs.Fs.Add(1);
			}
			else
			{
				foreach (string r in StringUtil.Split(scale, ","))
				{
					fs.Fs.Add(float.Parse(r));
				}
			}
			if (StringUtil.IsNullOrEmpty(rotaion))
			{
				fs.Fs.Add(0);
				fs.Fs.Add(0);
				fs.Fs.Add(0);
			}
			else
			{
				foreach (string r in StringUtil.Split(rotaion, ","))
				{
					fs.Fs.Add(float.Parse(r));
				}
			}
		}

		public override string GetDesc()
		{
			return desc;
		}

		public override void SetDesc(string desc)
		{
			this.desc = desc;
		}

		public virtual string GetSize()
		{
			return size;
		}

		public virtual void SetSize(string size)
		{
			this.size = size;
		}

		

		public override string GetMessageDesc()
		{
			return "创建特效" + key + "\n" + builder.ToString();
		}
	}
}
