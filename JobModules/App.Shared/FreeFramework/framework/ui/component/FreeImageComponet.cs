using Sharpen;
using com.wd.free.@event;
using com.wd.free.util;
using Free.framework;

namespace gameplay.gamerule.free.ui.component
{
	[System.Serializable]
	public class FreeImageComponet : AbstractFreeComponent
	{
		private const long serialVersionUID = -7699020163820046853L;

		private string url;

		private string center;

		private string originalSize;

		private string coverUrl;

		private string @fixed;

		private string noMouse;

		private string useMask;

		private string isMask;

		private string mirror;

		public override string GetStyle(IEventArgs args)
		{
			return FreeUtil.ReplaceVar(url, args) + "_$$$_" + center + "_$$$_" + FreeUtil.ReplaceVar(originalSize, args) + "_$$$_" + FreeUtil.ReplaceVar(coverUrl, args) + "_$$$_" + FreeUtil.ReplaceVar(@fixed, args) + "_$$$_" + FreeUtil.ReplaceVar(noMouse
				, args) + "_$$$_" + FreeUtil.ReplaceVar(useMask, args) + "_$$$_" + FreeUtil.ReplaceVar(isMask, args) + "_$$$_" + FreeUtil.ReplaceVar(mirror, args);
		}

		public override int GetKey(IEventArgs args)
		{
			return IMAGE;
		}

		public virtual string GetFixed()
		{
			return @fixed;
		}

		public virtual void SetFixed(string @fixed)
		{
			this.@fixed = @fixed;
		}

		public virtual string GetUrl()
		{
			return url;
		}

		public virtual void SetUrl(string url)
		{
			this.url = url;
		}

		public virtual string GetOriginalSize()
		{
			return originalSize;
		}

		public virtual void SetOriginalSize(string originalSize)
		{
			this.originalSize = originalSize;
		}

		public virtual string GetNoMouse()
		{
			return noMouse;
		}

		public virtual void SetNoMouse(string noMouse)
		{
			this.noMouse = noMouse;
		}

		public override IFreeUIValue GetValue()
		{
			return new FreeUIImageValue();
		}

		public override SimpleProto CreateChildren(IEventArgs args)
		{
			SimpleProto b = FreePool.Allocate();
			b.Key = 1;
			return b;
		}
	}
}
