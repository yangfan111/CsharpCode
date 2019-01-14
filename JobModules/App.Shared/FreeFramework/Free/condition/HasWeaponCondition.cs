using com.wd.free.para.exp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using App.Server.GameModules.GamePlay.free.player;

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
                    return fd.Player.GetBagLogicImp().GetCurrentWeaponInfo().Id == id;
                }
                else
                {
                    return fd.Player.GetBagLogicImp().HasWeapon(id);
                }   
            }

            return false;
        }
    }
}
