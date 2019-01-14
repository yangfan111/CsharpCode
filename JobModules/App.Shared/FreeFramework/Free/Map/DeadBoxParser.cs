using Core.Free;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.Singleton;

namespace App.Shared.GameModules.GamePlay.Free.Map
{
    public class DeadBoxParser : Singleton<DeadBoxParser>, IFreeEntityParser
    {
        public object FromString(string value)
        {
            string[] ss = value.Split(new[] { FreeMessageConstant.SpilterField }, StringSplitOptions.None);
            if (ss.Length == 4)
            {
                return new SimpleItemInfo(ss[0].Trim(), int.Parse(ss[1].Trim()), int.Parse(ss[2].Trim()), int.Parse(ss[3].Trim()), 0);
            }

            return null;
        }

        public string ToString(object obj)
        {
            StringBuilder sb = new StringBuilder();

            SimpleItemInfo box = (SimpleItemInfo)obj;
            sb.Append(box.name);
            sb.Append(FreeMessageConstant.SpilterField);
            sb.Append(box.cat);
            sb.Append(FreeMessageConstant.SpilterField);
            sb.Append(box.id);
            sb.Append(FreeMessageConstant.SpilterField);
            sb.Append(box.count);

            return sb.ToString();
        }
    }

    public struct SimpleItemInfo
    {
        public string name;
        public int cat;
        public int id;
        public int count;
        public int entityId;

        public SimpleItemInfo(string name, int cat, int id, int count, int entityId)
        {
            this.name = name;
            this.cat = cat;
            this.id = id;
            this.count = count;
            this.entityId = entityId;
        }
    }
}
