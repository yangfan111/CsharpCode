using App.Shared.GameModules.Weapon;
using Assets.App.Shared.EntityFactory;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using Core;
using Core.EntityComponent;
using Core.Room;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Shared.GameModeLogic.WeaponInitLoigc
{
    /// <summary>
    /// Defines the <see cref="DummyWeaponInitializeHandler" />
    /// </summary>
    public class DummyWeaponInitializeHandler : IWeaponInitHandler
    {
        public void Recovery(IPlayerWeaponGetter controller)
        {
        }

        public void Recovery(IPlayerWeaponGetter controller, int index)
        {
        }

        public void RecoveryBagContainer(int index, IPlayerWeaponGetter controller)
        {
        }

        public void RecoveryBagState(IPlayerWeaponGetter controller)
        {
        }

        public void ResetPlayerWeaponData(IPlayerWeaponGetter controller)
        {
            throw new NotImplementedException();
        }

        public void TrashPlayerBagState(IPlayerWeaponGetter controller)
        {
            throw new NotImplementedException();
        }

        public bool CanModeSwitchBag
        {
            get { return false; }
        }
        public int ModeId { get { return 0; } }
    }
}
