using com.wd.free.para.exp;
using System;
using com.wd.free.@event;
using App.Server.GameModules.GamePlay.free.player;
using App.Shared.GameModules.Weapon;
using Core.Free;

namespace App.Shared.FreeFramework.Free.condition
{
    [Serializable]
    public class HasWeaponCondition : IParaCondition, IRule
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
                    return fd.Player.WeaponGetter().HeldWeaponAgent.ConfigId == id;
                }
                else
                {
                    return fd.Player.WeaponGetter().IsWeaponInSlot(id);
                }   
            }

            return false;
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.HasWeaponCondition;
        }
    }
}
