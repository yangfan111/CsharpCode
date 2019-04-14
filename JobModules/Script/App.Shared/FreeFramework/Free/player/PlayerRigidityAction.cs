using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using App.Server.GameModules.GamePlay.free.player;
using App.Shared.FreeFramework.framework.unit;
using App.Shared.GameModules.Bullet;
using com.wd.free.para;
using Assets.Utils.Configuration;
using Utils.Singleton;

namespace App.Shared.FreeFramework.Free.player
{
    public class PlayerRigidityAction : AbstractGameAction
    {
        public const string RigidityEffect = "rigidity";

        public override void DoAction(IEventArgs args)
        {
            FreeData fd = (FreeData)args.GetUnit("target");

            if (fd != null)
            {
                SimpleParable damage = (SimpleParable)args.GetUnit("damage");
                if (damage != null)
                {
                    PlayerDamageInfo info = (PlayerDamageInfo)damage.GetFieldObject(0);

                    WeaponAllConfigs configs = SingletonManager.Get<WeaponConfigManagement>().FindConfigById(info.weaponId);

                    if(configs != null)
                    {
                        int time = configs.InitWeaponAllConfig.RigidityDuration;
                        int per = configs.InitWeaponAllConfig.RigidityEffect;

                        fd.EffectBufs.AddEffect(RigidityEffect, (float)per / 100, time);
                    }
                }
            }
        }
    }
}
