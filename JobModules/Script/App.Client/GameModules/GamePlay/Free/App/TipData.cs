using App.Client.GameModules.GamePlay.Free.Auto.Prefab;
using App.Client.GameModules.Ui.Models.Common;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.Singleton;
using WeaponConfigNs;

namespace App.Client.GameModules.GamePlay.Free.App
{
    public class TipUtil
    {
        private static Dictionary<string, TipData> chickenDic = new Dictionary<string, TipData>();

        public static void AddTip(string key, TipData tip)
        {
            if (chickenDic.ContainsKey(key))
            {
                chickenDic.Remove(key);
            }

            chickenDic.Add(key, tip);
        }

        public static bool HasTip(string key)
        {
            return chickenDic.ContainsKey(key);
        }

        public static TipShowData GetCompareTip(BagItem item)
        {
            if (item.cat == 2)
            {
                NewWeaponConfigItem weapon = SingletonManager.Get<WeaponConfigManager>().GetConfigById(item.id);

                if (weapon.Type == (int)EWeaponType.PrimeWeapon)
                {
                    if (HasTip("w1"))
                    {
                        return new TipShowData(item.cat, item.id, chickenDic["w1"].id, 1);
                    }
                    else if (HasTip("w2"))
                    {
                        return new TipShowData(item.cat, item.id, chickenDic["w2"].id, 1);
                    }
                }
                else if (weapon.Type == (int)EWeaponType.SubWeapon)
                {
                    if (HasTip("w3"))
                    {
                        return new TipShowData(item.cat, item.id, chickenDic["w3"].id, 1);
                    }
                }

            }

            return new TipShowData(item.cat, item.id, 0, 1);
        }

        public static TipShowData GetTip(string key)
        {
            TipData td = chickenDic[key];

            if (td.cat == 2 && key.Contains(","))
            {
                NewWeaponConfigItem weapon = SingletonManager.Get<WeaponConfigManager>().GetConfigById(td.id);

                if (weapon.Type == (int)EWeaponType.PrimeWeapon)
                {
                    if (HasTip("w1"))
                    {
                        return new TipShowData(td.cat, td.id, chickenDic["w1"].id, 1);
                    }
                    else if (HasTip("w2"))
                    {
                        return new TipShowData(td.cat, td.id, chickenDic["w2"].id, 1);
                    }
                }
                else if (weapon.Type == (int)EWeaponType.SubWeapon)
                {
                    if (HasTip("w3"))
                    {
                        return new TipShowData(td.cat, td.id, chickenDic["w3"].id, 1);
                    }
                }

            }

            return new TipShowData(td.cat, td.id, 0, td.count);
        }

    }

    public class TipData
    {
        public int cat;
        public int id;
        public int count;

        public TipData(int cat, int id, int count)
        {
            this.cat = cat;
            this.id = id;
            this.count = count;
        }
    }
}
