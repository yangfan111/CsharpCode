using System.Collections.Generic;
using Sharpen;
using com.wd.free.@event;
using com.wd.free.util;
using gameplay.gamerule.free.ui.component;

namespace gameplay.gamerule.free.ui
{
	public class FreeUIUtil
	{
		public static FreeUIUtil.Rectangle GetXY(IEventArgs args, IList<IFreeComponent> list, int index, int screenW, int screenH)
		{
			IFreeComponent component = list[index];
			int x = FreeUtil.ReplaceInt(component.GetX(args), args);
			int y = FreeUtil.ReplaceInt(component.GetY(args), args);
			int re = FreeUtil.ReplaceInt(component.GetRelative(args), args);
			int p = FreeUtil.ReplaceInt(component.GetParent(args), args);
			if (p == 0)
			{
				return GetXY(0, 0, screenW, screenH, x, y, re);
			}
			else
			{
				FreeUIUtil.Rectangle rec = GetXY(args, list, p - 1, screenW, screenH);
				return GetXY(rec.x, rec.y, screenW, screenH, x, y, re);
			}
		}

		// 向右x像素对于不同的相对位置给出的x值
		public static int GetRightX(int relative, int x)
		{
			switch (relative)
			{
				case 0:
				{
					return x;
				}

				case 1:
				{
					return -x;
				}

				case 2:
				{
					return x;
				}

				case 3:
				{
					return -x;
				}

				case 4:
				{
					return -x;
				}

				case 5:
				{
					return x;
				}

				case 6:
				{
					return -x;
				}

				case 7:
				{
					return x;
				}

				case 8:
				{
					return -x;
				}

				case 9:
				{
					return -x;
				}

				case 10:
				{
					return x;
				}

				case 11:
				{
					return -x;
				}

				default:
				{
					break;
				}
			}
			return x;
		}

		// 向下y像素对于不同的相对位置给出的y值
		public static int GetDownY(int relative, int y)
		{
			switch (relative)
			{
				case 0:
				{
					return y;
				}

				case 1:
				{
					return y;
				}

				case 2:
				{
					return -y;
				}

				case 3:
				{
					return -y;
				}

				case 4:
				{
					return -y;
				}

				case 5:
				{
					return -y;
				}

				case 6:
				{
					return y;
				}

				case 7:
				{
					return y;
				}

				case 8:
				{
					return y;
				}

				case 9:
				{
					return -y;
				}

				case 10:
				{
					return -y;
				}

				case 11:
				{
					return -y;
				}

				default:
				{
					break;
				}
			}
			return y;
		}

		public static FreeUIUtil.Rectangle GetXY(int startX, int startY, int width, int height, int x, int y, int relative)
		{
			FreeUIUtil.Rectangle rec = new FreeUIUtil.Rectangle();
			switch (relative)
			{
				case 0:
				{
					rec.x = startX + x;
					rec.y = startY + y;
					break;
				}

				case 1:
				{
					rec.x = startX + width - x;
					rec.y = startY + y;
					break;
				}

				case 2:
				{
					rec.x = startX + x;
					rec.y = startY + height - y;
					break;
				}

				case 3:
				{
					rec.x = startX + width - x;
					rec.y = startY + height - y;
					break;
				}

				case 4:
				{
					rec.x = startX + width / 2 - x;
					rec.y = startY + height / 2 - y;
					break;
				}

				case 5:
				{
					rec.x = startX + width / 2 + x;
					rec.y = startY + height / 2 - y;
					break;
				}

				case 6:
				{
					rec.x = startX + width / 2 - x;
					rec.y = startY + height / 2 + y;
					break;
				}

				case 7:
				{
					rec.x = startX + width / 2 + x;
					rec.y = startY + height / 2 + y;
					break;
				}

				case 8:
				{
					rec.x = startX + width / 2 - x;
					rec.y = startY + y;
					break;
				}

				case 9:
				{
					rec.x = startX + width / 2 - x;
					rec.y = startY + height - y;
					break;
				}

				case 10:
				{
					rec.x = startX + x;
					rec.y = startY + height / 2 - y;
					break;
				}

				case 11:
				{
					rec.x = startX + width - x;
					rec.y = startY + height / 2 - y;
					break;
				}

				default:
				{
					break;
				}
			}
			return rec;
		}

		public class Rectangle
		{
			public int x;

			public int y;

			public int width;

			public int height;

			public Rectangle()
				: base()
			{
			}

			public Rectangle(int x, int y, int width, int height)
				: base()
			{
				this.x = x;
				this.y = y;
				this.width = width;
				this.height = height;
			}

			public virtual bool In(int x, int y)
			{
				return (x <= this.x + width && x >= this.x) && (y <= this.y + height && y >= this.y);
			}

			public virtual bool In(FreeUIUtil.Rectangle r)
			{
				return (r.x <= x + width && r.x + r.width >= x) && (r.y <= y + height && r.y + r.height >= y);
			}

			public override string ToString()
			{
				return x + "," + y + "," + width + "," + height;
			}
		}
	}
}
