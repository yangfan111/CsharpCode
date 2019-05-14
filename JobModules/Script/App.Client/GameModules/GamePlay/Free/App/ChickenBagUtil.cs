using App.Shared;
using Assets.Sources.Free.UI;
using Core.Free;
using Free.framework;
using Shared.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Utils.Singleton;

namespace App.Client.GameModules.GamePlay.Free.App
{
    public class ChickenBagUtil
    {
        private static Dictionary<int, string> nameDic;

        private static Dictionary<int, string> avatarInv;
        private static Dictionary<string, int> invAvatar;

        static ChickenBagUtil()
        {
            nameDic = new Dictionary<int, string>();
            nameDic[0] = string.Empty;
            nameDic[1] = "ground";
            nameDic[2] = "default";

            avatarInv = new Dictionary<int, string>();
            avatarInv.Add((int)Wardrobe.Armor, "armor");
            avatarInv.Add((int)Wardrobe.Waist, "belt");
            avatarInv.Add((int)Wardrobe.Inner, "vest");
            avatarInv.Add((int)Wardrobe.Entirety, "coat");
            avatarInv.Add((int)Wardrobe.Outer, "coat");
            avatarInv.Add((int)Wardrobe.Glove, "glov");
            avatarInv.Add((int)Wardrobe.Trouser, "pant");
            avatarInv.Add((int)Wardrobe.PendantFace, "mask");
            avatarInv.Add((int)Wardrobe.Foot, "shoe");
            avatarInv.Add((int)Wardrobe.Bag, "bag");
            avatarInv.Add((int)Wardrobe.Cap, "hel");
            avatarInv.Add(0, "hel");

            invAvatar = new Dictionary<string, int>();
            foreach (var i in avatarInv.Keys)
            {
                if (!invAvatar.ContainsKey(avatarInv[i]))
                {
                    invAvatar.Add(avatarInv[i], i);
                }
            }
        }

        public static string GetAvatar(int avatar)
        {
            return avatarInv[avatar];
        }

        public static int GetAvatar(string inv)
        {
            return invAvatar[inv];
        }

        public static string GetBagKey(string key)
        {
            string[] ss = key.Split('|');
            if (ss.Length == 2)
            {
                int type = int.Parse(ss[0]);
                int pos = int.Parse(ss[1]);

                if (type == 4)
                {
                    return "w" + pos + ",0,0";
                }
                else if (type == 5)
                {
                    return "w" + pos / 10 + "" + GetPartFromWeaponPart(pos % 10) + "0,0";
                }
                else if (type == 3)
                {
                    return avatarInv[pos];
                }
                else if (nameDic.ContainsKey(type))
                {
                    if(type == 0)
                    {
                        return string.Empty;
                    }
                    else
                    {
                        return nameDic[type] + ",0," + pos;
                    }
                }
            }

            return key;
        }

        public static int GetPartFromInvPart(int part)
        {
            switch (part)
            {
                case 1:
                    return 3;
                case 2:
                    return 2;
                case 3:
                    return 5;
                case 4:
                    return 1;
                case 5:
                    return 6;
            }
            return part;
        }

        public static int GetPartFromWeaponPart(int part)
        {
            switch (part)
            {
                case 1:
                    return 4;
                case 2:
                    return 2;
                case 3:
                    return 1;
                case 5:
                    return 3;
                case 6:
                    return 5;
            }
            return part;
        }

        public static void ClickItem(string key, bool right)
        {
            SimpleProto data = FreePool.Allocate();
            data.Key = FreeMessageConstant.ClickImage;
            data.Ss.Add(GetBagKey(key));
            data.Bs.Add(right);
            data.Bs.Add(Input.GetKey(KeyCode.LeftControl));

            SingletonManager.Get<FreeUiManager>().Contexts1.session.clientSessionObjects.NetworkChannel.SendReliable((int)EClient2ServerMessage.FreeEvent, data);
        }

        public static void DragItem(string from, string to)
        {
            SimpleProto data = FreePool.Allocate();
            data.Key = FreeMessageConstant.DragImage;
            data.Ss.Add(GetBagKey(from));
            data.Ss.Add(GetBagKey(to));

            data.Bs.Add(Input.GetKey(KeyCode.LeftControl));

            SingletonManager.Get<FreeUiManager>().Contexts1.session.clientSessionObjects.NetworkChannel.SendReliable((int)EClient2ServerMessage.FreeEvent, data);
        }
    }
}
