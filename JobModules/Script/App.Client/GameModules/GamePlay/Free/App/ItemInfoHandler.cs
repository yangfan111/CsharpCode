using Assets.Sources.Free;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Free.framework;
using Core.Free;
using Utils.Singleton;
using Assets.Sources.Free.UI;
using Shared.Scripts;

namespace App.Client.GameModules.GamePlay.Free.App
{
    public class ItemInfoHandler : ISimpleMesssageHandler
    {
        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.ItemInfo;
        }

        public void Handle(SimpleProto data)
        {
            //string key = data.Ss[0];

            //TipData tip = new TipData(data.Ins[0], data.Ins[1], data.Ins[2]);

            //TipUtil.AddTip(key, tip);

            Contexts contexts = SingletonManager.Get<FreeUiManager>().Contexts1;
            var ui = contexts.ui.uI;

            string inv = data.Ss[0];

            if (inv.StartsWith("w") && inv.Length == 2)
            {
                int slot = int.Parse(inv.Substring(1));
                if (data.Bs[0])
                {
                    ui.WeaponIdList[slot] = 0;
                }
                else
                {
                    ui.WeaponIdList[slot] = data.Ins[1];
                }
            }
            else if (inv.StartsWith("w") && inv.Length == 3)
            {
                int slot = int.Parse(inv.Substring(1, 1));
                int part = int.Parse(inv.Substring(2, 1));
                part = ChickenBagUtil.GetPartFromInvPart(part);
                if (data.Bs[0])
                {
                    ui.WeaponPartList[slot, part] = 0;
                }
                else
                {
                    ui.WeaponPartList[slot, part] = data.Ins[1];
                }
            }
            else
            {
                int equip = ChickenBagUtil.GetAvatar(inv);

                if (inv == "coat")
                {
                    if (data.Bs[0])
                    {
                        ui.EquipIdList[(int)Wardrobe.Outer] = new KeyValuePair<int, int>(0, 0);
                        ui.EquipIdList[(int)Wardrobe.Entirety] = new KeyValuePair<int, int>(0, 0);
                    }
                    else
                    {
                        ui.EquipIdList[(int)Wardrobe.Outer] = new KeyValuePair<int, int>(data.Ins[1], data.Ins[2]);
                        ui.EquipIdList[(int)Wardrobe.Entirety] = new KeyValuePair<int, int>(data.Ins[1], data.Ins[2]);
                    }
                }
                else
                {
                    if (data.Bs[0])
                    {
                        ui.EquipIdList[equip] = new KeyValuePair<int, int>(0, 0);
                    }
                    else
                    {
                        ui.EquipIdList[equip] = new KeyValuePair<int, int>(data.Ins[1], data.Ins[2]);
                    }
                }
            }
        }
    }
}
