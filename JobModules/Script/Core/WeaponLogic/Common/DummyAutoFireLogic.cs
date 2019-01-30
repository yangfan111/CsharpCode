using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.WeaponLogic.Common
{
    public class DummyAutoFireLogic : IAutoFireLogic
    {
        public bool Running
        {
            get { return false; }
        }

        public void AfterFireBullet(IPlayerWeaponState playerWeapon, IWeaponCmd cmd, int bullet)
        {

        }

        public void Reset()
        {

        }
    }
}
