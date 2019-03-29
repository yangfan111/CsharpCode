using com.wd.free.unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Shared.FreeFramework.Free.Weapon
{
    public class WeaponFreeData : BaseGameUnit
    {
        public int WeaponId;

        public WeaponFreeData(int id)
        {
            this.WeaponId = id;
        }
    }
}
