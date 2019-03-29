using System;
using Sharpen;
using com.wd.free.@event;
using com.wd.free.util;
using Free.framework;

namespace gameplay.gamerule.free.ui.component
{
	[System.Serializable]
	public class FreeTextComponet : AbstractFreeComponent
	{
		private const long serialVersionUID = -7699020163820046853L;

		private string text;

		private string size;

		private string color;

		private bool bold;

		private string font;

		private string hAlign;

		private string vAlign;

		private string multiLine;

		public override string GetStyle(IEventArgs args)
		{
			string realText = text;
			try
			{
				realText = FreeUtil.ReplaceVar(text, args);
			}
			catch (Exception)
			{
			}
			return realText + "_$$$_" + FreeUtil.ReplaceFloat(size, args) + "_$$$_" + FreeUtil.ReplaceVar(color, args) + "_$$$_" + bold + "_$$$_" + FreeUtil.ReplaceVar(font, args) + "_$$$_" + hAlign + "_$$$_" + vAlign + "_$$$_" + FreeUtil.ReplaceVar(multiLine
				, args);
		}

		public override int GetKey(IEventArgs args)
		{
			return TEXT;
		}

		public override IFreeUIValue GetValue()
		{
			return new FreeUITextValue();
		}

		public override SimpleProto CreateChildren(IEventArgs args)
		{
		    SimpleProto b = FreePool.Allocate();
			b.Key = 1;
			return b;
		}

		public virtual string GetText()
		{
			return text;
		}

		public virtual void SetText(string text)
		{
			this.text = text;
		}

		public virtual string GetSize()
		{
			return size;
		}

		public virtual void SetSize(string size)
		{
			this.size = size;
		}

		public virtual string GetColor()
		{
			return color;
		}

		public virtual void SetColor(string color)
		{
			this.color = color;
		}

		public virtual bool IsBold()
		{
			return bold;
		}

		public virtual void SetBold(bool bold)
		{
			this.bold = bold;
		}

		public virtual string GetFont()
		{
			return font;
		}

		public virtual void SetFont(string font)
		{
			this.font = font;
		}

		public virtual string GethAlign()
		{
			return hAlign;
		}

		public virtual void SethAlign(string hAlign)
		{
			this.hAlign = hAlign;
		}

		public virtual string GetvAlign()
		{
			return vAlign;
		}

		public virtual void SetvAlign(string vAlign)
		{
			this.vAlign = vAlign;
		}

		public virtual string GetMultiLine()
		{
			return multiLine;
		}

		public virtual void SetMultiLine(string multiLine)
		{
			this.multiLine = multiLine;
		}
	}
}
