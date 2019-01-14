using System.Collections.Generic;
using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.map.position;
using com.wd.free.util;

namespace gameplay.gamerule.free.ui.component
{
	[System.Serializable]
	public class FreeSingleEffect : IFreeEffect
	{
		private const long serialVersionUID = -6550441006104410613L;

		private string depth;

		private string ptype;

		private string culling;

		private string resUrl;

		private string model;

		private int gtype;

		private string alpha;

		private bool @fixed;

		private string fixPoint;

		private string xyz;

		private string scale;

		private string rotation;

		private IList<IFreeUIAuto> autos;

		private FreeTextComponet text;

		public override string GetXyz(IEventArgs args, IPosSelector selector)
		{
			return FreeUtil.ReplaceVar(xyz, args);
		}

		public virtual void SetXyz(string xyz)
		{
			this.xyz = xyz;
		}

		public override string GetScale(IEventArgs args)
		{
			if (text != null)
			{
				scale = text.width + "," + text.height + ",0";
			}
			return FreeUtil.ReplaceVar(scale, args);
		}

		public virtual void SetScale(string scale)
		{
			this.scale = scale;
		}

		public virtual FreeTextComponet GetText()
		{
			return text;
		}

		public virtual void SetText(FreeTextComponet text)
		{
			this.text = text;
		}

		public override string GetRotation(IEventArgs args, IPosSelector selector)
		{
			return FreeUtil.ReplaceVar(rotation, args);
		}

		public virtual void SetRotation(string rotation)
		{
			this.rotation = rotation;
		}

		public virtual string GetDepth()
		{
			return depth;
		}

		public virtual void SetDepth(string depth)
		{
			this.depth = depth;
		}

		public virtual string GetPtype()
		{
			return ptype;
		}

		public virtual void SetPtype(string ptype)
		{
			this.ptype = ptype;
		}

		public virtual string GetCulling()
		{
			return culling;
		}

		public virtual void SetCulling(string culling)
		{
			this.culling = culling;
		}

		public virtual string GetResUrl()
		{
			return resUrl;
		}

		public virtual void SetResUrl(string resUrl)
		{
			this.resUrl = resUrl;
		}

		public virtual string GetModel()
		{
			return model;
		}

		public virtual void SetModel(string model)
		{
			this.model = model;
		}

		public virtual int GetGtype()
		{
			return gtype;
		}

		public virtual void SetGtype(int gtype)
		{
			this.gtype = gtype;
		}

		public virtual string GetAlpha()
		{
			return alpha;
		}

		public virtual void SetAlpha(string alpha)
		{
			this.alpha = alpha;
		}

		public virtual string GetXyz()
		{
			return xyz;
		}

		public virtual string GetScale()
		{
			return scale;
		}

		public virtual string GetRotation()
		{
			return rotation;
		}

		public virtual IList<IFreeUIAuto> GetAutos()
		{
			return autos;
		}

		public virtual bool IsFixed()
		{
			return @fixed;
		}

		public virtual void SetFixed(bool @fixed)
		{
			this.@fixed = @fixed;
		}

		public virtual string GetFixPoint()
		{
			return fixPoint;
		}

		public virtual void SetFixPoint(string fixPoint)
		{
			this.fixPoint = fixPoint;
		}

		public override string GetStyle(IEventArgs args, string key)
		{
			string spliter = "_$$$_";
			if (StringUtil.IsNullOrEmpty(alpha))
			{
				alpha = "1";
			}
			if (text != null)
			{
				resUrl = text.GetStyle(args).Replace("_$$$_", "|||") + "|||" + FreeUtil.ReplaceInt(text.width, args) + "|||" + FreeUtil.ReplaceInt(text.height, args);
			}
			return FreeUtil.ReplaceVar(depth, args) + spliter + FreeUtil.ReplaceVar(ptype, args) + spliter + FreeUtil.ReplaceVar(culling, args) + spliter + FreeUtil.ReplaceVar(resUrl, args) + spliter + (model == null ? string.Empty : FreeUtil.ReplaceVar
				(model, args)) + spliter + gtype + spliter + FreeUtil.ReplaceFloat(alpha, args) + spliter + @fixed + spliter + (StringUtil.IsNullOrEmpty(fixPoint) ? "0,0" : FreeUtil.ReplaceVar(fixPoint, args));
		}

		public override IList<IFreeUIAuto> GetAutos(IEventArgs args)
		{
			return autos;
		}

		public override int GetKey(IEventArgs args)
		{
			return 1;
		}

		public virtual void SetAutos(IList<IFreeUIAuto> autos)
		{
			this.autos = autos;
		}
	}
}
