using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using App.Server.GameModules.GamePlay.free.player;
using App.Server.GameModules.GamePlay.Free.weapon;
using Core;
using App.Shared.GameModules.Weapon;

namespace App.Shared.FreeFramework.UnitTest
{
    [Serializable]
    public class WeaponTestValue : AbstractTestValue
    {
        public string slot;

        public override TestValue GetCaseValue(IEventArgs args)
        {
            TestValue tv = new TestValue();

            FreeData fd = (FreeData)args.GetUnit(UnitTestConstant.Tester);
            if (fd != null)
            {
                EWeaponSlotType currentSlot = fd.Player.WeaponController().HeldSlotType;

                if (args.GetInt(slot) > 0)
                {
                    currentSlot = FreeWeaponUtil.GetSlotType(args.GetInt(slot));
                }

               WeaponScanStruct info = fd.Player.WeaponController().HeldWeaponAgent.ComponentScan;
                tv.AddField("id", info.ConfigId);
                tv.AddField("clip", info.Bullet);
                tv.AddField("carryClip", info.ReservedBullet);
            }

            return tv;
        }

    }
}
