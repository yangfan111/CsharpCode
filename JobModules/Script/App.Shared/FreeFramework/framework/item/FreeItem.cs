using Sharpen;
using com.cpkf.yyjd.tools.data.sort;
using com.cpkf.yyjd.tools.util;
using com.cpkf.yyjd.tools.util.math;
using com.wd.free.action;
using com.wd.free.para;
using com.wd.free.para.exp;
using com.wd.free.skill;
using com.wd.free.unit;

namespace com.wd.free.item
{
	[System.Serializable]
	public class FreeItem : IParable, IFeaturable
	{
		private const long serialVersionUID = -5059331058897789796L;

		private static int seq = 100000;

		private int id;

		private string key;

		private string name;

		private string desc;

	    private string cat;

		private string img;

		private string hotkey;

		private bool unique;

		private bool consume;

        private int useTime;

		private bool useAll;

		private string itemStack;

		private bool useClose;

		private bool isGoods;

		private string iconSize;

		private ISkill clickSkill;

		private IGameAction buyAction;

		private IGameAction createAction;

		private IGameAction addAction;

		private IGameAction removeAction;

		private string width;

		private string height;

		private IParaCondition condition;

		private IGameAction notReadyAction;

		private IParaCondition dragCondition;

		private IGameAction dragAction;

		[System.NonSerialized]
		private int gridWidth;

		[System.NonSerialized]
		private int gridHeight;

		[System.NonSerialized]
		private int count;

		private SimpleParaList paras;

		[System.NonSerialized]
		private byte[] cloneBytes;

		public FreeItem()
			: base()
		{
		}

		public FreeItem(string key)
			: base()
		{
			this.key = key;
			this.count = 1;
		}

	    public string Cat
	    {
	        get { return cat; }
	        set { cat = value; }
	    }

	    public virtual void SetId()
		{
			this.id = seq++;
		}

		public virtual int GetId()
		{
			return this.id;
		}

		public virtual bool IsGoods()
		{
			return isGoods;
		}

		public virtual string GetHotkey()
		{
			return hotkey;
		}

		public virtual void SetHotkey(string hotkey)
		{
			this.hotkey = hotkey;
		}

		public virtual ParaList GetParameters()
		{
			if (paras == null)
			{
				paras = new SimpleParaList();
				paras.AddFields(new ObjectFields(this));
			}
			if (paras.GetFieldList() == null)
			{
				paras.AddFields(new ObjectFields(this));
			}
			return paras;
		}

		public virtual void Drop(ISkillArgs args, UnitPosition pos)
		{
		}

		public virtual int GetItemWidth()
		{
			if (!StringUtil.IsNullOrEmpty(iconSize))
			{
				string[] vs = StringUtil.Split(iconSize, ",");
				if (vs.Length > 0)
				{
					return NumberUtil.GetInt(vs[0]);
				}
			}
			return 50;
		}

		public virtual IParaCondition GetDragCondition()
		{
			return dragCondition;
		}

		public virtual IGameAction GetDragAction()
		{
			return dragAction;
		}

		public virtual double GetIconScale()
		{
			if (!StringUtil.IsNullOrEmpty(iconSize))
			{
				string[] vs = StringUtil.Split(iconSize, ",");
				if (vs.Length > 2)
				{
					return NumberUtil.GetDouble(vs[2]) / 100d;
				}
			}
			return 1;
		}

		public virtual int GetItemHeight()
		{
			if (!StringUtil.IsNullOrEmpty(iconSize))
			{
				string[] vs = StringUtil.Split(iconSize, ",");
				if (vs.Length > 1)
				{
					return NumberUtil.GetInt(vs[1]);
				}
			}
			return 50;
		}

        public bool CanUse(ISkillArgs args)
        {
            if (condition != null && !condition.Meet(args))
            {
                if (notReadyAction != null)
                {
                    notReadyAction.Act(args);
                }
                return false;
            }

            return true;
        }

		public virtual bool Effect(ISkillArgs args)
		{
			if (isGoods)
			{
				args.TempUse("item", this);
				if (buyAction != null)
				{
					buyAction.Act(args);
				}
				args.Resume("item");
				return true;
			}
			if (condition == null || condition.Meet(args))
			{
				if (clickSkill != null && args != null)
				{
					args.TempUse("item", this);
					this.clickSkill.Frame(args);
					args.Resume("item");
				}
				return true;
			}
			else
			{
				if (condition != null && !condition.Meet(args))
				{
					if (notReadyAction != null)
					{
						notReadyAction.Act(args);
					}
				}
			}
			return false;
		}

		public virtual void Created(ISkillArgs args)
		{
			if (createAction != null && args != null)
			{
				args.TempUse("item", this);
				createAction.Act(args);
				args.Resume("item");
			}
		}

		public virtual void Added(ISkillArgs args)
		{
			if (addAction != null && args != null)
			{
				args.TempUse("item", this);
				addAction.Act(args);
				args.Resume("item");
			}
		}

		public virtual void Removed(ISkillArgs args)
		{
			if (removeAction != null && args != null)
			{
				args.TempUse("item", this);
				removeAction.Act(args);
				args.Resume("item");
			}
		}

		public virtual int GetCount()
		{
			return count;
		}

		public virtual void SetCount(int count)
		{
			this.count = count;
		}

		public virtual bool IsUseAll()
		{
			return useAll;
		}

		public virtual void SetUseAll(bool useAll)
		{
			this.useAll = useAll;
		}

        public string GetItemStackVar()
        {
            return itemStack;
        }

		public virtual int GetItemStack()
		{
            try
            {
                return int.Parse(itemStack);
            }catch{
                return 1;
            }
		}

		public virtual void SetItemStack(string itemStack)
		{
			this.itemStack = itemStack;
		}

		public virtual int GetGridWidth()
		{
			if (gridWidth == 0)
			{
				gridWidth = int.Parse(width);
			}
			return gridWidth;
		}

		public virtual int GetGridHeight()
		{
			if (gridHeight == 0)
			{
				gridHeight = int.Parse(height);
			}
			return gridHeight;
		}

		public virtual string GetWidth()
		{
			return width;
		}

		public virtual string GetHeight()
		{
			return height;
		}

		public virtual void SetWidth(string width)
		{
			this.width = width;
		}

		public virtual void SetHeight(string height)
		{
			this.height = height;
		}

		public virtual bool IsConsume()
		{
			return consume;
		}

		public virtual void SetConsume(bool consume)
		{
			this.consume = consume;
		}

		public virtual bool IsUnique()
		{
			return unique;
		}

		public virtual void SetUnique(bool unique)
		{
			this.unique = unique;
		}

		public virtual string GetKey()
		{
			return key;
		}

		public virtual void SetKey(string key)
		{
			this.key = key;
		}

		public virtual string GetImg()
		{
			return img;
		}

		public virtual void SetImg(string img)
		{
			this.img = img;
		}

		public virtual string GetIconSize()
		{
			return iconSize;
		}

		public virtual void SetIconSize(string iconSize)
		{
			this.iconSize = iconSize;
		}

		public virtual string GetName()
		{
			return name;
		}

		public virtual void SetName(string name)
		{
			this.name = name;
		}

        public int UseTime
        {
            get { return useTime; }
            set { useTime = value; }
        }

		public virtual string GetDesc()
		{
			return desc;
		}

		public virtual void SetDesc(string desc)
		{
			this.desc = desc;
		}

		public virtual bool IsUseClose()
		{
			return useClose;
		}

		public virtual void SetUseClose(bool useClose)
		{
			this.useClose = useClose;
		}

		public virtual com.wd.free.item.FreeItem Clone()
		{
			if (cloneBytes == null)
			{
				cloneBytes = SerializeUtil.ObjectToByte(this);
			}
			com.wd.free.item.FreeItem fi = (com.wd.free.item.FreeItem)SerializeUtil.ByteToObject(cloneBytes);
			fi.gridHeight = this.gridHeight;
			fi.gridWidth = this.gridHeight;
			return fi;
		}

		public override int GetHashCode()
		{
			int prime = 31;
			int result = 1;
			result = prime * result + ((key == null) ? 0 : key.GetHashCode());
			return result;
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj == null)
			{
				return false;
			}
			if (GetType() != obj.GetType())
			{
				return false;
			}
			com.wd.free.item.FreeItem other = (com.wd.free.item.FreeItem)obj;
			if (key == null)
			{
				if (other.key != null)
				{
					return false;
				}
			}
			else
			{
				if (!key.Equals(other.key))
				{
					return false;
				}
			}
			return true;
		}

		public virtual bool HasFeature(string feature)
		{
			return paras.HasFeature(feature);
		}

		public virtual object GetFeatureValue(string feature)
		{
			return paras.GetFeatureValue(feature);
		}
	}
}
