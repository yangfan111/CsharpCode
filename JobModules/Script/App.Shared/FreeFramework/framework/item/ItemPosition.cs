using Sharpen;
using com.cpkf.yyjd.tools.data.sort;
using com.wd.free.@event;
using com.wd.free.para;
using System;
using UnityEngine;

namespace com.wd.free.item
{
    public class ItemPosition : IFeaturable, IParable
    {
        private static int uniqueIndex = 1;

        public FreeItem key;

        public int x;

        public int y;

        public int count;

        internal ItemInventory inventory;

        [System.NonSerialized]
        private ObjectFields keyFields;

        [System.NonSerialized]
        private SimpleParaList paras;

        [System.NonSerialized]
        private string uiKey;

        [System.NonSerialized]
        private bool used;

        public ItemPosition()
            : base()
        {
            this.keyFields = new ObjectFields(this);
        }

        public ItemPosition(FreeItem key, int x, int y)
            : base()
        {
            this.key = key;
            this.x = x;
            this.y = y;
            this.count = key.GetCount();
            this.keyFields = new ObjectFields(key);
        }

        public virtual int GetCount()
        {
            return count;
        }

        public virtual bool DragTo(IEventArgs args, com.wd.free.item.ItemPosition target)
        {
            args.TempUse("source", this);
            args.TempUse("target", target);
            bool drag = false;
            if (key.GetDragCondition() != null && key.GetDragCondition().Meet(args))
            {
                if (key.GetDragAction() != null)
                {
                    key.GetDragAction().Act(args);
                    drag = true;
                }
            }
            args.Resume("source");
            args.Resume("target");
            return drag;
        }

        public virtual void Remove()
        {
            for (int i = x; i < x + key.GetGridWidth(); i++)
            {
                for (int j = y; j < y + key.GetGridHeight(); j++)
                {
                    inventory.ins[j][i] = false;
                }
            }
            inventory = null;
        }

        public virtual void SetCount(int count)
        {
            this.count = count;
            this.key.SetCount(count);
        }

        public virtual bool IsUsed()
        {
            return used;
        }

        public virtual void SetUsed(bool used)
        {
            this.used = used;
        }

        public virtual FreeItem GetKey()
        {
            return key;
        }

        public virtual void SetKey(FreeItem key)
        {
            this.key = key;

            this.keyFields.SetObj(key);

            IniParameters();
            foreach (IPara p in key.GetParameters().GetMapPara().Values)
            {
                paras.AddPara(p);
            }
        }

        public virtual int GetX()
        {
            return x;
        }

        public virtual void SetX(int x)
        {
            this.x = x;
        }

        public virtual int GetY()
        {
            return y;
        }

        public virtual void SetY(int y)
        {
            this.y = y;
        }

        public virtual string GetUIKey()
        {
            if (uiKey == null)
            {
                uiKey = "inventory_" + uniqueIndex++;
            }
            return uiKey;
        }

        public virtual ItemInventory GetInventory()
        {
            return inventory;
        }

        public override string ToString()
        {
            return key.GetKey() + ":" + "x=" + x + " y=" + y + " w=" + key.GetWidth() + " h=" + key.GetHeight() + " count=" + count;
        }

        public virtual bool HasFeature(string feature)
        {
            if (feature.Equals("x") || feature.Equals("y") || feature.Equals("inventory"))
            {
                return true;
            }
            return key.GetParameters().HasFeature(feature);
        }

        public virtual object GetFeatureValue(string feature)
        {
            if ("x".Equals(feature))
            {
                return x + 1;
            }
            if ("y".Equals(feature))
            {
                return y + 1;
            }
            if ("inventory".Equals(feature))
            {
                return inventory.name;
            }
            return key.GetParameters().GetFeatureValue(feature);
        }

        private void IniParameters()
        {
            if (paras == null)
            {
                paras = new SimpleParaList();
                paras.AddFields(new ObjectFields(this));
                paras.AddFields(keyFields);
                foreach (IPara p in key.GetParameters().GetMapPara().Values)
                {
                    paras.AddPara(p);
                }
            }
        }

        public virtual ParaList GetParameters()
        {
            IniParameters();
            return paras;
        }
    }
}
