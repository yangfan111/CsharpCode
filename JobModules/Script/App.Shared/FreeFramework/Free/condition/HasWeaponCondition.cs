using com.wd.free.para.exp;
using System;
using com.wd.free.@event;
using App.Server.GameModules.GamePlay.free.player;
using App.Shared.GameModules.Weapon;

namespace App.Shared.FreeFramework.Free.condition
{
    [Serializable]
    public class HasWeaponCondition : IParaCondition
    {
        private string player;
        private bool hand;
        private int id;

        public bool Meet(IEventArgs args)
        {
            FreeData fd = (FreeData)args.GetUnit(player);
            if(fd != null)
            {
                if (hand)
                {
                    return fd.Player.WeaponController().HeldWeaponAgent.ConfigId == id;
                }
                else
                {
                    return fd.Player.WeaponController().IsWeaponInSlot(id);
                }   
            }

            return false;
        }
    }
}
