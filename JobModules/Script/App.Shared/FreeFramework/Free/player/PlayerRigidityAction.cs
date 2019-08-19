using App.Server.GameModules.GamePlay.free.player;
using App.Shared.GameModules.Attack;
using Assets.Utils.Configuration;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.para;
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

                        fd.EffectBufs.AddEffect(RigidityEffect, (float)per / 100, time, args);
                    }
                }
            }
        }
    }
}
