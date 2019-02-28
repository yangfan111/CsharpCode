using Assets.Utils.Configuration;
using com.cpkf.yyjd.tools.util.collection;
using com.cpkf.yyjd.tools.util.math;
using commons.data;
using commons.data.mysql;
using Sharpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using WeaponConfigNs;

namespace App.Server.GameModules.GamePlay.Free.item
{
    public class FreeItemDrop
    {
        private static MyDictionary<int, CatPriority> dropDic;
        private static MyDictionary<int, DropRange> rangeDic;
        private static MyDictionary<string, ItemPriority> catDic;
        private static MyDictionary<int, MyDictionary<int, CatPriority>> extraDic;

        private static Dictionary<int, List<ItemDrop>> dropCache;

        public static void Initial()
        {
            if (dropDic == null)
            {
                dropDic = new MyDictionary<int, CatPriority>();
                catDic = new MyDictionary<string, ItemPriority>();
                extraDic = new MyDictionary<int, MyDictionary<int, CatPriority>>();
                rangeDic = new MyDictionary<int, DropRange>();
                dropCache = new Dictionary<int, List<ItemDrop>>();

                List<DataRecord> dropList = MysqlUtil.SelectRecords("select * from new_item_drop", FreeRuleConfig.MysqlConnection);
                foreach (DataRecord dr in dropList)
                {
                    int id = int.Parse(dr.GetValue("id"));
                    string range = dr.GetValue("range");
                    string drop = dr.GetValue("drop");
                    string count = dr.GetValue("count");

                    rangeDic[id] = new DropRange(range, count);
                    dropDic[id] = new CatPriority(drop);
                }

                List<DataRecord> catList = MysqlUtil.SelectRecords("select * from new_drop_cat", FreeRuleConfig.MysqlConnection);
                MyDictionary<string, List<DataRecord>> catMap = new MyDictionary<string, List<DataRecord>>();
                foreach (DataRecord dr in catList)
                {
                    string cat = dr.GetValue("cat");
                    if (!catMap.ContainsKey(cat))
                    {
                        catMap[cat] = new List<DataRecord>();
                    }

                    catMap[cat].Add(dr);
                }
                foreach (string key in catMap.Keys)
                {
                    List<DataRecord> list = catMap[key];

                    int[] cats = new int[list.Count];
                    int[] ids = new int[list.Count];
                    int[] ps = new int[list.Count];
                    int[] cs = new int[list.Count];

                    for (int i = 0; i < list.Count; i++)
                    {
                        cats[i] = int.Parse(list[i].GetValue("itemCat"));
                        ids[i] = int.Parse(list[i].GetValue("itemId"));
                        ps[i] = int.Parse(list[i].GetValue("priority"));
                        cs[i] = int.Parse(list[i].GetValue("count"));
                    }

                    catDic[key] = new ItemPriority(cats, ids, ps, cs);
                }

                List<DataRecord> extraList = MysqlUtil.SelectRecords("select * from new_item_extra", FreeRuleConfig.MysqlConnection);
                foreach (DataRecord dr in extraList)
                {
                    int item = int.Parse(dr.GetValue("item"));
                    int cat = int.Parse(dr.GetValue("cat"));
                    string drop = dr.GetValue("extraDrop");
                    if (!extraDic.ContainsKey(cat))
                    {
                        extraDic.Add(cat, new MyDictionary<int, CatPriority>());
                    }
                    extraDic[cat][item] = new CatPriority(drop);
                }

                List<DataRecord> itemList = MysqlUtil.SelectRecords("select * from new_item", FreeRuleConfig.MysqlConnection);
                foreach (DataRecord dr in itemList)
                {
                    string item = dr.GetValue("item");
                    int id = int.Parse(dr.GetValue("id"));
                }

                InitialPoints();
            }
        }

        private static void InitialPoints()
        {
            foreach (MapConfigPoints.ID_Point point in MapConfigPoints.current.IDPints)
            {
                if (!dropCache.ContainsKey(point.ID))
                {
                    dropCache.Add(point.ID, GetDropItems(point.ID));
                }
            }
        }

        public static ItemDrop[] GetDropItems(string cat, int count)
        {
            Initial();

            List<ItemDrop> items = new List<ItemDrop>();

            if (catDic.ContainsKey(cat))
            {
                return catDic[cat].GetItems(count);
            }

            return new ItemDrop[0];
        }

        public static List<ItemDrop> GetExtraItems(ItemDrop item)
        {
            List<ItemDrop> list = new List<ItemDrop>();

            if (extraDic.ContainsKey(item.cat) && extraDic[item.cat].ContainsKey(item.id))
            {
                CatPriority cp = extraDic[item.cat][item.id];
                int[] cats = cp.GetCats(1);
                for (int i = 0; i < cats.Length; i++)
                {
                    if (catDic.ContainsKey(cp.cats[cats[i]]))
                    {
                        int index = cats[i];
                        ItemPriority itemPriority = catDic[cp.cats[index]];
                        int count = cp.counts[index];

                        for (int j = 0; j < count; j++)
                        {
                            foreach (ItemDrop drop in itemPriority.GetItems(1))
                            {
                                list.Add(drop);
                            }
                        }
                    }
                }
            }

            return list;
        }

        public static List<ItemDrop> GetDropItems(int id)
        {
            Initial();

            if (dropCache.ContainsKey(id))
            {
                return dropCache[id];
            }

            List<ItemDrop> items = new List<ItemDrop>();

            if (dropDic.ContainsKey(id) && null != MapConfigPoints.current)
            {
                CatPriority catPriority = dropDic[id];
                DropRange r = rangeDic[id];

                MapConfigPoints.ID_Point idPoint = null;

                List<MapConfigPoints.ID_Point> ps = MapConfigPoints.current.IDPints;
                for (int i = 0; i < ps.Count; i++)
                {
                    if (ps[i].ID == id)
                    {
                        idPoint = ps[i];
                        break;
                    }
                }

                if (idPoint != null)
                {
                    int itemCount = RandomUtil.Random(r.posFromCount, r.posToCount);
                    int[] pIndex = RandomUtil.Random(0, idPoint.points.Count - 1, itemCount);
                    for (int p = 0; p < pIndex.Length; p++)
                    {
                        Vector3 pos = idPoint.points[pIndex[p]];

                        int[] cats = catPriority.GetCats(RandomUtil.Random(r.dropFromCount, r.dropToCount));

                        for (int i = 0; i < cats.Length; i++)
                        {
                            if (catDic.ContainsKey(catPriority.cats[cats[i]]))
                            {
                                ItemPriority itemPriority = catDic[catPriority.cats[cats[i]]];
                                ItemDrop item = itemPriority.GetItems(1)[0];
                                item.pos = pos;

                                items.Add(item);
                            }
                        }
                    }
                }
            }

            return items;
        }
    }

    public class CatPriority
    {
        public string[] cats;
        public int[] prioritys;
        public int[] counts;

        public CatPriority(string drop)
        {
            string[] ss = drop.Split(",");
            cats = new string[ss.Length];
            prioritys = new int[ss.Length];
            counts = new int[ss.Length];

            for (int i = 0; i < ss.Length; i++)
            {
                string[] vs = ss[i].Split("=");
                if (vs.Length == 1)
                {
                    cats[i] = vs[0].Trim();
                    prioritys[i] = 1;
                    counts[i] = 1;
                }
                else if (vs.Length == 2)
                {
                    cats[i] = vs[0].Trim();
                    prioritys[i] = int.Parse(vs[1].Trim());
                    counts[i] = 1;
                }
                else if (vs.Length == 3)
                {
                    cats[i] = vs[0].Trim();
                    prioritys[i] = int.Parse(vs[2].Trim());
                    counts[i] = int.Parse(vs[1].Trim());
                }
            }
        }

        public CatPriority(string[] cats, int[] prioritys)
        {
            this.cats = cats;
            this.prioritys = prioritys;
            this.counts = new int[cats.Length];
            for (int i = 0; i < counts.Length; i++)
            {
                counts[i] = 1;
            }
        }

        public int[] GetCats(int n)
        {
            if (cats.Length == 1)
            {
                return new int[] { 0 };
            }

            return RandomUtil.RandomWithPro(0, prioritys, n);
        }
    }

    public class ItemPriority
    {
        public int[] cats;
        public int[] ids;
        public int[] counts;
        public int[] prioritys;

        public ItemPriority(int[] cats, int[] ids, int[] prioritys, int[] counts)
        {
            this.cats = cats;
            this.ids = ids;
            this.prioritys = prioritys;
            this.counts = counts;
        }

        public ItemDrop[] GetItems(int n)
        {
            if (cats.Length == 1)
            {
                return new ItemDrop[] { new ItemDrop(Vector3.zero, cats[0], ids[0], counts[0]) };
            }

            int[] indexs = RandomUtil.RandomWithPro(0, prioritys, n);

            ItemDrop[] ss = new ItemDrop[indexs.Length];
            for (int i = 0; i < indexs.Length; i++)
            {
                ss[i] = new ItemDrop(Vector3.zero, cats[indexs[i]], ids[indexs[i]], counts[indexs[i]]);
            }

            return ss;
        }
    }

    public class DropRange
    {
        public int posFromCount;
        public int posToCount;
        public int dropFromCount;
        public int dropToCount;

        public DropRange(int posFromCount, int posToCount, int dropFromCount, int dropToCount)
        {
            this.posFromCount = posFromCount;
            this.posToCount = posToCount;
            this.dropFromCount = dropFromCount;
            this.dropToCount = dropToCount;
        }

        public DropRange(string range, string count)
        {
            string[] ss = range.Split("-");
            if (ss.Length == 2)
            {
                this.posFromCount = int.Parse(ss[0]);
                this.posToCount = int.Parse(ss[1]);
            }
            else
            {
                this.posFromCount = int.Parse(ss[0]);
                this.posToCount = int.Parse(ss[0]);
            }

            ss = count.Split("-");
            if (ss.Length == 2)
            {
                this.dropFromCount = int.Parse(ss[0]);
                this.dropToCount = int.Parse(ss[1]);
            }
            else
            {
                this.dropFromCount = int.Parse(ss[0]);
                this.dropToCount = int.Parse(ss[0]);
            }
        }
    }

    public class ItemDrop
    {
        public Vector3 pos;
        public int cat;
        public int id;
        public int count;

        public ItemDrop()
        {

        }

        public ItemDrop(Vector3 pos, int cat, int id, int count)
        {
            this.pos = pos;
            this.cat = cat;
            this.id = id;
            this.count = count;
        }

        public bool IsEmpty
        {
            get { return count == 0; }
        }
    }
}
